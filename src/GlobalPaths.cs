public class GlobalPaths
{
    public string BaseDir => AppContext.BaseDirectory;

    public string InputDir => @"C:\Users\sr01\Desktop\SCAN\BuisinessCard";
    public string DoneDir  => @"C:\Users\sr01\Desktop\SCAN\done";

    public string OutputDir => Path.Combine(BaseDir, "Output");

    public string CsvPath => Path.Combine(OutputDir, "index.csv");

    // ★ deploy2 のローカルcloneパス
    public string Deploy2Dir => @"C:\Users\sr01\Desktop\MACRO\C\deploy2";

    public string DeployJsonPath => Path.Combine(Deploy2Dir, "data.json");

    public void EnsureDirectories()
    {
        Directory.CreateDirectory(InputDir);
        Directory.CreateDirectory(DoneDir);
        Directory.CreateDirectory(OutputDir);
        Directory.CreateDirectory(Deploy2Dir);
    }
}