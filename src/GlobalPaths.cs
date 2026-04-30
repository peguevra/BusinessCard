using System.IO;

public class GlobalPaths
{
    public string BaseDir => AppContext.BaseDirectory;

    // ===== 監視対象 =====
    public string InputDir => @"C:\Users\sr01\Desktop\SCAN\BuisinessCard";

    // ===== 処理済み =====
    public string DoneDir => @"C:\Users\sr01\Desktop\SCAN\done";

    // ===== ログ・CSV =====
    public string OutputDir => Path.Combine(BaseDir, "Output");

    public string CsvPath => Path.Combine(OutputDir, "index.csv");

    // ===== 初期化 =====
    public void EnsureDirectories()
    {
        Directory.CreateDirectory(InputDir);
        Directory.CreateDirectory(DoneDir);
        Directory.CreateDirectory(OutputDir);
    }
}