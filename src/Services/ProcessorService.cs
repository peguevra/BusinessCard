using System;
using System.IO;

public class ProcessorService
{
    public void Execute(string filePath, GlobalPaths paths)
    {
        try
        {
            SafeLog.Info("処理開始: " + filePath);

            // =========================
            // 1. 入力（Category + Name）
            // =========================
            var input = InputForm.ShowDialog();

            var category = input.category;
            var name = input.name;

            if (string.IsNullOrWhiteSpace(name))
            {
                SafeLog.Info("キャンセル");
                return;
            }

            // =========================
            // 2. ファイル名生成
            // =========================
            var newFileName = $"{category} {name}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            var donePath = Path.Combine(paths.DoneDir, newFileName);

            File.Copy(filePath, donePath, true);

            // 元ファイル削除
            File.Delete(filePath);

            // =========================
            // 3. Google Driveアップロード
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

                // Drive失敗でも継続
                url = "LOCAL_ONLY";
            }

            // =========================
            // 4. CSV追記
            // =========================
            var record = new BuisicessCardRecord
            {
                Category = category,
                Name = name,
                Url = url,
                CreatedAt = DateTime.Now
            };

            var csv = new CsvService();

            try
            {
                csv.Append(record, paths.OutputDir);
                SafeLog.Info("CSV追記完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("CSV失敗: " + ex);
            }

            // =========================
            // 5. JSON生成
            // =========================
            try
            {
                SafeLog.Info("JSON生成開始");
                new JsonService().Generate(paths.OutputDir, paths.DeployDir);
                SafeLog.Info("JSON生成完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("JSON生成失敗: " + ex);
            }

            // =========================
            // 6. GitHub Deploy（deploy2）
            // =========================
            try
            {
                SafeLog.Info("GitHub Deploy開始");
                new DeployService().Deploy();
                SafeLog.Info("GitHub Deploy完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("Deploy失敗: " + ex);
            }

            SafeLog.Info("処理完了: " + name);
        }
        catch (Exception ex)
        {
            SafeLog.Error("致命的エラー: " + ex);
        }
    }
}