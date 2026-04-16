using System.Diagnostics;
using CommonBatchFramework.App;

public class ProcessorService
{
    private static readonly object _gitLock = new();

    public void Execute(string filePath, GlobalPaths paths)
    {
        Log.Info($"処理開始: {filePath}");

        try
        {
            // 1. レコード生成（最小構造に統一）
            var record = Parse(filePath);

            // 2. CSV更新
            new CsvService().Append(paths.CsvPath, record);

            // 3. HTML更新（ここが正）
            new HtmlService().Generate(paths);

            // 4. Driveアップロード（PDF）
            var driveUrl = new GoogleDriveService()
                .UploadAndGetLink(filePath, Path.GetFileName(filePath));

            Log.Info($"Driveアップロード完了: {driveUrl}");

            // 5. HTMLもDriveへ（同じ固定ファイル）
            new GoogleDriveService()
                .UploadAndGetLink(paths.HtmlPath, "index.html");

            // 6. Git push
            PushToGit();

            Log.Info("処理完了");
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    private BuisicessCardRecord Parse(string filePath)
    {
        return new BuisicessCardRecord
        {
            No = DateTime.Now.ToString("yyyyMMddHHmmss"),
            Name = InputForm.ShowDialog(), // ←ここ重要（UI入力）
            Company = "",
            Url = "",
            Date = DateTime.Now,
            Note = filePath
        };
    }

    private void PushToGit()
    {
        lock (_gitLock)
        {
            Run("git add .");
            Run("git commit -m \"auto update\"");
            Run("git push");
        }
    }

    private void Run(string cmd)
    {
        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c " + cmd;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
    }
}