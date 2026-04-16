public class GoogleDriveService
{
    public string UploadAndGetLink(string filePath, string name)
    {
        string id = Guid.NewGuid().ToString("N");
        return $"https://drive.google.com/file/d/{id}/view";
    }
}