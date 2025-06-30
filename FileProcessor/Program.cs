
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
                    var status = GetRecordStatus(parsedRecords);
                    var statusFolder = Path.Combine(sourceFolder, status == "L" ? "Live" : "Test");
                    Directory.CreateDirectory(statusFolder);
                    var destinationFolder = status == "L"
                        ? Path.Combine(statusFolder, fileType.ToString())
                        : statusFolder;
                    Directory.CreateDirectory(destinationFolder);
                    var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(file));
                    File.Copy(file, destinationPath, true);
                    Console.WriteLine($"{Path.GetFileName(file)}: {status} {fileType}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Retrieves the record status indicator from the parsed records.
        /// </summary>
        /// <param name="parsedRecords">An array of parsed record objects.</param>
        /// <returns>"L" for live, "T" for test, or an empty string.</returns>
        private static string GetRecordStatus(object[] parsedRecords)
        {
            if (parsedRecords.Length > 0 && parsedRecords[0] is TransmissionHeader000 th)
            {
                return th.RecordStatus?.Trim().ToUpperInvariant() ?? string.Empty;
            }

            return string.Empty;
        }


    }
}
