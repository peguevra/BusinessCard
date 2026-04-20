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
                $"{record.CreatedAt:yyyy/M/d HH:mm}," +
                $"{Escape(record.Category)}," +
                $"{Escape(record.Name)}," +
                $"{Escape(record.Url)}";

            File.AppendAllText(path, line + "\n", Encoding.UTF8);
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