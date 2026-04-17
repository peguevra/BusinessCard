using CommonBatchFramework.App;

public static class SafeLog
{
    private static readonly object _lock = new object();

    public static void Info(string msg)
    {
        lock (_lock) Log.Info(msg);
    }

    public static void Error(string msg)
    {
        lock (_lock) Log.Error(msg);
    }
}