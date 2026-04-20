using System.Text;
using System.Text.Json;

public class JsonService
{
    public void Generate(string outputDir, string deployDir)
    {
        var csvPath = Path.Combine(outputDir, "index.csv");
        var jsonPath = Path.Combine(deployDir, "data.json");

        if (!File.Exists(csvPath))
            return;

        var list = new List<object>();

        foreach (var line in File.ReadAllLines(csvPath, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',', 4);
            if (parts.Length < 4) continue;

            list.Add(new
            {
                CreatedAt = parts[0],
                Category = parts[1],
                Name = parts[2],
                Url = parts[3]
            });
        }

        var json = JsonSerializer.Serialize(list, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }
}