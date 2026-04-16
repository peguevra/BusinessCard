using CommonBatchFramework.App;

class Program
{
    [STAThread]
    static void Main()
    {
        AppRunner.Run(() =>
        {
            var paths = new GlobalPaths();
            paths.EnsureDirectories();

            Log.Initialize(paths.OutputDir);

            SafeLog.Info("監視開始");

            var watcher = new WatcherService();
            watcher.Start(paths);

            Thread.Sleep(Timeout.Infinite);
        });
    }
}