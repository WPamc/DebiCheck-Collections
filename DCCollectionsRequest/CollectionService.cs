using FileHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RMCollectionProcessor
{
    public class CollectionService
    {
        public object[] ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File '{filePath}' not found.");

            var processor = new FileProcessor();
            return processor.ProcessFile(filePath);
        }

        public string GenerateFile(int deductionDay, IConfiguration configuration)
        {
            if (deductionDay < 1 || deductionDay > 31)
                throw new ArgumentOutOfRangeException(nameof(deductionDay), "Deduction day must be between 1 and 31.");

            var dbService = new DatabaseService(configuration);

            var collections = dbService.GetCollectionsAsync(deductionDay).GetAwaiter().GetResult();
            if (!collections.Any())
                throw new InvalidOperationException("No collections to process.");

            var creditorDefaults = new CreditorDefaults();
            int genNumber = dbService.GetNextGenerationNumberAsync().GetAwaiter().GetResult();

            var staticData = new StaticDataProvider(
                recordStatus: "L",
                transmissionNumber: genNumber.ToString(),
                userGenerationNumber: genNumber.ToString(),
                paymentInfoId: $"{genNumber}/{DateTime.Today:yyyy-MM-dd}",
                creditorDefaults: creditorDefaults);

            var records = new List<object>();
            var recordBuilder = new RecordBuilder();

            int firstSeq = dbService.GetNextDailyCounterAsync(DateTime.Today).GetAwaiter().GetResult();

            string fileName = $"ZR{creditorDefaults.UserCode}.AUL.DATA.{DateTime.Now:yyMMdd.HHmmss}";
            int fileRowId = dbService.CreateBankFileRecordAsync(fileName, genNumber, firstSeq).GetAwaiter().GetResult();

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
                    sequenceNumber = dbService.GetNextDailyCounterAsync(DateTime.Today).GetAwaiter().GetResult();
                    dbService.UpdateBankFileDailyCounterEndAsync(fileRowId, sequenceNumber).GetAwaiter().GetResult();
                }
            }

            dbService.UpdateBankFileDailyCounterEndAsync(fileRowId, lastSeq).GetAwaiter().GetResult();

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
            dbService.MarkBankFileGenerationCompleteAsync(fileRowId).GetAwaiter().GetResult();

            return fileName;
        }
    }
}
