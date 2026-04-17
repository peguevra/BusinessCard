using System;
using System.IO;
using System.Threading;

public class WatcherService
{
    private FileSystemWatcher? _watcher;
    private readonly object _lock = new();

    public void Start(GlobalPaths paths)
    {
        var dir = paths.InputDir;

        SafeLog.Info("監視対象: " + dir);

        _watcher = new FileSystemWatcher(dir)
        {
            Filter = "*.pdf",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        _watcher.Created += (s, e) => OnEvent(e.FullPath, paths);

        _watcher.EnableRaisingEvents = true;

        SafeLog.Info("Watcher起動完了");
    }

    private void OnEvent(string path, GlobalPaths paths)
    {
        lock (_lock)
        {
            SafeLog.Info("イベント検知: " + path);

            WaitForFile(path);

            new ProcessorService().Execute(path, paths);
        }
    }

    private void WaitForFile(string path)
    {
        while (true)
        {
            try
            {
                using var s = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                break;
            }
            catch
            {
                Thread.Sleep(300);
            }
        }
    }
}