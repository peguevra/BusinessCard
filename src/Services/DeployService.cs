using System;
using System.Diagnostics;
using System.IO;

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
            // =========================
            // 1. ファイルコピー
            // =========================
            var destPath = Path.Combine(gitSyncPath, "index.html");

            File.Copy(sourceHtmlPath, destPath, true);

            // =========================
            // 2. Git実行
            // =========================
            RunGit("git add index.html");
            RunGit("git commit -m \"auto deploy index.html\"");
            RunGit("git push origin deploy");
        }
        catch (Exception ex)
        {
            SafeLog.Error("Deploy失敗: " + ex);
        }
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

        var p = Process.Start(psi);
        p.WaitForExit();

        var output = p.StandardOutput.ReadToEnd();
        var error = p.StandardError.ReadToEnd();

        if (!string.IsNullOrWhiteSpace(error))
        {
            SafeLog.Error("Git error: " + error);
        }
    }
}