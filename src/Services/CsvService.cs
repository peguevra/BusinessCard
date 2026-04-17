using System.Text;

public class CsvService
{
    private static readonly object _lock = new();

    public void Append(BuisicessCardRecord record, string outputDir)
    {
        var path = Path.Combine(outputDir, "index.csv");

        lock (_lock)
        {
            // =========================
            // 日付フォーマット修正
            // 旧: yyyy-MM-dd HH:mm:ss
            // 新: yyyy/M/d HH:mm（秒なし・軽量表示）
            // =========================
            var line =
                $"{record.CreatedAt:yyyy/M/d HH:mm}," +
                $"{Escape(record.Name)}," +
                $"{Escape(record.Url)}";

            File.AppendAllText(path, line + "\n", Encoding.UTF8);
        }
    }

    private string Escape(string v)
    {
        if (string.IsNullOrEmpty(v)) return "";

        // CSV崩れ防止（カンマ対策）
        if (v.Contains(",") || v.Contains("\""))
        {
            v = v.Replace("\"", "\"\"");
            return $"\"{v}\"";
        }

        return v;
    }
}