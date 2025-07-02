using FileHelpers;

namespace EFT_Collections
{
    /// <summary>
    /// Represents the Transmission Header record (the first line in the file).
    /// Conforms to the specification in Section 5.1.1.1, page 15.
    /// Static values: RecordIdentifier = "000", Destination = "00000" for submissions to Absa
    /// Example: RecordStatus = 'L' (Live) or 'T' (Test), TransmissionDate = "20231027", ClientCode = "12345", TransmissionNumber = "0000123"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class TransmissionHeader000
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "000"
        [FieldFixedLength(1)] public string RecordStatus;             // 'L' or 'T'
        [FieldFixedLength(8)] public string TransmissionDate;         // e.g., "20231027"
        [FieldFixedLength(5)] public string ClientCode;               // e.g., "12345"
        [FieldFixedLength(30)] public string ClientName;              // e.g., "MY COMPANY PTY LTD"
        [FieldFixedLength(7)] public string TransmissionNumber;       // e.g., "0000123"
        [FieldFixedLength(5)] public string Destination;              // "00000"
        [FieldFixedLength(119)] public string Filler1;
        [FieldFixedLength(20)] public string ForLdUsersUse;
    }

    /// <summary>
    /// Represents the User Header Record, marking the start of a transaction batch.
    /// Conforms to the specification in Section 5.1.2.1, page 17.
    /// Static values: RecordIdentifier = "001" or "020", BankservRecordId = "04", TypeOfService = "SAMEDAY"
    /// Example: DataSetStatus = 'L' (Live) or 'T' (Test), BankservCreationDate = "231027", BankservUserCode = "1234", FirstActionDate = "231101"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftUserHeader001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "04"
        [FieldFixedLength(4)] public string BankservUserCode;         // e.g., "1234"
        [FieldFixedLength(6)] public string BankservCreationDate;     // e.g., "231027"
        [FieldFixedLength(6)] public string BankservPurgeDate;        // e.g., "231115"
        [FieldFixedLength(6)] public string FirstActionDate;          // e.g., "231101"
        [FieldFixedLength(6)] public string LastActionDate;           // e.g., "231101"
        [FieldFixedLength(6)] public string FirstSequenceNumber;      // e.g., "000001"
        [FieldFixedLength(4)] public string UserGenerationNumber;     // e.g., "0001"
        [FieldFixedLength(10)] public string TypeOfService;           // e.g., "SAMEDAY"
        [FieldFixedLength(1)] public string AcceptedReport;           // 'Y' or ' '
        [FieldFixedLength(1)] public string AccountTypeCorrection;    // 'Y' or ' '
        [FieldFixedLength(142)] public string Filler;
    }

    /// <summary>
    /// Represents a Standard Transaction Record (one for each collection).
    /// Conforms to the specification in Section 5.1.2.2, pages 18-19.
    /// Static values: RecordIdentifier = "001" or "020", BankservRecordId = "50" for Direct Debit (Collection), HomingInstitution = "21"
    /// Example: AmountInCents = "00000123450" (for R1234.50), TypeOfAccount = '1' (Current), UserReference = "INV12345"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftStandardTransaction001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "50" for Direct Debit (Collection)
        [FieldFixedLength(6)] public string UserBranch;               // e.g., "632005"
        [FieldFixedLength(11)] public string UserNominatedAccount;     // e.g., "40123456789"
        [FieldFixedLength(4)] public string UserCode;                 // e.g., "1234"
        [FieldFixedLength(6)] public string UserSequenceNumber;       // e.g., "000001"
        [FieldFixedLength(6)] public string HomingBranch;             // e.g., "632005"
        [FieldFixedLength(11)] public string HomingAccountNumber;      // e.g., "98765432101"
        [FieldFixedLength(1)] public string TypeOfAccount;            // '1' (Current), '2' (Savings)
        [FieldFixedLength(11)] public string AmountInCents;            // e.g., "00000123450"
        [FieldFixedLength(6)] public string ActionDate;               // e.g., "231101"
        [FieldFixedLength(2)] public string EntryClass;               // e.g., "33"
        [FieldFixedLength(1)] public string TaxCode;                  // e.g., "0"
        [FieldFixedLength(3)] public string Filler1;
        [FieldFixedLength(30)] public string UserReference;           // e.g., "INV12345                    "
        [FieldFixedLength(30)] public string HomingAccountName;        // e.g., "JOHN DOE                    "
        [FieldFixedLength(20)] public string NonStandardHomingAccount;
        [FieldFixedLength(16)] public string Filler2;
        [FieldFixedLength(2)] public string HomingInstitution;        // "21"
        [FieldFixedLength(26)] public string Filler3;
    }

    /// <summary>
    /// Represents the Contra Record, which balances the batch.
    /// Conforms to the specification in Section 5.1.2.3, pages 20-21.
    /// Static values: RecordIdentifier = "001" or "020", BankservRecordId = "12" for Direct Credit Contra, TypeOfAccount = "1", EntryClass = "10"
    /// Example: UserReference = "MYCOMPANY CONTRA", AmountInCents = "00000123450" (Total of transactions)
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftContraRecord001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "12" for Direct Credit Contra
        [FieldFixedLength(6)] public string UserBranch;               // e.g., "632005"
        [FieldFixedLength(11)] public string UserNominatedAccount;     // e.g., "40123456789"
        [FieldFixedLength(4)] public string UserCode;                 // e.g., "1234"
        [FieldFixedLength(6)] public string UserSequenceNumber;       // e.g., "000002"
        [FieldFixedLength(6)] public string HomingBranch;             // e.g., "632005"
        [FieldFixedLength(11)] public string HomingAccountNumber;      // e.g., "40123456789"
        [FieldFixedLength(1)] public string TypeOfAccount;            // "1"
        [FieldFixedLength(11)] public string AmountInCents;            // e.g., "00000123450"
        [FieldFixedLength(6)] public string ActionDate;               // e.g., "231101"
        [FieldFixedLength(2)] public string EntryClass;               // "10"
        [FieldFixedLength(4)] public string Filler1;
        [FieldFixedLength(30)] public string UserReference;           // e.g., "MYCOMPANY CONTRA            "
        [FieldFixedLength(30)] public string Filler2;
        [FieldFixedLength(64)] public string Filler3;
    }

    /// <summary>
    /// Represents the User Trailer Record, concluding the transaction batch with totals.
    /// Conforms to the specification in Section 5.1.2.4, pages 22-23.
    /// Static values: RecordIdentifier = "001" or "020", BankservRecordId = "92"
    /// Example: NumberOfDebitRecords = "000001", TotalDebitValueInCents = "00000123450"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftUserTrailer001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "92"
        [FieldFixedLength(4)] public string UserCode;                 // e.g., "1234"
        [FieldFixedLength(6)] public string FirstSequenceNumber;      // e.g., "000001"
        [FieldFixedLength(6)] public string LastSequenceNumber;       // e.g., "000002"
        [FieldFixedLength(6)] public string FirstActionDate;          // e.g., "231101"
        [FieldFixedLength(6)] public string LastActionDate;           // e.g., "231101"
        [FieldFixedLength(6)] public string NumberOfDebitRecords;     // e.g., "000001"
        [FieldFixedLength(6)] public string NumberOfCreditRecords;    // e.g., "000000"
        [FieldFixedLength(6)] public string NumberOfContraRecords;    // e.g., "000001"
        [FieldFixedLength(12)] public string TotalDebitValueInCents;   // e.g., "00000123450"
        [FieldFixedLength(12)] public string TotalCreditValueInCents;  // e.g., "00000000000"
        [FieldFixedLength(12)] public string HashTotalOfHomingAcctNos; // e.g., "098765432101"
        [FieldFixedLength(110)] public string Filler;
    }

    /// <summary>
    /// Represents the Transmission Trailer record (the last line in the file).
    /// Conforms to the specification in Section 5.1.1.2, page 16.
    /// Static values: RecordIdentifier = "999"
    /// Example: NumberOfRecords = "000000004"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class TransmissionTrailer999
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "999"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(9)] public string NumberOfRecords;          // e.g., "000000004"
        [FieldFixedLength(185)] public string Filler;
    }

    /// <summary>
    /// Represents a status record from a reply file (Record ID "900").
    /// This can indicate the status of the entire transmission or a specific user set.
    /// Conforms to the specification in Section 5.1.14.1 and 5.1.14.3, pages 79-81.
    /// Static values: RecordIdentifier = "900"
    /// Example: ServiceIndicator = "001", UserSetStatus = "ACCEPTED"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class ResponseStatus900
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "900"
        [FieldFixedLength(1)] public string Status;                     // 'L' or 'T'
        [FieldFixedLength(3)] public string ServiceIndicator;           // "001" (EFT), "050" (NAEDO), etc.
        [FieldFixedLength(14)] public string MessageText;               // e.g., "ACB USER SET "
        [FieldFixedLength(4)] public string BankservUserCode;           // e.g., "1234"
        [FieldFixedLength(1)] public string Separator1;                 // "-"
        [FieldFixedLength(7)] public string UserCodeGenerationNumber;   // e.g., "0000001"
        [FieldFixedLength(1)] public string Separator2;                 // "-"
        [FieldFixedLength(6)] public string LastSequenceNumber;         // e.g., "000002"
        [FieldFixedLength(1)] public string Separator3;                 // "-"
        [FieldFixedLength(8)] public string UserSetStatus;              // "ACCEPTED" or "REJECTED"
        [FieldFixedLength(149)] public string Filler;
    }

    /// <summary>
    /// Represents a rejected message record from a reply file (Record ID "901").
    /// Provides detailed reasons for a specific transaction's failure.
    /// Conforms to the specification in Section 5.1.14.6, page 84.
    /// Static values: RecordIdentifier = "901"
    /// Example: ErrorCode = "00034", ErrorMessage = "HOMING ACCOUNT INVALID"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class RejectionReason901
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "901"
        [FieldFixedLength(1)] public string Status;                     // 'L' or 'T'
        [FieldFixedLength(3)] public string ServiceIndicator;           // e.g., "001"
        [FieldFixedLength(1)] public string Filler1;                    // " "
        [FieldFixedLength(4)] public string BankservUserCode;           // e.g., "1234"
        [FieldFixedLength(1)] public string Filler2;                    // "/"
        [FieldFixedLength(7)] public string UserCodeGenerationNumber;   // e.g., "0000001"
        [FieldFixedLength(1)] public string Filler3;                    // "/"
        [FieldFixedLength(6)] public string UserSequenceNumber;         // e.g., "000001"
        [FieldFixedLength(1)] public string Filler4;                    // "-"
        [FieldFixedLength(5)] public string ErrorCode;                  // e.g., "00034"
        [FieldFixedLength(1)] public string Filler5;                    // "-"
        [FieldFixedLength(60)] public string ErrorMessage;              // e.g., "HOMING ACCOUNT INVALID"
        [FieldFixedLength(104)] public string Filler6;
    }

    /// <summary>
    /// Represents an accepted report record from a reply file (Record ID "903").
    /// This record echoes back an entire input transaction that was successfully accepted for processing.
    /// Conforms to the specification in Section 5.1.14.5, page 83.
    /// Static values: RecordIdentifier = "903"
    /// Example: AcceptedReportTransaction will contain the full 194-byte string of the original record.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class AcceptedReportRecord903
    {
        [FieldFixedLength(3)] public string RecordIdentifier;            // "903"
        [FieldFixedLength(1)] public string Status;                      // 'L' or 'T'
        [FieldFixedLength(194)] public string AcceptedReportTransaction; // The full original record string
    }

    /// <summary>
    /// Represents the header for an EFT Output File (Record ID "010").
    /// This file typically contains details of unpaid and redirected transactions.
    /// Conforms to the specification in Section 5.1.4.1, page 30.
    /// Static values: RecordIdentifier = "010"
    /// Example: BankservUserCode = "1234", BankservGenerationNumber = "0000123", BankservService = "03" (2 Day)
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class OutputFileHeader010
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "010"
        [FieldFixedLength(1)] public string DataSetStatus;              // 'L' or 'T'
        [FieldFixedLength(4)] public string BankservUserCode;           // e.g., "1234"
        [FieldFixedLength(7)] public string BankservGenerationNumber;   // e.g., "0000123"
        [FieldFixedLength(2)] public string BankservService;            // e.g., "03" (2 Day)
        [FieldFixedLength(181)] public string Filler;
    }

    /// <summary>
    /// Represents the header for a set of unpaid transactions within an EFT Output File (Record ID "011").
    /// Conforms to the specification in Section 5.1.4.2, page 31.
    /// Static values: RecordIdentifier = "011"
    /// Example: ActionDateForDataSet = "20231027", NominatedAccountNumber = "40123456789"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class UnpaidSetHeader011
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "011"
        [FieldFixedLength(1)] public string DataSetStatus;              // 'L' or 'T'
        [FieldFixedLength(4)] public string BankservUserCode;           // e.g., "1234"
        [FieldFixedLength(6)] public string NominatedAccountBranch;     // e.g., "632005"
        [FieldFixedLength(16)] public string NominatedAccountNumber;    // e.g., "0000040123456789"
        [FieldFixedLength(2)] public string NominatedAccountType;       // e.g., "01" (Current)
        [FieldFixedLength(8)] public string ActionDateForDataSet;       // e.g., "20231027"
        [FieldFixedLength(158)] public string Filler;
    }

    /// <summary>
    /// Represents the detailed record for a single unpaid transaction (Record ID "013").
    /// Conforms to the specification in Section 5.1.4.3, page 32.
    /// Static values: RecordIdentifier = "013"
    /// Example: TransactionType = "50" (Debit), RejectionReason = "004" (Payment Stopped), AmountInCents = "00000123450"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class UnpaidTransactionDetail013
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "013"
        [FieldFixedLength(1)] public string DataSetStatus;              // 'L' or 'T'
        [FieldFixedLength(2)] public string TransactionType;            // "50" (Debit), "10" (Credit)
        [FieldFixedLength(8)] public string TransmissionDate;           // e.g., "20231025"
        [FieldFixedLength(6)] public string OriginalSequenceNumber;     // e.g., "000001"
        [FieldFixedLength(6)] public string HomingBranchCode;           // e.g., "632005"
        [FieldFixedLength(16)] public string HomingAccountNumber;       // e.g., "0000098765432101"
        [FieldFixedLength(11)] public string AmountInCents;             // e.g., "00000123450"
        [FieldFixedLength(30)] public string UserReference;             // e.g., "INV12345"
        [FieldFixedLength(3)] public string RejectionReason;            // e.g., "004" (See Annexure 6)
        [FieldFixedLength(5)] public string RejectionQualifier;         // e.g., "00007" (See Annexure 7)
        [FieldFixedLength(6)] public string DistributionSequenceNumber; // e.g., "123456"
        [FieldFixedLength(16)] public string Filler1;
        [FieldFixedLength(30)] public string HomingAccountName;         // e.g., "JOHN DOE"
        [FieldFixedLength(55)] public string Filler2;
    }

    /// <summary>
    /// Represents the trailer for a set of unpaid transactions, containing hash totals (Record ID "014").
    /// Conforms to the specification in Section 5.1.4.4, page 33.
    /// Static values: RecordIdentifier = "014"
    /// Example: NumberOfDebitRecords = "000000015", DebitAmountHashTotal = "00001851750"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class UnpaidSetTrailer014
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "014"
        [FieldFixedLength(1)] public string DataSetStatus;              // 'L' or 'T'
        [FieldFixedLength(9)] public string NumberOfDebitRecords;       // e.g., "000000015"
        [FieldFixedLength(9)] public string NumberOfCreditRecords;      // e.g., "000000000"
        [FieldFixedLength(18)] public string HomingAccountHashTotal;    // e.g., "000148148148148148"
        [FieldFixedLength(14)] public string DebitAmountHashTotal;      // e.g., "0000001851750"
        [FieldFixedLength(14)] public string CreditAmountHashTotal;     // e.g., "0000000000000"
        [FieldFixedLength(130)] public string Filler;
    }

    /// <summary>
    /// Represents the trailer for an entire EFT Output File (Record ID "019").
    /// Conforms to the specification in Section 5.1.4.8, page 36.
    /// Static values: RecordIdentifier = "019"
    /// Example: NumberOfDebitRecords = "000000015", DebitAmountHashTotal = "00001851750"
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class OutputFileTrailer019
    {
        [FieldFixedLength(3)] public string RecordIdentifier;           // "019"
        [FieldFixedLength(1)] public string DataSetStatus;              // 'L' or 'T'
        [FieldFixedLength(9)] public string NumberOfDebitRecords;       // e.g., "000000015"
        [FieldFixedLength(9)] public string NumberOfCreditRecords;      // e.g., "000000000"
        [FieldFixedLength(18)] public string HomingAccountHashTotal;    // e.g., "000148148148148148"
        [FieldFixedLength(14)] public string DebitAmountHashTotal;      // e.g., "0000001851750"
        [FieldFixedLength(14)] public string CreditAmountHashTotal;     // e.g., "0000000000000"
        [FieldFixedLength(130)] public string Filler;
    }
}