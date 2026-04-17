using System.Text;

public class CsvService
{
    private static readonly object _lock = new();

    public void Append(BuisicessCardRecord record, string outputDir)
    {
        var path = Path.Combine(outputDir, "index.csv");

        lock (_lock)
        {
            var line =
                $"{record.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                $"{Escape(record.Name)}," +
                $"{Escape(record.Url)}";

            File.AppendAllText(path, line + "\n", Encoding.UTF8);
        }
    }

    private string Escape(string v)
    {
        if (string.IsNullOrEmpty(v)) return "";
        return v.Contains(",") ? $"\"{v}\"" : v;
    }
}