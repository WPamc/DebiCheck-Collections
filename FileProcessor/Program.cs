
using System;
using System.IO;
using RMCollectionProcessor.Models;

namespace FileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFolder = args.Length > 0 ? args[0] : null;
            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                Console.WriteLine("Enter the folder path:");
                sourceFolder = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                Console.WriteLine($"Error: Directory not found at {sourceFolder}");
                return;
            }

            var processor = new RMCollectionProcessor.FileProcessor();
            var files = Directory.GetFiles(sourceFolder);

            foreach (var file in files)
            {
                try
                {
                    var parsedRecords = processor.ProcessFile(file);
                    var fileType = FileTypeIdentifier.Identify(parsedRecords);
                    Console.WriteLine($"{Path.GetFileName(file)}: {fileType}");

                    var destinationFolder = Path.Combine(sourceFolder, fileType.ToString());
                    Directory.CreateDirectory(destinationFolder);
                    var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(file));
                    File.Copy(file, destinationPath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }


    }
}
