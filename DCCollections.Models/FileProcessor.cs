using FileHelpers;
using System;
using System.IO;
using System.Text;
using RMCollectionProcessor.Models;

namespace RMCollectionProcessor
{
    public class FileProcessor
    {
        private Type? _lastProcessedRecordType;

        /// <summary>
        /// Processes a file using a multi-record engine capable of handling Collection, Status, and Reply file formats.
        /// </summary>
        /// <param name="filePath">The path to the file to process.</param>
        /// <returns>An array of parsed record objects.</returns>
        public object[] ProcessFile(string filePath)
        {
            _lastProcessedRecordType = null;

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
            if (string.IsNullOrEmpty(recordLine))
            {
                _lastProcessedRecordType = null;
                return null;
            }

            Type? selectedType = null;

            if (recordLine.Length >= 3)
            {
                string recordId = recordLine.Substring(0, 3);
                switch (recordId)
                {
                    case "000": selectedType = typeof(TransmissionHeader000); break;
                    case "999": selectedType = typeof(TransmissionTrailer999); break;
                    case "084": selectedType = typeof(StatusUserSetTrailer084); break;
                    case "085": selectedType = typeof(StatusUserSetErrorRecord085); break;

                    case "080":
                        if (recordLine.Length >= 7)
                        {
                            string bankservId = recordLine.Substring(4, 2);
                            if (bankservId == "04") selectedType = typeof(CollectionHeader080);
                            else if (bankservId == "92") selectedType = typeof(CollectionTrailer080);
                            else if (bankservId == "08") selectedType = typeof(CollectionTxLine01);
                            else selectedType = typeof(StatusUserSetHeader080);
                        }
                        else
                        {
                            selectedType = typeof(StatusUserSetHeader080);
                        }
                        break;

                    case "081":
                        if (_lastProcessedRecordType == typeof(StatusUserSetHeader080)) selectedType = typeof(StatusUserSetHeaderLine01);
                        else if (_lastProcessedRecordType == typeof(StatusUserSetHeaderLine01)) selectedType = typeof(StatusUserSetHeaderLine02);
                        break;

                    case "082":
                        if (_lastProcessedRecordType == typeof(StatusUserSetHeaderLine02) ||
                            _lastProcessedRecordType == typeof(StatusUserSetTransactionLine04) ||
                            _lastProcessedRecordType == typeof(StatusUserSetErrorRecord085))
                        {
                            selectedType = typeof(StatusUserSetTransactionLine01);
                        }
                        else if (_lastProcessedRecordType == typeof(StatusUserSetTransactionLine01)) selectedType = typeof(StatusUserSetTransactionLine02);
                        else if (_lastProcessedRecordType == typeof(StatusUserSetTransactionLine02)) selectedType = typeof(StatusUserSetTransactionLine03);
                        else if (_lastProcessedRecordType == typeof(StatusUserSetTransactionLine03)) selectedType = typeof(StatusUserSetTransactionLine04);
                        break;

                    case "900":
                        if (recordLine.Length >= 8)
                        {
                            string indicator900 = recordLine.Substring(4, 3);
                            if (indicator900 == "000") selectedType = typeof(ReplyTransmissionStatus900);
                            else if (indicator900 == "080") selectedType = typeof(ReplyUserSetStatus900);
                        }
                        break;

                    case "901":
                        if (recordLine.Length >= 8)
                        {
                            string indicator901 = recordLine.Substring(4, 3);
                            if (indicator901 == "080") selectedType = typeof(ReplyRejectedMessage901);
                            else if (indicator901 == "000") selectedType = typeof(ReplyTransmissionRejectReason901);
                        }
                        break;
                }
            }

            if (selectedType == null && recordLine.Length >= 2)
            {
                string recordId2Char = recordLine.Substring(0, 2);
                if (recordId2Char == "08")
                {
                    if (_lastProcessedRecordType == typeof(CollectionTxLine01)) selectedType = typeof(CollectionTxLine02);
                    else if (_lastProcessedRecordType == typeof(CollectionTxLine02)) selectedType = typeof(CollectionTxLine03);
                }
            }

            if (selectedType != null)
            {
                _lastProcessedRecordType = selectedType;
            }
            return selectedType;
        }
    }
}