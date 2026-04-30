using System.Text;

public class CsvService
{
    private static readonly object _lock = new();

    // 既存（ログ用）
    public void Append(BuisicessCardRecord record, string outputDir)
    {
        var path = Path.Combine(outputDir, "index.csv");

        lock (_lock)
        {
            var line =
                $"{record.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                $"{Escape(record.Category)}," +
                $"{Escape(record.Name)}," +
                $"{Escape(record.Url)}";

            File.AppendAllText(path, line + "\n", Encoding.UTF8);
        }
    }

    // =========================
    // ★ 追加：完全再生成
    // =========================
    public void Rewrite(List<BuisicessCardRecord> list, string outputDir)
    {
        var path = Path.Combine(outputDir, "index.csv");

        lock (_lock)
        {
            var lines = new List<string>();

            // ヘッダ
            lines.Add("Date,Category,Name,Url");

            foreach (var r in list.OrderByDescending(x => x.CreatedAt))
            {
                var line =
                    $"{r.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                    $"{Escape(r.Category)}," +
                    $"{Escape(r.Name)}," +
                    $"{Escape(r.Url)}";

                lines.Add(line);
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }

    private string Escape(string v)
    {
        if (string.IsNullOrEmpty(v)) return "";

        if (v.Contains(",") || v.Contains("\""))
        {
            v = v.Replace("\"", "\"\"");
            return $"\"{v}\"";
        }

        return v;
    }
}