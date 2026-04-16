using System.Text;

public class HtmlService
{
    public void Generate(GlobalPaths paths)
    {
        if (!File.Exists(paths.CsvPath)) return;

        var lines = File.ReadAllLines(paths.CsvPath);

        var sb = new StringBuilder();

        sb.AppendLine("<html><head><meta charset='utf-8'><title>名刺一覧</title></head><body>");
        sb.AppendLine("<h1>名刺一覧</h1>");
        sb.AppendLine("<table border='1'>");

        foreach (var line in lines.Skip(1))
        {
            var cols = line.Split(',');

            if (cols.Length < 5) continue;

            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{cols[0]}</td>");
            sb.AppendLine($"<td>{cols[1]}</td>");
            sb.AppendLine($"<td><a href='{cols[3]}'>開く</a></td>");
            sb.AppendLine($"<td>{cols[4]}</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</table></body></html>");

        File.WriteAllText(paths.HtmlPath, sb.ToString(), Encoding.UTF8);
    }
}