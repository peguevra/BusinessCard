using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class GoogleDriveService
{
    private DriveService _service;

    private const string FolderId = "1iB4xhSNNXZVhT5hZf57sc5bj1eDYL1kL";

    public GoogleDriveService()
    {
        _service = CreateService();
    }

    private DriveService CreateService()
    {
        // ★Program.csと同じ場所基準
        var baseDir = AppContext.BaseDirectory;

        // bin → プロジェクトルートへ戻す
        var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));

        var credentialPath = Path.Combine(projectRoot, "credentials.json");

        if (!File.Exists(credentialPath))
        {
            throw new FileNotFoundException("credentials.json が見つかりません", credentialPath);
        }

        using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);

        var credPath = Path.Combine(projectRoot, "token.json");

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { DriveService.Scope.DriveFile },
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)
        ).Result;

        return new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "BusinessCardApp"
        });
    }

    public string UploadPdf(string filePath, string name)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = name,
            Parents = new List<string> { FolderId }
        };

        using var stream = new FileStream(filePath, FileMode.Open);

        var request = _service.Files.Create(fileMetadata, stream, "application/pdf");
        request.Fields = "id";
        request.Upload();

        var file = request.ResponseBody;

        var permission = new Google.Apis.Drive.v3.Data.Permission()
        {
            Type = "anyone",
            Role = "reader"
        };

        _service.Permissions.Create(permission, file.Id).Execute();

        return $"https://drive.google.com/file/d/{file.Id}/view";
    }

    public void UploadOrUpdateHtml(string filePath)
    {
        var fileId = FindFileId("index.html");

        if (fileId == null)
        {
            UploadNewHtml(filePath);
        }
        else
        {
            UpdateHtml(fileId, filePath);
        }
    }

    private string? FindFileId(string fileName)
    {
        var listRequest = _service.Files.List();
        listRequest.Q = $"name = '{fileName}' and '{FolderId}' in parents and trashed = false";
        listRequest.Fields = "files(id, name)";

        var result = listRequest.Execute();

        return result.Files.FirstOrDefault()?.Id;
    }

    private void UploadNewHtml(string filePath)
    {
        var metadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = "index.html",
            Parents = new List<string> { FolderId }
        };

        using var stream = new FileStream(filePath, FileMode.Open);

        var request = _service.Files.Create(metadata, stream, "text/html");
        request.Fields = "id";
        request.Upload();

        var file = request.ResponseBody;

        var permission = new Google.Apis.Drive.v3.Data.Permission()
        {
            Type = "anyone",
            Role = "reader"
        };

        _service.Permissions.Create(permission, file.Id).Execute();
    }

    private void UpdateHtml(string fileId, string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open);

        var request = _service.Files.Update(
            new Google.Apis.Drive.v3.Data.File(),
            fileId,
            stream,
            "text/html"
        );

        request.Upload();
    }
}