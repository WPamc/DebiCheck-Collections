using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RMCollectionProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- RM Collection File Processor ---");

            // Step 1: Determine the input file path. If a command line argument
            // is supplied, use it; otherwise fall back to the default file name.
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
                // Step 2: Process and parse the file into a structured object.
                var fileProcessor = new FileProcessor();
                RMCollectionFile collectionFile = fileProcessor.ProcessFile(filePath);

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
              
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
        }

      
    }

    #region File Processor
    public class FileProcessor
    {
        public RMCollectionFile ProcessFile(string filePath)
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

            return StitchRecords(parsedRecords);
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

        private RMCollectionFile StitchRecords(object[] records)
        {
            var collectionFile = new RMCollectionFile();
            var currentTransaction = new RMCollectionTransaction();

            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];

                if (record is TransmissionHeader000 th) collectionFile.TransmissionHeader = th;
                else if (record is CollectionHeader080 ch) collectionFile.CollectionHeader = ch;
                else if (record is CollectionTrailer080 ct) collectionFile.CollectionTrailer = ct;
                else if (record is TransmissionTrailer999 tt) collectionFile.TransmissionTrailer = tt;
                else if (record is CollectionTxLine01 l1)
                {
                    // Start of a new transaction
                    currentTransaction = new RMCollectionTransaction
                    {
                        L1_DataSetStatus = l1.DataSetStatus,
                        L1_BankServUserCode = l1.BankServUserCode,
                        L1_RecordSequenceNumber = l1.RecordSequenceNumber,
                        L1_InitiatingParty = l1.InitiatingParty,
                        L1_PaymentInformation = l1.PaymentInformation,
                        L1_RequestedCollectionDate = l1.RequestedCollectionDate,
                        L1_CreditorName = l1.CreditorName,
                        L1_CreditorContactDetails = l1.CreditorContactDetails,
                        L1_CreditorAbbreviatedShortName = l1.CreditorAbbreviatedShortName
                    };
                }
                else if (record is CollectionTxLine02 l2)
                {
                    currentTransaction.L2_CreditorEmail = l2.CreditorEmail;
                    currentTransaction.L2_CreditorAccountNumber = l2.CreditorAccountNumber;
                    currentTransaction.L2_CreditorBankBranch = l2.CreditorBankBranch;
                    currentTransaction.L2_TrackingPeriod = l2.TrackingPeriod;
                    currentTransaction.L2_DebitSequence = l2.DebitSequence;
                    currentTransaction.L2_EntryClass = l2.EntryClass;
                    currentTransaction.L2_InstructedAmount = l2.InstructedAmount;
                    currentTransaction.L2_Currency = l2.Currency;
                    currentTransaction.L2_ChargeBearer = l2.ChargeBearer;
                    currentTransaction.L2_MandateReference = l2.MandateReference;
                    currentTransaction.L2_DebtorBankBranch = l2.DebtorBankBranch;
                }
                else if (record is CollectionTxLine03 l3)
                {
                    currentTransaction.L3_DebtorName = l3.DebtorName;
                    currentTransaction.L3_DebtorAccountNumber = l3.DebtorAccountNumber;
                    currentTransaction.L3_AccountType = l3.AccountType;
                    currentTransaction.L3_ContractReference = l3.ContractReference;
                    currentTransaction.L3_RelatedCycleDate = l3.RelatedCycleDate;

                    // End of the transaction, add it to the list
                    collectionFile.Transactions.Add(currentTransaction);
                }
            }
            return collectionFile;
        }
    }
    #endregion

}