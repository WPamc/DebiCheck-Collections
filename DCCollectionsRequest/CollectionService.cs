using FileHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RMCollectionProcessor.Models;

namespace RMCollectionProcessor
{
    public class CollectionService
    {
        private readonly CsvFileStore _store;

        public CollectionService()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "transactions.csv");
            _store = new CsvFileStore(path);
        }

        /// <summary>
        /// Parses the specified file, identifies its type, and routes it to the appropriate processing logic.
        /// </summary>
        /// <param name="filePath">The path to the file to parse.</param>
        /// <returns>The parsed records and their detected file type.</returns>
        public ParseResult ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File '{filePath}' not found.");

            var processor = new FileProcessor();
            var parsed = processor.ProcessFile(filePath);

            var fileType = FileTypeIdentifier.Identify(parsed);

            switch (fileType)
            {
                case FileType.CollectionRequest:
                    var records = ExtractTransactionRecords(filePath, parsed);
                    _store.AddRecords(records);
                    break;
                case FileType.StatusReport:
                    throw new NotImplementedException("Processing for Status Report files is not yet implemented.");
                case FileType.Reply:
                    throw new NotImplementedException("Processing for Reply files is not yet implemented.");
                case FileType.Unknown:
                default:
                    throw new InvalidDataException($"The file '{filePath}' is of an unknown or unsupported type.");
            }

            return new ParseResult(parsed, fileType);
        }

        public string GenerateFile(int deductionDay, IConfiguration configuration, bool isTest = false)
        {
            if (deductionDay < 1 || deductionDay > 31)
                throw new ArgumentOutOfRangeException(nameof(deductionDay), "Deduction day must be between 1 and 31.");

            var dbService = new DatabaseService(configuration);

            var collections = dbService.GetCollections(deductionDay);
            if (!collections.Any())
                throw new InvalidOperationException("No collections to process.");


            var now = DateTime.Now;
            var cutOffTime = new TimeSpan(01, 30, 0);
//adjust collection date if too late
            if (now.TimeOfDay > cutOffTime)
            {
                foreach (var collection in collections)
                {
                    // Check if the collection is scheduled for today
                    if (collection.RequestedCollectionDate.Date.Day <= now.Date.Day)
                    {
                        // Move the collection to the next day
                        collection.RequestedCollectionDate = now.Date.AddDays(1);

                        // Per the spec (page 55), the RelatedDate/CycleDate must be kept in sync
                        // for FRST, OOFF, and RCUR debit sequences.
                        string debitSeq = collection.DebitSequence.Trim().ToUpper();
                        if (debitSeq == "FRST" || debitSeq == "OOFF" || debitSeq == "RCUR")
                        {
                            collection.RelatedCycleDate = collection.RequestedCollectionDate;
                        }
                    }
                }
            }


            var creditorDefaults = new CreditorDefaults();
            int genNumber;
            if (isTest)
            {
                genNumber = dbService.GetCurrentGenerationNumber() + 1;
            }
            else
            {
                genNumber = dbService.GetNextGenerationNumber();
            }

            var staticData = new StaticDataProvider(
                recordStatus: isTest ? "T" : "L",
                transmissionNumber: genNumber.ToString(),
                userGenerationNumber: genNumber.ToString(),
                paymentInfoId: $"{genNumber}/{DateTime.Today:yyyy-MM-dd}",
                creditorDefaults: creditorDefaults);

            var records = new List<object>();
            var recordBuilder = new RecordBuilder();

            int firstSeq;
            if (isTest)
            {
                firstSeq = dbService.GetCurrentDailyCounter(DateTime.Today) + 1;
            }
            else
            {
                firstSeq = dbService.GetNextDailyCounter(DateTime.Today);
            }

            string fileName = $"ZR{creditorDefaults.UserCode}.AUL.DATA.{DateTime.Now:yyMMdd.HHmmss}";
            int fileRowId = -1;
            if (!isTest)
            {
                fileRowId = dbService.CreateBankFileRecord(fileName, genNumber, firstSeq);
            }

            records.Add(recordBuilder.BuildTransmissionHeader(staticData));

            int sequenceNumber = firstSeq;
            int lastSeq = firstSeq;
            for (int i = 0; i < collections.Count; i++)
            {
                var debtorData = collections[i];
                var (line1, line2, line3) = recordBuilder.BuildTransactionLines(staticData, debtorData, sequenceNumber);
                records.Add(line1);
                records.Add(line2);
                records.Add(line3);
                lastSeq = sequenceNumber;
                if (i < collections.Count - 1)
                {
                    if (isTest)
                    {
                        sequenceNumber += 1;
                    }
                    else
                    {
                        sequenceNumber = dbService.GetNextDailyCounter(DateTime.Today);
                    }
                    if (!isTest)
                    {
                        dbService.UpdateBankFileDailyCounterEnd(fileRowId, sequenceNumber);
                    }
                }
            }

            if (!isTest)
            {
                dbService.UpdateBankFileDailyCounterEnd(fileRowId, lastSeq);
            }

            var collectionHeader = recordBuilder.BuildCollectionHeader(staticData, firstSeq, collections.Count);
            records.Insert(1, collectionHeader);

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

            engine.WriteFile(fileName, records);

            if (!isTest)
            {
                // Parse the generated file and store each request
                var txRecords = ExtractTransactionRecords(fileName, records.ToArray());
                dbService.InsertCollectionRequests(txRecords, fileRowId);

                dbService.MarkBankFileGenerationComplete(fileRowId);
            }

            return fileName;
        }

        public BillingCollectionRequest? GetRequestByReference(string reference, IConfiguration configuration)
        {
            var dbService = new DatabaseService(configuration);
            return dbService.GetCollectionRequestByReference(reference);
        }

        private IEnumerable<TransactionRecord> ExtractTransactionRecords(string filePath, object[] parsed)
        {
            string fileName = Path.GetFileName(filePath);
            string dataSetStatus = string.Empty;
            string generationNumber = string.Empty;
            string submissionDate = string.Empty;

            foreach (var obj in parsed)
            {
                if (obj is CollectionHeader080 h)
                {
                    dataSetStatus = h.DataSetStatus.Trim();
                    generationNumber = h.UserGenerationNumber.Trim();
                    submissionDate = h.CreationDateTime.Trim();
                    break;
                }
            }

            var dict = new Dictionary<string, (string status, CollectionTxLine01? l1, CollectionTxLine02? l2, CollectionTxLine03? l3)>();
            foreach (var obj in parsed)
            {
                switch (obj)
                {
                    case CollectionTxLine01 l1:
                        var seq1 = l1.RecordSequenceNumber.Trim();
                        if (!dict.ContainsKey(seq1))
                            dict[seq1] = (l1.DataSetStatus.Trim(), null, null, null);
                        var tup1 = dict[seq1];
                        tup1.l1 = l1;
                        tup1.status = l1.DataSetStatus.Trim();
                        dict[seq1] = tup1;
                        break;
                    case CollectionTxLine02 l2:
                        var seq2 = l2.RecordSequenceNumber.Trim();
                        if (!dict.ContainsKey(seq2))
                            dict[seq2] = (dataSetStatus, null, null, null);
                        var tup2 = dict[seq2];
                        tup2.l2 = l2;
                        dict[seq2] = tup2;
                        break;
                    case CollectionTxLine03 l3:
                        var seq3 = l3.RecordSequenceNumber.Trim();
                        if (!dict.ContainsKey(seq3))
                            dict[seq3] = (dataSetStatus, null, null, null);
                        var tup3 = dict[seq3];
                        tup3.l3 = l3;
                        dict[seq3] = tup3;
                        break;
                }
            }

            foreach (var kvp in dict)
            {
                var l1 = kvp.Value.l1;
                var l2 = kvp.Value.l2;
                var l3 = kvp.Value.l3;
                yield return new TransactionRecord
                {
                    Filename = fileName,
                    DataSetStatus = kvp.Value.status,
                    GenerationNumber = generationNumber,
                    RecordSequenceNumber = kvp.Key,
                    SubmissionDate = submissionDate,
                    PaymentInformation = l1?.PaymentInformation.Trim() ?? string.Empty,
                    RequestedCollectionDate = l1?.RequestedCollectionDate.Trim() ?? string.Empty,
                    InstructedAmount = l2?.InstructedAmount.Trim() ?? string.Empty,
                    MandateReference = l2?.MandateReference.Trim() ?? string.Empty,
                    ContractReference = l3?.ContractReference.Trim() ?? string.Empty,
                    RelatedCycleDate = l3?.RelatedCycleDate.Trim() ?? string.Empty,
                    FileType = "RM Collections"
                };
            }
        }
    }
}
