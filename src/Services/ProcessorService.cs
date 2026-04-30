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

            var (category, name, updateOnly) = InputForm.ShowDialog();

            // =========================
            // 更新モード（CSV→JSONなど）
            // =========================
            if (updateOnly)
            {
                SafeLog.Info("更新モード開始");

                GenerateAndDeploy(paths);

                SafeLog.Info("更新モード完了");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
            {
                SafeLog.Info("キャンセル");
                return;
            }

            // =========================
            // 1. ファイル移動
            // =========================
            var newFileName = $"{category}_{name}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var donePath = Path.Combine(paths.DoneDir, newFileName);

            File.Copy(filePath, donePath, true);
            File.Delete(filePath);

            // =========================
            // 2. Driveアップロード
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
                url = "LOCAL_ONLY";
            }

            // =========================
            // 3. レコード生成
            // =========================
            var record = new BuisicessCardRecord
            {
                Category = category,
                Name = name,
                Url = url,
                CreatedAt = DateTime.Now
            };

            // =========================
            // 4. CSV追記（ログ用途）
            // =========================
            try
            {
                new CsvService().Append(record, paths.OutputDir);
                SafeLog.Info("CSV追記完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("CSV失敗: " + ex);
            }

            // =========================
            // 5. Supabase登録（★本命）
            // =========================
            try
            {
                SafeLog.Info("Supabase登録開始");

                var supabase = new SupabaseService();
                supabase.Insert(record).Wait();

                SafeLog.Info("Supabase登録完了");
            }
            catch (Exception ex)
            {
                SafeLog.Error("Supabase失敗: " + ex);
            }

            // =========================
            // 6. JSON＋Deploy（保険 / 互換）
            // =========================
            try
            {
                GenerateAndDeploy(paths);
            }
            catch (Exception ex)
            {
                SafeLog.Error("旧処理失敗: " + ex);
            }

            SafeLog.Info("処理完了: " + name);
        }
        catch (Exception ex)
        {
            SafeLog.Error("致命的エラー: " + ex);
        }
    }

    private void GenerateAndDeploy(GlobalPaths paths)
    {
        try
        {
            SafeLog.Info("JSON生成開始");
            new JsonService().Generate(paths);
            SafeLog.Info("JSON生成完了");
        }
        catch (Exception ex)
        {
            SafeLog.Error("JSON失敗: " + ex);
        }

        try
        {
            SafeLog.Info("GitHub Deploy開始");
            new DeployService().DeployJson();
            SafeLog.Info("GitHub Deploy完了");
        }
        catch (Exception ex)
        {
            SafeLog.Error("Deploy失敗: " + ex);
        }
    }
}