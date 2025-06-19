using FileHelpers;
using System;
using System.IO;
using System.Text;
using RMCollectionProcessor.Models;

namespace RMCollectionProcessor
{
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

        private Type? CustomRecordSelector(MultiRecordEngine engine, string recordLine)
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

                string lineCount = recordLine.Substring(16, 2);
                if (bankservId == "08" && lineCount == "01") 
                    return typeof(CollectionTxLine01);
                lineCount = recordLine.Substring(8, 2);
                if (lineCount == "02") 
                    return typeof(CollectionTxLine02);
                if (lineCount == "03") 
                    return typeof(CollectionTxLine03);

            }

            return null; // Unknown record type
        }
    }
}
