using System;
using System.IO;

public class ProcessorService
{
    public void Execute(string filePath, GlobalPaths paths)
    {
        try
        {
            SafeLog.Info("処理開始: " + filePath);

            var name = InputForm.ShowDialog();

            if (string.IsNullOrWhiteSpace(name))
            {
                SafeLog.Info("キャンセル");
                return;
            }

            var newFileName = $"{name}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var donePath = Path.Combine(paths.DoneDir, newFileName);

            File.Copy(filePath, donePath, true);
            File.Delete(filePath);

            var drive = new GoogleDriveService();

            string url = "";
            try
            {
                url = drive.UploadPdf(donePath, newFileName);
            }
            catch (Exception ex)
            {
                SafeLog.Error("Drive失敗: " + ex);
                url = "LOCAL_ONLY";
            }

            var record = new BuisicessCardRecord
            {
                Name = name,
                Url = url,
                CreatedAt = DateTime.Now
            };

            // CSV（唯一の正本）
            new CsvService().Append(record, paths.OutputDir);

            // HTMLはCSVから再生成
            new HtmlService().GenerateFromCsv(paths);

            // GitHubへ反映
            try
            {
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c git add . && git commit -m \"auto update\" && git push";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                SafeLog.Error("Git失敗: " + ex);
            }

            SafeLog.Info("処理完了: " + name);
        }
        catch (Exception ex)
        {
            SafeLog.Error("致命的エラー: " + ex);
        }
    }
}