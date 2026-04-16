using CommonBatchFramework.App;
using CommonBatchFramework.App;

AppRunner.Run(() =>
{
    var paths = new GlobalPaths();
    paths.EnsureDirectories();

    Log.Initialize(paths.OutputDir);

    Log.Info("処理開始");

    var watcher = new WatcherService();
    var processor = new ProcessorService();

    watcher.Start(paths, processor);

    Log.Info("監視開始");

    // 常駐
    Thread.Sleep(Timeout.Infinite);
});