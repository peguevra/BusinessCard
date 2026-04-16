using System.IO;

public class WatcherService
{
    private FileSystemWatcher _watcher;

    public void Start(GlobalPaths paths, ProcessorService processor)
    {
        _watcher = new FileSystemWatcher(paths.InputDir);

        // PDF生成直後のイベント
        _watcher.Created += (s, e) =>
        {
            SafeLog.Info($"イベント検知: {e.FullPath}");

            try
            {
                processor.Execute(e.FullPath, paths);
            }
            catch (Exception ex)
            {
                SafeLog.Error(ex.ToString());
            }
        };

        // 変更イベント（スキャナ等の追従用）
        _watcher.Changed += (s, e) =>
        {
            SafeLog.Info($"変更検知: {e.FullPath}");
        };

        // 削除イベント（後片付け確認用）
        _watcher.Deleted += (s, e) =>
        {
            SafeLog.Info($"削除検知: {e.FullPath}");
        };

        _watcher.IncludeSubdirectories = false;
        _watcher.EnableRaisingEvents = true;

        SafeLog.Info($"監視対象: {paths.InputDir}");
        SafeLog.Info("Watcher起動完了");
    }
}