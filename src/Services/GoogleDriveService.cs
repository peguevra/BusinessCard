using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class GoogleDriveService
{
    private DriveService _service;
    private const string FolderId = "YOUR_FOLDER_ID";

    public GoogleDriveService()
    {
        _service = Create();
    }

    private DriveService Create()
    {
        using var stream = new FileStream("credentials.json", FileMode.Open);

        var cred = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { DriveService.Scope.DriveFile },
            "user",
            CancellationToken.None,
            new FileDataStore("token.json", true)
        ).Result;

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = cred,
            ApplicationName = "BusinessCard"
        });
    }

    public string UploadPdf(string filePath, string name)
    {
        var meta = new Google.Apis.Drive.v3.Data.File
        {
            Name = name,
            Parents = new List<string> { FolderId }
        };

        using var stream = new FileStream(filePath, FileMode.Open);

        var req = _service.Files.Create(meta, stream, "application/pdf");
        req.Fields = "id";
        req.Upload();

        var id = req.ResponseBody.Id;

        _service.Permissions.Create(new Google.Apis.Drive.v3.Data.Permission
        {
            Type = "anyone",
            Role = "reader"
        }, id).Execute();

        return $"https://drive.google.com/file/d/{id}/view";
    }

    public void UploadOrUpdateHtml(string path)
    {
        var fileId = Find("index.html");

        if (fileId == null)
            UploadNew(path);
        else
            Update(fileId, path);
    }

    private string? Find(string name)
    {
        var req = _service.Files.List();
        req.Q = $"name='{name}' and '{FolderId}' in parents and trashed=false";
        req.Fields = "files(id,name)";

        return req.Execute().Files.FirstOrDefault()?.Id;
    }

    private void UploadNew(string path)
    {
        var meta = new Google.Apis.Drive.v3.Data.File
        {
            Name = "index.html",
            Parents = new List<string> { FolderId }
        };

        using var stream = new FileStream(path, FileMode.Open);

        var req = _service.Files.Create(meta, stream, "text/html");
        req.Fields = "id";
        req.Upload();
    }

    private void Update(string id, string path)
    {
        using var stream = new FileStream(path, FileMode.Open);

        var req = _service.Files.Update(
            new Google.Apis.Drive.v3.Data.File(),
            id,
            stream,
            "text/html");

        req.Upload();
    }
}