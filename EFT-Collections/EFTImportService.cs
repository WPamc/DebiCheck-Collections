using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                typeof(TransmissionTrailer999),
                typeof(ResponseStatus900),
                typeof(RejectionReason901),
                typeof(AcceptedReportRecord903),
                typeof(OutputFileHeader010),
                typeof(UnpaidSetHeader011),
                typeof(UnpaidTransactionDetail013),
                typeof(UnpaidSetTrailer014),
                typeof(RedirectsSetHeader016),
                typeof(RedirectsTransactionDetail017),
                typeof(RedirectsSetTrailer018),
                typeof(OutputFileTrailer019)
            )
            {
                RecordSelector = new RecordTypeSelector(EftFileIdentifier.CustomSelectorForAllEftTypes )
            };
        }

        /// <summary>
        /// Parses a file, identifies its type, and processes its contents against the database.
        /// </summary>
        /// <param name="filePath">The path to the EFT file to process.</param>
        /// <returns>A tuple containing the raw parsed records and the identified file type.</returns>
        public (object[] Records, EftFileType FileType) ParseFile(string filePath)
        {
            var fileType = _identifier.IdentifyFileType(filePath);
            if (fileType == EftFileType.Unknown || fileType == EftFileType.EmptyTransmission)
            {
                return (Array.Empty<object>(), fileType);
            }

            var records = _engine.ReadFile(filePath);
            var db = new DatabaseService();

            switch (fileType)
            {
                case EftFileType.CollectionSubmission:
                    ProcessCollectionSubmission(records, db);
                    break;

                case EftFileType.EftOutput:
                    ProcessEftOutput(records, db, Path.GetFileName(filePath));
                    break;

                case EftFileType.ImmediateResponse:
                    ProcessImmediateResponse(records, db);
                    break;
            }

            return (records, fileType);
        }

        /// <summary>
        /// Processes records from a Collection Submission file and inserts them into the database.
        /// </summary>
        /// <param name="records">The parsed records from the file.</param>
        /// <param name="db">The database service instance.</param>
        private void ProcessCollectionSubmission(object[] records, DatabaseService db)
        {
            var data = records
                .OfType<EftStandardTransaction001>()
                .Select(r => new DebtorCollectionData
                {
                    PaymentInformation = r.UserReference.Trim(),
                    RequestedCollectionDate = ParseDate(r.ActionDate, "yyMMdd"),
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
                    RelatedCycleDate = ParseDate(r.ActionDate, "yyMMdd")
                })
                .ToList();

            if (data.Any())
            {
                db.InsertCollectionRequests(data, 0);
            }
        }

        /// <summary>
        /// Processes records from an EFT Output file, handling both unpaids and redirects.
        /// </summary>
        /// <param name="records">The parsed records from the file.</param>
        /// <param name="db">The database service instance.</param>
        /// <param name="fileName">The name of the file being processed.</param>
        private void ProcessEftOutput(object[] records, DatabaseService db, string fileName)
        {
            var bankFileId = db.GetBankFileRowId(fileName);
            if (bankFileId == 0)
            {
                bankFileId = db.CreateBankFileRecord(fileName, 0, 0);
            }

            var unpaidData = records.OfType<UnpaidTransactionDetail013>().ToList();
            if (unpaidData.Any())
            {
                db.InsertUnpaidTransactions(unpaidData, bankFileId);
            }

            var redirectData = records.OfType<RedirectsTransactionDetail017>().ToList();
            if (redirectData.Any())
            {
               // db.UpdateAccountsFromRedirects(redirectData, bankFileId);
            }
        }

        /// <summary>
        /// Processes records from an Immediate Response file to update batch and transaction statuses.
        /// </summary>
        /// <param name="records">The parsed records from the file.</param>
        /// <param name="db">The database service instance.</param>
        private void ProcessImmediateResponse(object[] records, DatabaseService db)
        {
            var statusUpdates = records.OfType<ResponseStatus900>().ToList();
            if (statusUpdates.Any())
            {
                //db.UpdateBatchStatusFromResponse(statusUpdates);
            }

            var rejectionDetails = records.OfType<RejectionReason901>().ToList();
            if (rejectionDetails.Any())
            {
                //db.UpdateTransactionStatusFromRejections(rejectionDetails);
            }
        }

        /// <summary>
        /// Parses an amount string in cents into a decimal value.
        /// </summary>
        /// <param name="value">The amount in cents as a string.</param>
        /// <returns>The decimal value.</returns>
        private static decimal ParseAmount(string value)
        {
            if (decimal.TryParse(value.Trim(), out var cents))
            {
                return cents / 100m;
            }
            return 0m;
        }

        /// <summary>
        /// Parses a date string with a given format.
        /// </summary>
        /// <param name="value">The date string.</param>
        /// <param name="format">The expected date format (e.g., "yyMMdd", "yyyyMMdd").</param>
        /// <returns>The parsed DateTime, or DateTime.MinValue if parsing fails.</returns>
        private static DateTime ParseDate(string value, string format)
        {
            return DateTime.TryParseExact(value.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                ? dt
                : DateTime.MinValue;
        }

      
    }
}