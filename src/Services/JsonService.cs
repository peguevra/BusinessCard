using System.Text;
using System.Text.Json;

public class JsonService
{
    public void Generate(GlobalPaths paths)
    {
        var list = new List<object>();

        if (!File.Exists(paths.CsvPath))
            return;

        foreach (var line in File.ReadAllLines(paths.CsvPath, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',', 4);
            if (parts.Length < 4) continue;

            list.Add(new
            {
                Date = parts[0],
                Category = parts[1],
                Name = parts[2],
                Url = parts[3]
            });
        }

        var json = JsonSerializer.Serialize(list, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(paths.DeployJsonPath, json, Encoding.UTF8);
    }
}