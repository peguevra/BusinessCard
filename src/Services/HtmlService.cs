using System;
using System.IO;
using System.Text;
using System.Linq;

public class HtmlService
{
    private string FilePath(string outputDir)
    {
        return Path.Combine(outputDir, "index.html");
    }

    // =========================
    // CSVから毎回HTMLを再生成
    // =========================
    public void GenerateFromCsv(string outputDir)
    {
        var csvPath = Path.Combine(outputDir, "index.csv");
        var htmlPath = FilePath(outputDir);

        if (!File.Exists(csvPath))
            return;

        var lines = File.ReadAllLines(csvPath);

        var sb = new StringBuilder();

        sb.Append(@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<title>名刺一覧</title>
</head>
<body>
<h2>名刺一覧</h2>

<table border='1'>
<tr><th>日時</th><th>名前</th><th>リンク</th></tr>
");

        foreach (var line in lines)
        {
            var cols = line.Split(',');

            if (cols.Length < 3)
                continue;

            var date = cols[0];
            var name = cols[1];
            var url = cols[2];

            sb.Append($@"
<tr>
<td>{date}</td>
<td>{name}</td>
<td><a href='{url}' target='_blank'>開く</a></td>
</tr>");
        }

        sb.Append(@"
</table>
</body>
</html>");

        File.WriteAllText(htmlPath, sb.ToString(), Encoding.UTF8);
    }
}