using System;
using System.Collections.Generic;
using System.IO;

namespace RequestsCleaner
{
    class Program
    {

        static int daysOld = 7;
        static bool deleteEnabled = false;
        static bool verbose = false;

        static void Main(string[] args)
        {

            Console.WriteLine("RequestsCleaner v0.0 by Birb");

            string path = "";
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].TrimStart('-');
                switch (args[i])
                {
                    case "days":
                    case "day":
                        daysOld = Int32.Parse(args[i + 1]);
                        break;

                    case "enable":
                    case "delete":
                        deleteEnabled = true;
                        break;

                    case "verbose":
                        verbose = true;
                        break;

                    default:
                        path = args[i];
                        break;
                }
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"### {path} does not exist. Exiting");
                return;
            }

            Console.WriteLine($"### Looking for files older than {daysOld} days");

            Console.WriteLine($"### File deletion {(deleteEnabled ? "ENABLED" : "disabled")}!");

            Console.WriteLine($"### Searching {path}...");

            int deleted = 0;
            var r = EnumerateFiles(path);
            foreach (var item in r)
            {
                var dt = File.GetLastWriteTime(item);
                if (verbose)
                    Console.WriteLine($"{(DateTime.Now - dt).Days} days old | Deleting {Path.GetFileName(item)}");
                if (deleteEnabled)
                {
                    try
                    {
                        //File.Delete(item);
                        deleted++;
                    } catch
                    {
                        Console.WriteLine($"Unable to delete: {item}");
                    }
                }
            }
            Console.WriteLine($"Successfully deleted: {deleted}/{r.Count} files");
            
        }

        static List<string> EnumerateFiles(string path)
        {
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));
            List<string> files = new List<string>(Directory.EnumerateFiles(path));
            List<string> allFiles = new List<string>();

            foreach (var item in dirs)
            {
                foreach (var file in EnumerateFiles(item))
                {
                    var dt = File.GetLastWriteTime(file);
                    if (dt.AddDays(daysOld) < DateTime.Now)
                    {
                        allFiles.Add(file);
                    }
                }
            }

            foreach (var file in files)
            {
                var dt = File.GetLastWriteTime(file);
                if (dt.AddDays(daysOld) < DateTime.Now)
                    allFiles.Add(file);
            }


            Console.WriteLine($"[{allFiles.Count} files] {path}");

            return allFiles;
        }
    }
}
