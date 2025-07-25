
using System;
using System.Data;
using System.IO;
using System.Linq;
using DbConnection;
using Microsoft.Data.SqlClient;
using RMCollectionProcessor.Models;
using static System.Net.Mime.MediaTypeNames;

namespace FileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFolder = @"C:\AbsaArchive\tests\ZR07675";
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
                    var fileType = DCFileTypeIdentifier.Identify(parsedRecords);
                    var status = GetRecordStatus(parsedRecords);
                    var statusFolder = Path.Combine(sourceFolder, status == "L" ? "Live" : "Test");
                    Directory.CreateDirectory(statusFolder);
                    var destinationFolder = status == "L"
                        ? Path.Combine(statusFolder, fileType.ToString())
                        : statusFolder;
                    Directory.CreateDirectory(destinationFolder);
                    var generation = GetGenerationNumber(parsedRecords, fileType);
                    var fileName = Path.GetFileName(file);
                    if (!string.IsNullOrWhiteSpace(generation) &&
                        (fileType == DCFileType.StatusReport || fileType == DCFileType.Reply))
                    {
                        fileName = $"{generation}_{fileName}";
                    }
                    var destinationPath = Path.Combine(destinationFolder, fileName);
                    File.Copy(file, destinationPath, true);
                    Console.WriteLine($"{Path.GetFileName(file)}: {status} {fileType}");
                    if (fileType == DCFileType.Reply && int.TryParse(generation, out var genNum))
                    {
                        var related = GetFileNameByGenerationNumber(genNum);
                        if (!string.IsNullOrEmpty(related))
                        {
                            Console.WriteLine(related);
                        }
                    }
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

        /// <summary>
        /// Extracts the generation number from the parsed records for the specified file type.
        /// </summary>
        /// <param name="parsedRecords">The parsed records.</param>
        /// <param name="fileType">The detected file type.</param>
        /// <returns>The generation number or an empty string.</returns>
        private static string GetGenerationNumber(object[] parsedRecords, DCFileType fileType)
        {
            if (fileType == DCFileType.StatusReport)
            {
                var header = parsedRecords.OfType<StatusUserSetHeader080>().FirstOrDefault();
                return header?.BankServUserCodeGenerationNumber.Trim() ?? string.Empty;
            }

            if (fileType == DCFileType.Reply)
            {
                var reply = parsedRecords.OfType<ReplyTransmissionStatus900>().FirstOrDefault();
                return reply?.TransmissionNumber.Trim() ?? string.Empty;
            }

            return string.Empty;
        }

        private static string GetFileNameByGenerationNumber(int generationNumber)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("SELECT FILENAME FROM dbo.EDI_BANKFILES WHERE GENERATIONNUMBER = @gen", conn);
            cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.Int) { Value = generationNumber });
            conn.Open();
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }


    }
}
