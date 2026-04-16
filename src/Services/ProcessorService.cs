public class ProcessorService
{
    public void Process(string filePath, GlobalPaths paths)
    {
        try
        {
            WaitForFile(filePath);

            string name = InputForm.ShowDialog();

            if (string.IsNullOrWhiteSpace(name))
            {
                SafeLog.Info("入力キャンセル");
                return;
            }

            var drive = new GoogleDriveService();
            string url = drive.UploadAndGetLink(filePath, name);

            var record = new BuisicessCardRecord
            {
                No = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Name = name,
                Url = url,
                Date = DateTime.Now
            };

            var csv = new CsvService();
            csv.Append(paths.CsvPath, record);

            // ★ HTML生成追加
            var html = new HtmlService();
            html.Generate(paths);

            MoveToDone(filePath, paths.DoneDir, name);

            SafeLog.Info($"処理完了: {name}");
        }
        catch (Exception ex)
        {
            SafeLog.Error(ex.ToString());
        }
    }

    private void WaitForFile(string path)
    {
        while (true)
        {
            try
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                break;
            }
            catch
            {
                Thread.Sleep(500);
            }
        }
    }

    private void MoveToDone(string filePath, string doneDir, string name)
    {
        string ext = Path.GetExtension(filePath);
        string safeName = Sanitize(name);

        string destPath = Path.Combine(doneDir,
            $"{safeName}_{DateTime.Now:yyyyMMddHHmmss}{ext}");

        File.Move(filePath, destPath);
    }

    private string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}