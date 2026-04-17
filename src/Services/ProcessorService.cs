using System;
using System.IO;
using BuisinessCard.Services;

public class ProcessorService
{
    public void Execute(string filePath, GlobalPaths paths)
    {
        try
        {
            SafeLog.Info("処理開始: " + filePath);

            // =========================
            // 1. 名前入力
            // =========================
            var name = InputForm.ShowDialog();

            if (string.IsNullOrWhiteSpace(name))
            {
                SafeLog.Info("キャンセル");
                return;
            }

            // =========================
            // 2. ファイル名生成
            // =========================
            var newFileName = $"{name}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            var donePath = Path.Combine(paths.DoneDir, newFileName);

            File.Copy(filePath, donePath, true);

            // 元ファイル削除（重要）
            File.Delete(filePath);

            // =========================
            // 3. Google Driveアップロード（安全化）
            // =========================
            var drive = new GoogleDriveService();

            string url;

            try
            {
                SafeLog.Info("Driveアップロード開始");
                url = drive.UploadPdf(donePath, newFileName);
                SafeLog.Info("Driveアップロード完了: " + url);
            }
            catch (Exception ex)
            {
                SafeLog.Error("Drive失敗: " + ex);

                // Drive失敗でも処理継続
                url = "LOCAL_ONLY";
            }

            // =========================
            // 4. CSV追記（唯一の正本）
            // =========================
            var record = new BuisicessCardRecord
            {
                Name = name,
                Url = url,
                CreatedAt = DateTime.Now
            };

            var csv = new CsvService();

            try
            {
                csv.Append(record, paths.OutputDir);
            }
            catch (Exception ex)
            {
                SafeLog.Error("CSV失敗: " + ex);
            }

            // =========================
            // 5. HTML再生成（CSVから毎回生成）
            // =========================
            var html = new HtmlService();

            try
            {
                html.GenerateFromCsv(paths.OutputDir);
            }
            catch (Exception ex)
            {
                SafeLog.Error("HTML生成失敗: " + ex);
            }

            // =========================
            // 6. HTMLをGoogle Driveへ反映
            // =========================
            try
            {
                var htmlPath = Path.Combine(paths.OutputDir, "index.html");

                SafeLog.Info("HTMLアップロード開始");
                drive.UploadOrUpdateHtml(htmlPath);
                SafeLog.Info("HTMLアップロード完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("HTML Drive反映失敗: " + ex);
            }

            // =========================
            // 7. GitHub Deploy（追加）
            // =========================
            try
            {
                SafeLog.Info("GitHub Deploy開始");
                new DeployService().Deploy();
                SafeLog.Info("GitHub Deploy完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("Deploy呼び出し失敗: " + ex);
            }

            SafeLog.Info("処理完了: " + name);
        }
        catch (Exception ex)
        {
            SafeLog.Error("致命的エラー: " + ex);
        }
    }
}