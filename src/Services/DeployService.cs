using System;
using System.Diagnostics;

public class DeployService
{
    private readonly string gitSyncPath =
        @"C:\Users\sr01\Desktop\MACRO\C\deploy2";

    public void Deploy()
    {
        try
        {
            RunGit("git add data.json");
            RunGit("git commit -m \"auto update json\"");
            RunGit("git push origin main");

            SafeLog.Info("Deploy完了（JSON）");
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