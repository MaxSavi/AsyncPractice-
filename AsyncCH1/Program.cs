using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        DownloadManager downloadManager = new DownloadManager();
        string[] urls = { "https://prezentacii.org/upload/cloud/20/05/393/d6bc809da44cf66b29d0ac02dd65acb8.ppt", "https://prezentacii.org/upload/cloud/20/05/430/19bcdce1879e570ed70b16da49f50a2b.pptx " };
        await downloadManager.DownloadFilesAsync(urls);

        ArrayProcessor arrayProcessor = new ArrayProcessor();
        int[] array = { 1, 2, 3, 4, 5 };
        arrayProcessor.IncrementArrayElements(array);
        Console.WriteLine(string.Join(", ", array));

        ConcurrentBag<int> data = new ConcurrentBag<int>(array);
        DataProcessor dataProcessor = new DataProcessor();
        dataProcessor.ProcessData(data);
    }
}
public class DownloadManager
{
    public async Task DownloadFilesAsync(string[] urls)
    {
        var tasks = new Task[urls.Length];
        for (int i = 0; i < urls.Length; i++)
        {
            tasks[i] = DownloadFileAsync(urls[i]);
        }
        await Task.WhenAll(tasks);
    }

    private async Task DownloadFileAsync(string url)
    {
        using (var client = new WebClient())
        {
            try
            {
                string fileName = Path.GetFileName(url);
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("Invalid URL: " + url);

                string uniqueFileName = GetUniqueFileName(fileName);

                // Загрузка файла
                await client.DownloadFileTaskAsync(new Uri(url), uniqueFileName); 

                Console.WriteLine($"File downloaded successfully: {uniqueFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file from {url}: {ex.Message}");
            }
        }
    }

    // Генерация уникального имени файла
    private string GetUniqueFileName(string fileName)
    {
        string directory = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(directory, fileName);

        int count = 1;
        while (File.Exists(filePath))
        {
            string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + count + Path.GetExtension(fileName);
            filePath = Path.Combine(directory, newFileName);
            count++;
        }

        return filePath;
    }
}
public class ArrayProcessor
{
    public void IncrementArrayElements(int[] array)
    {
        Parallel.For(0, array.Length, i =>
        {
            array[i]++;
        });
    }
}
public class DataProcessor
{
    public void ProcessData(ConcurrentBag<int> data)
    {
        Parallel.ForEach(data, item =>
        {
            Console.WriteLine($"Processed item: {item}");
        });
    }
}