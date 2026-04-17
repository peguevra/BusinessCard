using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

public class DeployService
{
    private readonly string gitSyncPath =
        @"C:\Users\sr01\Desktop\MACRO\C\BuisinessCard\GitSync";

    private readonly string sourceHtmlPath =
        @"C:\Users\sr01\Desktop\MACRO\C\BuisinessCard\bin\Debug\net8.0-windows\Output\index.html";

    public void Deploy()
    {
        try
        {
            var destPath = Path.Combine(gitSyncPath, "index.html");

            // =========================
            // 1. HTMLコピー
            // =========================
            var html = File.ReadAllText(sourceHtmlPath);

            // =========================
            // 2. バージョン付与
            // =========================
            var version = DateTime.Now.ToString("yyyyMMddHHmmss");

            // すでにindex.htmlを開いている場合のキャッシュ対策
            html = AddCacheBuster(html, version);

            File.WriteAllText(destPath, html);

            // =========================
            // 3. Git実行
            // =========================
            RunGit("git add index.html");
            RunGit("git commit -m \"auto deploy index.html\"");
            RunGit("git push origin deploy");

            SafeLog.Info("Deploy完了 v=" + version);
        }
        catch (Exception ex)
        {
            SafeLog.Error("Deploy失敗: " + ex);
        }
    }

    /// <summary>
    /// HTML内の参照URLにバージョンを付与
    /// </summary>
    private string AddCacheBuster(string html, string version)
    {
        // index.html 自体をクエリ付きで扱うための仕込み
        // （後述のJS or metaで利用可能）

        var tag = $"?v={version}";

        // もしCSS/JSがあればそこにも拡張可能
        html = Regex.Replace(html, @"index\.html(\?v=\d+)?", $"index.html{tag}");

        // bodyに埋め込む（任意だがデバッグに便利）
        html = html.Replace(
            "<body>",
            $"<body data-version=\"{version}\">"
        );

        return html;
    }

    private void RunGit(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c " + command,
            WorkingDirectory = gitSyncPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = Process.Start(psi);

        if (p == null)
        {
            SafeLog.Error("Gitプロセス起動失敗");
            return;
        }

        string output = p.StandardOutput.ReadToEnd();
        string error = p.StandardError.ReadToEnd();

        p.WaitForExit();

        if (p.ExitCode != 0)
        {
            SafeLog.Error($"Git失敗: {error}");
            return;
        }

        if (!string.IsNullOrWhiteSpace(output))
        {
            SafeLog.Info(output);
        }
    }
}