using System;
using System.IO;
using System.Text;
using System.Threading;

public class CsvService
{
    public void Append(BuisicessCardRecord record, string outputDir)
    {
        var path = Path.Combine(outputDir, "index.csv");

        var line = $"{record.CreatedAt:yyyy-MM-dd HH:mm:ss},{record.Name},{record.Url}";

        WriteWithRetry(path, line + Environment.NewLine);
    }

    private void WriteWithRetry(string path, string content)
    {
        for (int i = 0; i < 10; i++)
        {
            try
            {
                File.AppendAllText(path, content, Encoding.UTF8);
                return;
            }
            catch
            {
                Thread.Sleep(200);
            }
        }

        SafeLog.Error("CSV書き込み失敗: " + path);
    }
}