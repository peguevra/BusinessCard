using System.Text;

public class CsvService
{
    public void Append(string path, BuisicessCardRecord record)
    {
        bool exists = File.Exists(path);

        using var sw = new StreamWriter(path, true, Encoding.UTF8);

        if (!exists)
        {
            sw.WriteLine("No,Name,Company,Url,Date,Note");
        }

        string line = $"{record.No},{Escape(record.Name)},{Escape(record.Company)},{record.Url},{record.Date:yyyy-MM-dd HH:mm:ss},{Escape(record.Note)}";
        sw.WriteLine(line);
    }

    private string Escape(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        if (value.Contains(",") || value.Contains("\""))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}