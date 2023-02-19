using System.Diagnostics;
using System.IO.Compression;

namespace Updater;

public static class Updater
{
    private const string Help = "Утилита для загрузки обновлений в виде zip архива и последующей распаковки.\n" +
                                "Использование: Updater.exe [1] [2] [3]\n" +
                                "[1] url адрес, откуда нужно скачать архив\n" +
                                "[2] путь, по которому нужно распаковать архив\n" +
                                "[3] Название exe файла, который нужно запустить после завершения обновления\n" +
                                "Автор: sadovnichek";
    
    private static async Task Main(string[] args)
    {
        try
        {
            if (!IsCorrectArguments(args)) 
                return;
            var url = args[0];
            var destination = args[1];
            var setupFile = args[2];
            var archiveName = url.Split("/").Last();
            var archivePath = Path.Combine(destination, archiveName);
            await HttpDownload(url, archivePath);
            UnpackZip(archivePath, destination);
            StartNewProcess(setupFile, destination);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }
    
    private static bool IsCorrectArguments(IReadOnlyCollection<string> args)
    {
        Console.WriteLine("Переданные аргументы:");
        args.ToList().ForEach(Console.WriteLine);
        if (args.Count == 3)
            return true;
        Console.WriteLine(Help);
        Console.ReadKey();
        return false;
    }
    
    private static void StartNewProcess(string filename, string workingDirectory)
    {
        var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = $@"{workingDirectory}\{filename}",
        };
        process.Start();
    }

    private static async Task HttpDownload(string url, string savePath)
    {
        try
        {
            var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
            await using var streamToWriteTo = File.Open(savePath, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Проверьте подключение к интернету");
            Console.ReadKey();
            throw;
        }
    }

    private static void UnpackZip(string zipPath, string destination)
    {
        Console.WriteLine("Распаковка архива...");
        var zipArchive = ZipFile.OpenRead(zipPath);
        foreach (var file in zipArchive.Entries
                     .Where(file => !file.Name.Contains("Updater")))
        {
            var fullPath = Path.Combine(destination, file.FullName);
            if (!File.Exists(fullPath))
            {
                CheckPath(file.FullName, destination);
            }
            file.ExtractToFile(fullPath, true);
        }
        Console.WriteLine("Архив успешно распакован");
    }

    private static void CheckPath(string relativePath, string destination)
    {
        var parts = relativePath.Split('/', '\\');
        parts = parts.Take(parts.Length - 1).ToArray();
        var currentPath = destination;
        foreach (var part in parts)
        {
            currentPath += $@"\{part}";
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
        }
    }
}