using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace Updater;

public static class Updater
{
    private const string Help = "Утилита для загрузки обновлений в виде zip архива и последующей распаковки.\n" +
                                "Использование: Updater.exe [1] [2] [3]\n" +
                                "[1] url адрес, откуда нужно скачать архив\n" +
                                "[2] путь, по которому нужно распаковать архив\n" +
                                "[3] Название exe файла, который нужно запустить после завершения обновления\n" +
                                "Автор: sadovnichek";

    private static readonly HttpClient client = new();

    private static void Main(string[] args)
    {
        try
        {
            var arguments = ParseArguments(args);
            if (arguments.Length == 3)
            {
                var url = args[0];
                var destination = args[1];
                var setupFile = args[2];
                var archiveName = url.Split("/").Last();
                var archivePath = Path.Combine(destination, archiveName);
                Download(url, archivePath);
                UnpackZip(archivePath, destination);
                StartNewProcess(setupFile, destination);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }

    private static string[] ParseArguments(string[] args)
    {
        Console.WriteLine("Переданные аргументы:");
        args.ToList().ForEach(Console.WriteLine);
        if (args.Length == 3)
            return args;
        Console.WriteLine(Help);
        Console.ReadKey();
        return Array.Empty<string>();
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

    private static async void HttpDownload(string url, string savePath)
    {
        using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
        using Stream streamToWriteTo = File.Open(savePath, FileMode.Create);
        await streamToReadFrom.CopyToAsync(streamToWriteTo);
    }

    private static void Download(string url, string savePath)
    {
        try
        {
            Console.WriteLine("Загрузка архива...");
            var client = new WebClient();
            client.DownloadFile(url, savePath);
            Console.WriteLine("Архив успешно загружен");
        }
        catch (WebException)
        {
            Console.WriteLine("Проверьте подключение к интернету");
            Console.ReadKey();
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