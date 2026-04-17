using System;
using System.IO;
using System.Linq;

public class HtmlService
{
    public void GenerateFromCsv(GlobalPaths paths)
    {
        var csvPath = paths.CsvPath;
        var htmlPath = paths.HtmlPath;

        if (!File.Exists(csvPath))
            return;

        var lines = File.ReadAllLines(csvPath)
                         .Skip(1)
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .ToList();

        var html = CreateHeader();

        foreach (var line in lines)
        {
            var cols = line.Split(',');

            if (cols.Length < 3) continue;

            html +=
                $"<tr>" +
                $"<td>{cols[0]}</td>" +
                $"<td>{cols[1]}</td>" +
                $"<td><a href='{cols[2]}'>開く</a></td>" +
                $"</tr>\n";
        }

        html += "</tbody></table></body></html>";

        File.WriteAllText(htmlPath, html);
    }

    private string CreateHeader()
    {
        return @"<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<title>名刺一覧</title>

<style>
body { font-family: -apple-system; margin: 10px; }
table { width: 100%; border-collapse: collapse; }
th, td { border-bottom: 1px solid #ccc; padding: 8px; }
th { background: #f5f5f5; }
a { color: blue; }
</style>

</head>
<body>

<h2>名刺一覧</h2>

<table>
<thead>
<tr>
<th>日付</th>
<th>名前</th>
<th>リンク</th>
</tr>
</thead>
<tbody>";
    }
}