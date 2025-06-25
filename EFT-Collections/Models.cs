using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace EFT_Collections
{// Note: These models assume a 198-byte data record length, which is standard for this format.
 // The specification document is sometimes ambiguous, implying 199 bytes, but 198 is the
 // conventional interpretation for the data portion of the fixed-length records.

    /// <summary>
    /// Represents the Transmission Header record (the first line in the file).
    /// Conforms to the specification in Section 5.1.1.1, page 15.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class TransmissionHeader000
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "000"
        [FieldFixedLength(1)] public string RecordStatus;             // 'L' or 'T'
        [FieldFixedLength(8)] public string TransmissionDate;         // CCYYMMDD
        [FieldFixedLength(5)] public string ClientCode;               // Your client code from Absa
        [FieldFixedLength(30)] public string ClientName;
        [FieldFixedLength(7)] public string TransmissionNumber;
        [FieldFixedLength(5)] public string Destination;              // Should be "00000" for submissions to Absa
        [FieldFixedLength(119)] public string Filler1;
        [FieldFixedLength(20)] public string ForLdUsersUse;
    }

    /// <summary>
    /// Represents the User Header Record, marking the start of a transaction batch.
    /// Conforms to the specification in Section 5.1.2.1, page 17.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftUserHeader001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "04"
        [FieldFixedLength(4)] public string BankservUserCode;         // Your 4-digit user code
        [FieldFixedLength(6)] public string BankservCreationDate;     // YYMMDD
        [FieldFixedLength(6)] public string BankservPurgeDate;        // YYMMDD
        [FieldFixedLength(6)] public string FirstActionDate;          // YYMMDD
        [FieldFixedLength(6)] public string LastActionDate;           // YYMMDD
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(4)] public string UserGenerationNumber;
        [FieldFixedLength(10)] public string TypeOfService;           // e.g., "TWO DAY   "
        [FieldFixedLength(1)] public string AcceptedReport;           // 'Y' or blank
        [FieldFixedLength(1)] public string AccountTypeCorrection;    // 'Y' or blank
                                                                      // Filler length is calculated as 198 (total) - 56 (fields above) = 142
        [FieldFixedLength(142)] public string Filler;
    }

    /// <summary>
    /// Represents a Standard Transaction Record (one for each collection).
    /// Conforms to the specification in Section 5.1.2.2, pages 18-19.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftStandardTransaction001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "50" for Direct Debit (Collection)
        [FieldFixedLength(6)] public string UserBranch;               // Your (creditor's) branch code
        [FieldFixedLength(11)] public string UserNominatedAccount;     // Your (creditor's) account number
        [FieldFixedLength(4)] public string UserCode;                 // Your 4-digit user code
        [FieldFixedLength(6)] public string UserSequenceNumber;
        [FieldFixedLength(6)] public string HomingBranch;             // Customer's (debtor's) branch code
        [FieldFixedLength(11)] public string HomingAccountNumber;      // Customer's (debtor's) account number
        [FieldFixedLength(1)] public string TypeOfAccount;            // '1'=Current, '2'=Savings, etc.
        [FieldFixedLength(11)] public string AmountInCents;            // e.g., 00000123450 for R1234.50
        [FieldFixedLength(6)] public string ActionDate;               // YYMMDD
        [FieldFixedLength(2)] public string EntryClass;
        [FieldFixedLength(1)] public string TaxCode;
        [FieldFixedLength(3)] public string Filler1;
        [FieldFixedLength(30)] public string UserReference;           // Appears on debtor's statement
        [FieldFixedLength(30)] public string HomingAccountName;        // Debtor's account name
        [FieldFixedLength(20)] public string NonStandardHomingAccount;
        [FieldFixedLength(16)] public string Filler2;
        [FieldFixedLength(2)] public string HomingInstitution;        // "21"
                                                                      // Filler length is calculated as 198 (total) - 172 (fields above) = 26
        [FieldFixedLength(26)] public string Filler3;
    }

    /// <summary>
    /// Represents the Contra Record, which balances the batch.
    /// Conforms to the specification in Section 5.1.2.3, pages 20-21.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftContraRecord001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "12" for Direct Credit Contra (balancing a debit batch)
        [FieldFixedLength(6)] public string UserBranch;
        [FieldFixedLength(11)] public string UserNominatedAccount;
        [FieldFixedLength(4)] public string UserCode;
        [FieldFixedLength(6)] public string UserSequenceNumber;
        [FieldFixedLength(6)] public string HomingBranch;
        [FieldFixedLength(11)] public string HomingAccountNumber;
        [FieldFixedLength(1)] public string TypeOfAccount;            // "1"
        [FieldFixedLength(11)] public string AmountInCents;            // Total of all standard transactions
        [FieldFixedLength(6)] public string ActionDate;               // YYMMDD
        [FieldFixedLength(2)] public string EntryClass;               // "10"
        [FieldFixedLength(4)] public string Filler1;
        [FieldFixedLength(30)] public string UserReference;           // e.g., "MYCOMPANY CONTRA"
        [FieldFixedLength(30)] public string Filler2;                  // Homing Account Name for contra
                                                                       // Filler length is calculated as 198 (total) - 134 (fields above) = 64
                                                                       // The spec on page 21 is likely a typo showing a shorter record. We maintain the 198-byte length.
        [FieldFixedLength(64)] public string Filler3;
    }

    /// <summary>
    /// Represents the User Trailer Record, concluding the transaction batch with totals.
    /// Conforms to the specification in Section 5.1.2.4, pages 22-23.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class EftUserTrailer001
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "001" or "020"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(2)] public string BankservRecordId;         // "92"
        [FieldFixedLength(4)] public string UserCode;
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(6)] public string LastSequenceNumber;
        [FieldFixedLength(6)] public string FirstActionDate;
        [FieldFixedLength(6)] public string LastActionDate;
        [FieldFixedLength(6)] public string NumberOfDebitRecords;
        [FieldFixedLength(6)] public string NumberOfCreditRecords;
        [FieldFixedLength(6)] public string NumberOfContraRecords;
        [FieldFixedLength(12)] public string TotalDebitValueInCents;
        [FieldFixedLength(12)] public string TotalCreditValueInCents;
        [FieldFixedLength(12)] public string HashTotalOfHomingAcctNos;
        // Filler length is calculated as 198 (total) - 88 (fields above) = 110
        [FieldFixedLength(110)] public string Filler;
    }

    /// <summary>
    /// Represents the Transmission Trailer record (the last line in the file).
    /// Conforms to the specification in Section 5.1.1.2, page 16.
    /// </summary>
    [FixedLengthRecord(FixedMode.AllowLessChars)]
    public class TransmissionTrailer999
    {
        [FieldFixedLength(3)] public string RecordIdentifier;         // "999"
        [FieldFixedLength(1)] public string DataSetStatus;            // 'L' or 'T'
        [FieldFixedLength(9)] public string NumberOfRecords;          // Total records in file, including header/trailer
                                                                      // Filler length is calculated as 198 (total) - 13 (fields above) = 185
        [FieldFixedLength(185)] public string Filler;
    }
}

