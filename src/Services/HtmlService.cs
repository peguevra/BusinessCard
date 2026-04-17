using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuisinessCard.Services
{
    public class HtmlService
    {
        public void GenerateFromCsv(string outputDir)
        {
            var csvPath = Path.Combine(outputDir, "index.csv");
            var htmlPath = Path.Combine(outputDir, "index.html");

            if (!File.Exists(csvPath))
                return;

            var rows = new List<string[]>();

            foreach (var line in File.ReadAllLines(csvPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',', 3);
                if (parts.Length < 2) continue;

                rows.Add(parts);
            }

            var html = Build(rows);
            File.WriteAllText(htmlPath, html, Encoding.UTF8);
        }

        public string Build(List<string[]> rows)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<html><head>");
            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'>");

            sb.AppendLine("<style>");

            sb.AppendLine("body { font-family:sans-serif; background:#f5f5f5; padding:10px; margin:0; }");

            // =========================
            // ソートバー（旧UI寄せ）
            // =========================
            sb.AppendLine(".sort-bar { display:flex; gap:8px; margin-bottom:12px; }");

            sb.AppendLine(".sort-btn {");
            sb.AppendLine("  flex:1;");
            sb.AppendLine("  text-align:center;");
            sb.AppendLine("  padding:10px;");
            sb.AppendLine("  background:white;");
            sb.AppendLine("  border-radius:10px;");
            sb.AppendLine("  box-shadow:0 1px 3px rgba(0,0,0,0.12);");
            sb.AppendLine("  font-size:14px;");
            sb.AppendLine("  cursor:pointer;");
            sb.AppendLine("  user-select:none;");
            sb.AppendLine("}");

            sb.AppendLine(".sort-btn.active {");
            sb.AppendLine("  background:#222;");
            sb.AppendLine("  color:white;");
            sb.AppendLine("}");

            // =========================
            // カード
            // =========================
            sb.AppendLine(".card {");
            sb.AppendLine("  background:white;");
            sb.AppendLine("  border-radius:12px;");
            sb.AppendLine("  padding:12px;");
            sb.AppendLine("  margin-bottom:10px;");
            sb.AppendLine("  box-shadow:0 2px 6px rgba(0,0,0,0.1);");
            sb.AppendLine("}");

            sb.AppendLine(".card a { text-decoration:none; color:inherit; display:block; }");

            sb.AppendLine(".name { font-size:16px; font-weight:bold; }");
            sb.AppendLine(".date { font-size:12px; color:gray; margin-top:4px; }");

            sb.AppendLine("</style>");

            // =========================
            // JS（即時ソート）
            // =========================
            sb.AppendLine("<script>");
            sb.AppendLine("function sortCards(type){");
            sb.AppendLine("  const container = document.getElementById('list');");
            sb.AppendLine("  const cards = Array.from(container.children);");

            sb.AppendLine("  cards.sort((a,b)=>{");
            sb.AppendLine("    const an = a.getAttribute('data-name');");
            sb.AppendLine("    const bn = b.getAttribute('data-name');");
            sb.AppendLine("    const ad = a.getAttribute('data-date');");
            sb.AppendLine("    const bd = b.getAttribute('data-date');");

            sb.AppendLine("    if(type==='name'){ return an.localeCompare(bn); }");
            sb.AppendLine("    return bd.localeCompare(ad);");
            sb.AppendLine("  });");

            sb.AppendLine("  cards.forEach(c=>container.appendChild(c));");

            sb.AppendLine("  document.querySelectorAll('.sort-btn').forEach(b=>b.classList.remove('active'));");
            sb.AppendLine("  document.getElementById('btn-'+type).classList.add('active');");
            sb.AppendLine("}");
            sb.AppendLine("</script>");

            sb.AppendLine("</head><body>");

            // =========================
            // ソートUI（旧見た目）
            // =========================
            sb.AppendLine("<div class='sort-bar'>");
            sb.AppendLine("<div id='btn-name' class='sort-btn active' onclick=\"sortCards('name')\">Name</div>");
            sb.AppendLine("<div id='btn-date' class='sort-btn' onclick=\"sortCards('date')\">Date</div>");
            sb.AppendLine("</div>");

            // =========================
            // リスト
            // =========================
            sb.AppendLine("<div id='list'>");

            var cleanRows = rows
                .Where(r => r.Length >= 2)
                .Where(r => r[1] != "Name")
                .ToList();

            foreach (var r in cleanRows)
            {
                var date = r.Length > 0 ? r[0] : "";
                var name = r.Length > 1 ? r[1] : "";
                var url  = r.Length > 2 ? r[2] : "";

                sb.AppendLine($"<div class='card' data-name='{name}' data-date='{date}'>");

                if (!string.IsNullOrWhiteSpace(url))
                    sb.AppendLine($"<a href='{url}'>");

                sb.AppendLine($"<div class='name'>{name}</div>");
                sb.AppendLine($"<div class='date'>{date}</div>");

                if (!string.IsNullOrWhiteSpace(url))
                    sb.AppendLine("</a>");

                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");

            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
    }
}