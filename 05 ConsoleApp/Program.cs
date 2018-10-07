// ConsoleApp UWP CS
// My first UWP Console App
// https://blogs.windows.com/buildingapps/2018/06/06/c-console-uwp-applications/
// https://blogs.windows.com/buildingapps/2018/05/18/console-uwp-applications-and-file-system-access/
// https://msdn.microsoft.com/en-us/magazine/mt846651
//
// Project Debug option: Do not launch, but wait for program to start, then launch it manually in a console
//
// 2018-10-07   PV


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace ConsoleApp_UWP_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Insufficient arguments.");
                Console.WriteLine("Usage:");
                Console.WriteLine("   mFindstr <search-pattern> <fully-qualified-folder-path>.");
                Console.WriteLine("Example:");
                Console.WriteLine("   mFindstr on D:\\Temp.");
            }
            else
            {
                string searchPattern = args[0];
                string folderPath = args[1];
                RecurseFolders(folderPath, searchPattern).Wait();
            }

            Console.WriteLine("Press a key to continue: ");
            Console.ReadLine();
        }


        private static async Task<bool> RecurseFolders(string folderPath, string searchPattern)
        {
            bool success = true;
            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

                if (folder != null)
                {
                    Console.WriteLine(
                        $"Searching folder '{folder}' and below for pattern '{searchPattern}'");
                    try
                    {
                        // Get the files in this folder. 
                        IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                        foreach (StorageFile file in files)
                        {
                            await SearchFile(file, searchPattern);
                        }

                        // Recurse sub-directories. 
                        IReadOnlyList<StorageFolder> subDirs = await folder.GetFoldersAsync();
                        if (subDirs.Count != 0)
                        {
                            await GetDirectories(subDirs, searchPattern);
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex.Message);
            }
            return success;
        }


        private static async Task GetDirectories(IReadOnlyList<StorageFolder> folders, string searchPattern)
        {
            try
            {
                foreach (StorageFolder folder in folders)
                {
                    // Get the files in this folder. 
                    IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                    foreach (StorageFile file in files)
                    {
                        await SearchFile(file, searchPattern);
                    }

                    // Recurse this folder to get sub-folder info. 
                    IReadOnlyList<StorageFolder> subDirs = await folder.GetFoldersAsync();
                    if (subDirs.Count != 0)
                    {
                        await GetDirectories(subDirs, searchPattern);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static async Task SearchFile(StorageFile file, string searchPattern)
        {
            if (file != null)
            {
                try
                {
                    Console.WriteLine($"Scanning file '{file.Path}'");
                    string text = await FileIO.ReadTextAsync(file);
                    string compositePattern =
                        "(\\S+\\s+){0}\\S*" + searchPattern + "\\S*(\\s+\\S+){0}";
                    Regex regex = new Regex(compositePattern);
                    MatchCollection matches = regex.Matches(text);
                    foreach (Match match in matches)
                    {
                        Console.WriteLine($"{match.Index,8} {match.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


    }
}
