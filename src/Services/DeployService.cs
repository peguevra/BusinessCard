using System.Diagnostics;

public class DeployService
{
    private readonly string repoPath =
        @"C:\Users\sr01\Desktop\MACRO\C\deploy2";

    public void DeployJson()
    {
        RunGit("git add data.json");
        RunGit("git commit -m \"auto update json\"");
        RunGit("git push origin main");
    }

    private void RunGit(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c " + command,
            WorkingDirectory = repoPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var p = Process.Start(psi);

        if (p == null) return;

        string output = p.StandardOutput.ReadToEnd();
        string error = p.StandardError.ReadToEnd();

        p.WaitForExit();

        if (p.ExitCode != 0)
        {
            SafeLog.Error("Git失敗: " + error);
        }
        else if (!string.IsNullOrWhiteSpace(output))
        {
            SafeLog.Info(output);
        }
    }
}