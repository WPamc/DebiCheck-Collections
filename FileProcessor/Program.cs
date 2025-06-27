
using System;
using System.IO;
using System.Linq;
using FileHelpers;
using RMCollectionProcessor.Models;

namespace FileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;
            var exampleFilesPath = Path.Combine(solutionDirectory, "ExampleFiles");

            if (!Directory.Exists(exampleFilesPath))
            {
                Console.WriteLine($"Error: Directory not found at {exampleFilesPath}");
                return;
            }

            var files = Directory.GetFiles(exampleFilesPath);

            var engine = new MultiRecordEngine((FileHelpers.MultiRecordEngine.RecordSelector)RecordSelectorDelegate, 
                typeof(TransmissionHeader000),
                typeof(CollectionHeader080),
                typeof(CollectionTxLine01),
                typeof(CollectionTxLine02),
                typeof(CollectionTxLine03),
                typeof(CollectionTrailer080),
                typeof(StatusUserSetHeader080),
                typeof(StatusUserSetHeaderLine01),
                typeof(StatusUserSetHeaderLine02),
                typeof(StatusUserSetTransactionLine01),
                typeof(StatusUserSetTransactionLine02),
                typeof(StatusUserSetTransactionLine03),
                typeof(StatusUserSetTransactionLine04),
                typeof(StatusUserSetErrorRecord085),
                typeof(StatusUserSetTrailer084),
                typeof(ReplyTransmissionStatus900),
                typeof(ReplyUserSetStatus900),
                typeof(ReplyRejectedMessage901),
                typeof(ReplyTransmissionRejectReason901),
                typeof(TransmissionTrailer999)
            );

            foreach (var file in files)
            {
                try
                {
                    var parsedRecords = engine.ReadFile(file);

                    var fileType = FileTypeIdentifier.Identify(parsedRecords);
                    var subType = IdentifySubType(parsedRecords, fileType);

                    Console.WriteLine($"File: {Path.GetFileName(file)}");
                    Console.WriteLine($"  Type: {fileType}");
                    Console.WriteLine($"  Sub-Type: {subType}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {Path.GetFileName(file)}: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        private static Type RecordSelectorDelegate(string recordLine)
        {
            if (recordLine.Length < 3)
            {
                return null; // Not enough characters to determine record type
            }

            string recordIdentifier = recordLine.Substring(0, 3);

            switch (recordIdentifier)
            {
                case "000": return typeof(TransmissionHeader000);
                case "080": 
                    // Differentiate between CollectionHeader080 and StatusUserSetHeader080
                    // This is a simplified check, a more robust solution might need more context
                    if (recordLine.Length > 10 && recordLine.Substring(8, 2) == "04") // BankServRecordId for CollectionHeader080
                    {
                        return typeof(CollectionHeader080);
                    }
                    else
                    {
                        return typeof(StatusUserSetHeader080);
                    }
                case "081": return typeof(StatusUserSetHeaderLine01);
                case "082": return typeof(StatusUserSetTransactionLine01);
                case "084": return typeof(StatusUserSetTrailer084);
                case "085": return typeof(StatusUserSetErrorRecord085);
                case "900": 
                    // Differentiate between ReplyTransmissionStatus900 and ReplyUserSetStatus900
                    // This is a simplified check, a more robust solution might need more context
                    if (recordLine.Length > 4 && recordLine.Substring(3, 1) == "L") // Status for ReplyTransmissionStatus900
                    {
                        return typeof(ReplyTransmissionStatus900);
                    }
                    else
                    {
                        return typeof(ReplyUserSetStatus900);
                    }
                case "901": return typeof(ReplyRejectedMessage901);
                case "999": return typeof(TransmissionTrailer999);
                default: return null;
            }
        }

        public static string IdentifySubType(object[] parsedRecords, FileType fileType)
        {
            if (fileType != FileType.StatusReport)
            {
                return "Not Applicable";
            }

            if (parsedRecords == null || parsedRecords.Length == 0)
            {
                return "Unknown";
            }

            bool hasAccepted = false;
            bool hasRejected = false;

            foreach (var record in parsedRecords)
            {
                if (record is StatusUserSetTransactionLine01 statusRecord)
                {
                    if (statusRecord.TransactionStatus.Trim() == "ACCP")
                    {
                        hasAccepted = true;
                    }
                    else if (statusRecord.TransactionStatus.Trim() == "RJCT")
                    {
                        hasRejected = true;
                    }
                }
            }

            if (hasAccepted && hasRejected)
            {
                return "Collections Partially Accepted/Rejected";
            }
            if (hasAccepted)
            {
                return "Collections Accepted";
            }
            if (hasRejected)
            {
                return "Collections Rejected";
            }

            foreach (var record in parsedRecords)
            {
                 if (record is StatusUserSetHeaderLine01 headerLine)
                 {
                     switch (headerLine.GroupLevelStatus.Trim())
                     {
                         case "ACCP":
                         case "ACCR":
                             return "Collections Accepted";
                         case "RJCT":
                         case "RJCR":
                             return "Collections Rejected";
                         case "PART":
                         case "PECR":
                             return "Collections Partially Accepted/Rejected";
                         case "PDNG":
                             return "Collections Pending";
                     }
                 }
            }


            return "Unknown Status";
        }
    }
}
