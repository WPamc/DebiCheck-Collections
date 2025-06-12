using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RMCollectionProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- RM Collection File Processor ---");
            Console.WriteLine("1) Parse existing file");
            Console.WriteLine("2) Generate sample file");
            Console.Write("Select option: ");
            var key = Console.ReadLine();

            if (key == "1")
            {
                ParseFile(args);
            }
            else if (key == "2")
            {
                GenerateFile();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        private static void ParseFile(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "RM-Collections.txt";
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(args.Length > 0
                    ? $"Error: File '{filePath}' not found."
                    : $"Error: Default file '{filePath}' not found.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"Processing file: {filePath}\n");
            try
            {
                var fileProcessor = new FileProcessor();
                var collectionFile = fileProcessor.ProcessFile(filePath);
                Console.WriteLine($"Parsed {collectionFile.Length} transactions.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void GenerateFile()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbService = new DatabaseService(configuration);

            var collections = dbService.GetCollectionsAsync().GetAwaiter().GetResult();
            if (!collections.Any())
            {
                Console.WriteLine("No collections to process.");
                return;
            }

            var creditorDefaults = dbService.GetCreditorDefaultsAsync(1).GetAwaiter().GetResult()
                                   ?? new CreditorDefaults();

            var staticData = new StaticDataProvider(
                recordStatus: "L",
                transmissionNumber: "0000878",
                userGenerationNumber: "0878",
                paymentInfoId: "2878/2025-06-01",
                creditorDefaults: creditorDefaults);

            var records = new List<object>();
            var recordBuilder = new RecordBuilder();

            records.Add(recordBuilder.BuildTransmissionHeader(staticData));
            records.Add(recordBuilder.BuildCollectionHeader(staticData, 1));

            int sequenceNumber = 1;
            foreach (var debtorData in collections)
            {
                var (line1, line2, line3) = recordBuilder.BuildTransactionLines(staticData, debtorData, sequenceNumber);
                records.Add(line1);
                records.Add(line2);
                records.Add(line3);
                sequenceNumber++;
            }

            int firstSeq = 1;
            int lastSeq = collections.Count;
            int totalLines = 2 + (collections.Count * 3) + 2;
            records.Add(recordBuilder.BuildCollectionTrailer(staticData, collections, firstSeq, lastSeq));
            records.Add(recordBuilder.BuildTransmissionTrailer(staticData, totalLines));

            var engine = new MultiRecordEngine(
                typeof(TransmissionHeader000),
                typeof(CollectionHeader080),
                typeof(CollectionTxLine01),
                typeof(CollectionTxLine02),
                typeof(CollectionTxLine03),
                typeof(CollectionTrailer080),
                typeof(TransmissionTrailer999));

            engine.WriteFile("RM-Collections-Generated.txt", records);
            Console.WriteLine("RM Collection file generated successfully.");
        }

        private static List<DebtorCollectionData> GetSampleCollections()
        {
            return new List<DebtorCollectionData>
            {
                new DebtorCollectionData {
                    PaymentInformation = "677936/777573/2025-06-01/39",
                    RequestedCollectionDate = new DateTime(2025, 6, 1, 12, 0, 0),
                    TrackingPeriod = 0,
                    DebitSequence = "RCUR",
                    EntryClass = "0021",
                    InstructedAmount = 1501.00m,
                    MandateReference = "00032022022408817535",
                    DebtorBankBranch = "677936",
                    DebtorName = "KAGISO BOWANE DN",
                    DebtorAccountNumber = "62052597443",
                    AccountType = "CURRENT",
                    ContractReference = "78250655",
                    RelatedCycleDate = new DateTime(2025, 6, 1)
                }
            };
        }
    }

    #region File Processor
    public class FileProcessor
    {
        public object[] ProcessFile(string filePath)
        {
            var engine = new MultiRecordEngine(
                typeof(TransmissionHeader000),
                typeof(CollectionHeader080),
                typeof(CollectionTxLine01),
                typeof(CollectionTxLine02),
                typeof(CollectionTxLine03),
                typeof(CollectionTrailer080),
                typeof(TransmissionTrailer999))
            {
                RecordSelector = new RecordTypeSelector(CustomRecordSelector)
            };

            var parsedRecords = engine.ReadFile(filePath);

            if (engine.ErrorManager.HasErrors)
            {
                var errorDetails = new StringBuilder("FileHelpers parsing error:\n");
                foreach (var err in engine.ErrorManager.Errors)
                {
                    errorDetails.AppendLine($"Line {err.LineNumber}: {err.ExceptionInfo.Message}");
                }
                throw new InvalidDataException(errorDetails.ToString());
            }

            return parsedRecords;
        }

        private Type CustomRecordSelector(MultiRecordEngine engine, string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine) || recordLine.Length < 3)
                return null;

            string recordId = recordLine.Substring(0, 3);

            if (recordId == "000") return typeof(TransmissionHeader000);
            if (recordId == "999") return typeof(TransmissionTrailer999);

            if (recordId == "080")
            {
                string bankservId = recordLine.Substring(4, 2);
                if (bankservId == "04") return typeof(CollectionHeader080);
                if (bankservId == "92") return typeof(CollectionTrailer080);

                if (bankservId == "08")
                {
                    string lineCount = recordLine.Substring(16, 2);
                    if (lineCount == "01") return typeof(CollectionTxLine01);
                    if (lineCount == "02") return typeof(CollectionTxLine02);
                    if (lineCount == "03") return typeof(CollectionTxLine03);
                }
            }

            return null; // Unknown record type
        }

        
    }
    #endregion
}
