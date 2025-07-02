using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileHelpers;

namespace EFT_Collections
{
    /// <summary>
    /// Provides functionality to import EFT files and persist transactions.
    /// </summary>
    public class EFTImportService
    {
        private readonly EftFileIdentifier _identifier;
        private readonly MultiRecordEngine _engine;

        public EFTImportService()
        {
            _identifier = new EftFileIdentifier();
            _engine = new MultiRecordEngine(
                typeof(TransmissionHeader000),
                typeof(EftUserHeader001),
                typeof(EftStandardTransaction001),
                typeof(EftContraRecord001),
                typeof(EftUserTrailer001),
                typeof(TransmissionTrailer999))
            {
                RecordSelector = new RecordTypeSelector(CustomRecordSelector)
            };
        }

        public (object[] Records, EftFileType FileType) ParseFile(string filePath)
        {
            var fileType = _identifier.IdentifyFileType(filePath);
            var records = _engine.ReadFile(filePath);
            if (fileType == EftFileType.CollectionSubmission)
            {
                var data = records
                    .OfType<EftStandardTransaction001>()
                    .Where(r => r.DataSetStatus.Trim().Equals("L", StringComparison.OrdinalIgnoreCase))
                    .Select(r => new DebtorCollectionData
                    {
                        PaymentInformation = r.UserReference.Trim(),
                        RequestedCollectionDate = ParseDate(r.ActionDate),
                        TrackingPeriod = 0,
                        DebitSequence = string.Empty,
                        EntryClass = string.Empty,
                        InstructedAmount = ParseAmount(r.AmountInCents),
                        MandateReference = string.Empty,
                        DebtorBankBranch = r.HomingBranch.Trim(),
                        DebtorName = r.HomingAccountName.Trim(),
                        DebtorAccountNumber = r.HomingAccountNumber.Trim(),
                        AccountType = r.TypeOfAccount.Trim(),
                        ContractReference = string.Empty,
                        RelatedCycleDate = ParseDate(r.ActionDate)
                    })
                    .ToList();

                if (data.Any())
                {
                    var db = new DatabaseService();
                    db.InsertCollectionRequests(data, 0);
                }
            }

            return (records, fileType);
        }

        private static decimal ParseAmount(string value)
        {
            if (decimal.TryParse(value.Trim(), out var cents))
            {
                return cents / 100m;
            }
            return 0m;
        }

        private static DateTime ParseDate(string value)
        {
            return DateTime.TryParseExact(value.Trim(), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                ? dt
                : DateTime.MinValue;
        }

        private Type CustomRecordSelector(MultiRecordEngine engine, string recordLine)
        {
            if (recordLine.Length < 7)
                return null;

            string recId = recordLine.Substring(0, 3);
            if (recId == "000")
                return typeof(TransmissionHeader000);
            if (recId == "999")
                return typeof(TransmissionTrailer999);

            if (recId == "001" || recId == "020")
            {
                string bankservId = recordLine.Substring(4, 2);
                switch (bankservId)
                {
                    case "04": return typeof(EftUserHeader001);
                    case "50": return typeof(EftStandardTransaction001);
                    case "12": return typeof(EftContraRecord001);
                    case "52": return typeof(EftContraRecord001);
                    case "92": return typeof(EftUserTrailer001);
                }
            }
            return null;
        }
    }
}
