using CommonBatchFramework.App;

AppRunner.Run(() =>
{
    var paths = new GlobalPaths();
    paths.EnsureDirectories();

    Log.Initialize(paths.OutputDir);

    Log.Info("処理開始");

    var watcher = new WatcherService();
    watcher.Start(paths);

    Log.Info("監視開始");

    Console.ReadLine();
});