using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace Updater;

public static class Updater
{
    private const string Help = "Утилита для загрузки обновлений в виде zip архива и последующей распаковки.\n" +
                                "Использование: Updater.exe [1] [2] [3]\n" +
                                "[1] url адрес, откуда нужно скачать архив\n" +
                                "[2] путь, по которому нужно сохранить файл\n" +
                                "[3] Название exe файла, который нужно запустить после завершения обновления\n" +
                                "Автор: sadovnichek";

    private static void CheckPath(string fullPath)
    {
        var parts = fullPath.Split("/");
        parts = parts.Take(parts.Length - 1).ToArray();
        var currentPath = "";
        foreach (var part in parts)
        {
            currentPath += $"/{part}";
            if (!Directory.Exists($"../{currentPath}"))
            {
                Directory.CreateDirectory($"../{currentPath}");
            }
        }
    }

    private static void Download(string url, string fullPath)
    {
        try
        {
            Console.WriteLine("Загрузка архива...");
            var client = new WebClient();
            client.DownloadFile(url, fullPath);
            Console.WriteLine("Архив успешно загружен");
        }
        catch (WebException)
        {
            Console.WriteLine("Проверьте подключение к интернету");
            Console.ReadKey();
        }
    }

    private static void UnpackZip(string fullPath)
    {
        Console.WriteLine("Распаковка архива...");
        try
        {
            var zipArchive = ZipFile.OpenRead(fullPath);
            Console.WriteLine("Архив успешно распакован");
            foreach (var file in zipArchive.Entries
                         .Where(file => file.Name.Length > 0 && !file.Name.Contains("Updater")))
            {
                CheckPath(file.FullName);
                file.ExtractToFile($"../{file.FullName}", true);
            }

            Console.WriteLine("Обновление завершено");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadKey();
            throw;
        }
    }

    private static void Update(string url, string pathToSave)
    {
        var fullPath = pathToSave + url.Split("/").Last();
        Download(url, fullPath);
        UnpackZip(fullPath);
    }

    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine(Help);
            Console.ReadKey();
        }
        else
        {
            Update(args[0], args[1]);
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = args[2]
            };
            process.Start();
            Environment.Exit(0);
        }
    }
}