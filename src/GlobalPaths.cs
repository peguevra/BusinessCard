public class GlobalPaths
{
    public string BaseDir => AppContext.BaseDirectory;

    public string InputDir => @"C:\Users\sr01\Desktop\SCAN\BuisinessCard";
    public string DoneDir  => @"C:\Users\sr01\Desktop\SCAN\done";

    public string OutputDir => Path.Combine(BaseDir, "Output");

    // 旧（残すが使わない）
    public string CsvPath => Path.Combine(OutputDir, "index.csv");
    public string HtmlPath => Path.Combine(OutputDir, "index.html");

    // ★追加：deploy2
    public string DeployDir => @"C:\Users\sr01\Desktop\MACRO\C\deploy2";

    public void EnsureDirectories()
    {
        Directory.CreateDirectory(InputDir);
        Directory.CreateDirectory(DoneDir);
        Directory.CreateDirectory(OutputDir);
    }
}