using FileHelpers;
using System;
using System.IO;
using System.Text;
using RMCollectionProcessor.Models;

namespace RMCollectionProcessor
{
    public class FileProcessor
    {
        /// <summary>
        /// Processes a file using a multi-record engine capable of handling Collection, Status, and Reply file formats.
        /// </summary>
        /// <param name="filePath">The path to the file to process.</param>
        /// <returns>An array of parsed record objects.</returns>
        public object[] ProcessFile(string filePath)
        {
            var engine = new MultiRecordEngine(
                typeof(TransmissionHeader000),
                typeof(CollectionHeader080),
                typeof(CollectionTxLine01),
                typeof(CollectionTxLine02),
                typeof(CollectionTxLine03),
                typeof(CollectionTrailer080),
                typeof(TransmissionTrailer999),
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
                typeof(ReplyTransmissionRejectReason901)
                )
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

        /// <summary>
        /// Selects the appropriate record type based on the content of the record line.
        /// </summary>
        /// <param name="engine">The multi-record engine.</param>
        /// <param name="recordLine">The string content of the line to parse.</param>
        /// <returns>The Type of the model that corresponds to the record line.</returns>
        private Type? CustomRecordSelector(MultiRecordEngine engine, string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine) || recordLine.Length < 3)
                return null;

            string recordId = recordLine.Substring(0, 3);

            switch (recordId)
            {
                case "000": return typeof(TransmissionHeader000);
                case "999": return typeof(TransmissionTrailer999);
                case "084": return typeof(StatusUserSetTrailer084);
                case "085": return typeof(StatusUserSetErrorRecord085);

                case "080":
                    Type lastRecordType = engine.LastRecord?.GetType();

                    if (recordLine.Length >= 10)
                    {
                        string lineCount = recordLine.Substring(8, 2);
                        if (lastRecordType == typeof(CollectionTxLine01) && lineCount == "02")
                        {
                            return typeof(CollectionTxLine02);
                        }
                        if (lastRecordType == typeof(CollectionTxLine02) && lineCount == "03")
                        {
                            return typeof(CollectionTxLine03);
                        }
                    }

                    if (recordLine.Length < 6) return null;
                    string bankservId = recordLine.Substring(4, 2);
                    if (bankservId == "04") return typeof(CollectionHeader080);
                    if (bankservId == "92") return typeof(CollectionTrailer080);
                    if (bankservId == "08") return typeof(CollectionTxLine01);

                    return typeof(StatusUserSetHeader080);

                case "081":
                    if (recordLine.Length < 8) return null;
                    string lineCount081 = recordLine.Substring(6, 2);
                    if (lineCount081 == "01") return typeof(StatusUserSetHeaderLine01);
                    if (lineCount081 == "02") return typeof(StatusUserSetHeaderLine02);
                    break;

                case "082":
                    if (recordLine.Length < 5) return null;
                    string lineCount082 = recordLine.Substring(3, 2);
                    if (lineCount082 == "01") return typeof(StatusUserSetTransactionLine01);
                    if (lineCount082 == "02") return typeof(StatusUserSetTransactionLine02);
                    if (lineCount082 == "03") return typeof(StatusUserSetTransactionLine03);
                    if (lineCount082 == "04") return typeof(StatusUserSetTransactionLine04);
                    break;

                case "900":
                    if (recordLine.Length < 7) return null;
                    string indicator900 = recordLine.Substring(4, 3);
                    if (indicator900 == "000") return typeof(ReplyTransmissionStatus900);
                    if (indicator900 == "080") return typeof(ReplyUserSetStatus900);
                    break;

                case "901":
                    if (recordLine.Length < 7) return null;
                    string indicator901 = recordLine.Substring(4, 3);
                    if (indicator901 == "080") return typeof(ReplyRejectedMessage901);
                    if (indicator901 == "000") return typeof(ReplyTransmissionRejectReason901);
                    break;
            }

            return null;
        }
    }
}