using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// A logical representation of a single transaction, combining all 3 lines.


// Wrapper class to hold the entire parsed file content



// --- FileHelpers Models for Parsing Individual Lines ---
// These should ideally be private nested classes within your file processing logic.

[FixedLengthRecord] // 198 bytes (spec page 14)
public class TransmissionHeader000
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "000"
    [FieldFixedLength(1)] public string RecordStatus;          // L / T
    [FieldFixedLength(8)] public string TransmissionDate;      // CCYYMMDD
    [FieldFixedLength(5)] public string UserCode;
    [FieldFixedLength(30)] public string UserName;
    [FieldFixedLength(7)] public string TransmissionNumber;
    [FieldFixedLength(5)] public string Destination;
    [FieldFixedLength(119)] public string Filler1;
    [FieldFixedLength(20)] public string ForLdUsersUse;
}

[FixedLengthRecord] // 198 bytes (spec page 52)
public class CollectionHeader080
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "080"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(2)] public string BankServRecordId;      // "04"
    [FieldFixedLength(4)] public string BankServUserCode;
    [FieldFixedLength(6)] public string FirstSequenceNumber;
    [FieldFixedLength(4)] public string UserGenerationNumber;
    [FieldFixedLength(8)] public string ServiceType;           // "COLLREQ"
    [FieldFixedLength(19)] public string CreationDateTime;
    [FieldFixedLength(15)] public string TotalTransactions;
    [FieldFixedLength(20)] public string PaymentInfoId;
    [FieldFixedLength(1)] public string AccountTypeCorrection; // Y / blank
    [FieldFixedLength(115)] public string Filler;
}

[FixedLengthRecord] // 198 bytes (spec page 53)
public class CollectionTxLine01
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "080"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(2)] public string BankServRecordId;      // "08"
    [FieldFixedLength(4)] public string BankServUserCode;
    [FieldFixedLength(6)] public string RecordSequenceNumber;
    [FieldFixedLength(2)] public string LineCount;             // "01"
    [FieldFixedLength(35)] public string InitiatingParty;
    [FieldFixedLength(35)] public string PaymentInformation;
    [FieldFixedLength(19)] public string RequestedCollectionDate;
    [FieldFixedLength(35)] public string CreditorName;
    [FieldFixedLength(30)] public string CreditorContactDetails;
    [FieldFixedLength(10)] public string CreditorAbbreviatedShortName;
    [FieldFixedLength(16)] public string Filler;
}

/// <summary>
/// Represents Line "02" of a Collection Request User Set Transaction Record.
/// Conforms to the specification on page 54.
/// </summary>
[FixedLengthRecord] // 198 bytes (spec page 54)
public class CollectionTxLine02
{
    [FieldFixedLength(2)] public string RecordIdentifier;      // "08"
    [FieldFixedLength(6)] public string RecordSequenceNumber;
    [FieldFixedLength(2)] public string LineCount;             // "02"
    [FieldFixedLength(90)] public string CreditorEmail;
    [FieldFixedLength(19)] public string CreditorAccountNumber;
    [FieldFixedLength(6)] public string CreditorBankBranch;
    [FieldFixedLength(2)] public string TrackingPeriod;
    [FieldFixedLength(4)] public string DebitSequence;         // FRST/RCUR/…
    [FieldFixedLength(4)] public string EntryClass;            // e.g. 0021
    [FieldFixedLength(14)] public string InstructedAmount;
    [FieldFixedLength(3)] public string Currency;              // "ZAR"
    [FieldFixedLength(4)] public string ChargeBearer;          // "SLEV"
    [FieldFixedLength(22)] public string MandateReference;
    [FieldFixedLength(6)] public string DebtorBankBranch;      // This field was missing in the original request's model for TxLine02 but is present in the spec for Line "02" (page 54, "Debtor Bank Branch Number")
    [FieldFixedLength(14)] public string Filler;
}

/// <summary>
/// Represents Line "03" of a Collection Request User Set Transaction Record.
/// Conforms to the specification on page 55.
/// </summary>
[FixedLengthRecord] // 198 bytes (spec page 55)
public class CollectionTxLine03
{
    [FieldFixedLength(2)] public string RecordIdentifier;      // "08"
    [FieldFixedLength(6)] public string RecordSequenceNumber;
    [FieldFixedLength(2)] public string LineCount;             // "03"
    [FieldFixedLength(35)] public string DebtorName;
    [FieldFixedLength(19)] public string DebtorAccountNumber;
    [FieldFixedLength(35)] public string AccountType;           // CURRENT/SAVINGS/TRANSMISSION
    [FieldFixedLength(14)] public string ContractReference;
    [FieldFixedLength(10)] public string RelatedCycleDate;      // YYYY-MM-DD
    [FieldFixedLength(75)] public string Filler;
}

[FixedLengthRecord] // 198 bytes (spec page 56)
public class CollectionTrailer080
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "080"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(2)] public string BankServRecordId;      // "92"
    [FieldFixedLength(4)] public string BankServUserCode;
    [FieldFixedLength(6)] public string FirstSequenceNumber;
    [FieldFixedLength(6)] public string LastSequenceNumber;
    [FieldFixedLength(15)] public string NumberOfCollectionRecords;
    [FieldFixedLength(18)] public string SumOfAcctNosAndAmounts;
    [FieldFixedLength(143)] public string Filler;
}

[FixedLengthRecord] // 198 bytes (spec page 15)
public class TransmissionTrailer999
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "999"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(9)] public string NumberOfRecords;
    [FieldFixedLength(185)] public string Filler;
}