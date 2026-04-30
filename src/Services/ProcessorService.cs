using System;
using System.IO;

public class ProcessorService
{
    public void Execute(string filePath, GlobalPaths paths)
    {
        try
        {
            SafeLog.Info("処理開始: " + filePath);

            var (category, name, updateOnly) = InputForm.ShowDialog();

            // =========================
            // ★ 更新モード
            // =========================
            if (updateOnly)
            {
                SafeLog.Info("更新モード開始");

                var supabase = new SupabaseService();
                var list = supabase.GetAll().Result;

                new CsvService().Rewrite(list, paths.OutputDir);

                SafeLog.Info("CSV再生成完了");
                SafeLog.Info("更新モード完了");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
            {
                SafeLog.Info("キャンセル");
                return;
            }

            // =========================
            // ファイル移動
            // =========================
            var newFileName = $"{category}_{name}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var donePath = Path.Combine(paths.DoneDir, newFileName);

            File.Copy(filePath, donePath, true);
            File.Delete(filePath);

            // =========================
            // Drive
            // =========================
            string url;

            try
            {
                var drive = new GoogleDriveService();
                url = drive.UploadPdf(donePath, newFileName);
            }
            catch (Exception ex)
            {
                SafeLog.Error("Drive失敗: " + ex);
                return;
            }

            var record = new BuisicessCardRecord
            {
                Category = category,
                Name = name,
                Url = url,
                CreatedAt = DateTime.Now
            };

            // CSVログ
            new CsvService().Append(record, paths.OutputDir);

            // Supabase
            new SupabaseService().Insert(record).Wait();

            SafeLog.Info("処理完了: " + name);
        }
        catch (Exception ex)
        {
            SafeLog.Error("致命的エラー: " + ex);
        }
    }
}