using System.Collections.Concurrent;

public class WatcherService
{
    private readonly BlockingCollection<string> _queue = new();
    private readonly HashSet<string> _seen = new();

    private FileSystemWatcher? _watcher;

    public void Start(GlobalPaths paths)
    {
        SafeLog.Info($"監視対象: {paths.InputDir}");

        _watcher = new FileSystemWatcher(paths.InputDir);

        _watcher.IncludeSubdirectories = false;
        _watcher.Filter = "*.*";
        _watcher.NotifyFilter =
            NotifyFilters.FileName |
            NotifyFilters.CreationTime |
            NotifyFilters.LastWrite;

        _watcher.Created += (s, e) => OnEvent("Created", e);
        _watcher.Changed += (s, e) => OnEvent("Changed", e);
        _watcher.Deleted += (s, e) => OnEvent("Deleted", e);

        _watcher.EnableRaisingEvents = true;

        SafeLog.Info("Watcher起動完了");

        Task.Run(() => ProcessQueue(paths));
    }

    private void OnEvent(string type, FileSystemEventArgs e)
    {
        SafeLog.Info($"イベント: {type} / {e.FullPath}");

        lock (_seen)
        {
            if (_seen.Contains(e.FullPath))
            {
                SafeLog.Info("スキップ（重複）");
                return;
            }

            _seen.Add(e.FullPath);
        }

        _queue.Add(e.FullPath);
    }

    private void ProcessQueue(GlobalPaths paths)
    {
        foreach (var file in _queue.GetConsumingEnumerable())
        {
            SafeLog.Info($"キュー処理開始: {file}");

            try
            {
                var processor = new ProcessorService();
                processor.Process(file, paths);
            }
            catch (Exception ex)
            {
                SafeLog.Error(ex.ToString());
            }
        }
    }
}