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

        using var p = Process.Start(psi);

        if (p == null)
        {
            SafeLog.Error("Gitプロセス起動失敗: " + command);
            return;
        }

        string output = p.StandardOutput.ReadToEnd();
        string error = p.StandardError.ReadToEnd();

        p.WaitForExit();

        // =========================
        // 成功判定は ExitCode
        // =========================
        if (p.ExitCode != 0)
        {
            SafeLog.Error($"Git失敗 (code={p.ExitCode}) : {error}");
            return;
        }

        // =========================
        // stderrは警告レベル扱い
        // =========================
        if (!string.IsNullOrWhiteSpace(error))
        {
            SafeLog.Info("Git警告/出力: " + error);
        }

        if (!string.IsNullOrWhiteSpace(output))
        {
            SafeLog.Info("Git出力: " + output);
        }
    }
}