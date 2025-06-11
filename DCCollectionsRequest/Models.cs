using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// A logical representation of a single transaction, combining all 3 lines.
public class RMCollectionTransaction
{
    // --- Line 01 Data ---
    public string L1_DataSetStatus { get; set; }
    public string L1_BankServUserCode { get; set; }
    public string L1_RecordSequenceNumber { get; set; }
    public string L1_InitiatingParty { get; set; }
    public string L1_PaymentInformation { get; set; }
    public string L1_RequestedCollectionDate { get; set; }
    public string L1_CreditorName { get; set; }
    public string L1_CreditorContactDetails { get; set; }
    public string L1_CreditorAbbreviatedShortName { get; set; }

    // --- Line 02 Data ---
    public string L2_CreditorEmail { get; set; }
    public string L2_CreditorAccountNumber { get; set; }
    public string L2_CreditorBankBranch { get; set; }
    public string L2_TrackingPeriod { get; set; }
    public string L2_DebitSequence { get; set; }
    public string L2_EntryClass { get; set; }
    public string L2_InstructedAmount { get; set; }
    public string L2_Currency { get; set; }
    public string L2_ChargeBearer { get; set; }
    public string L2_MandateReference { get; set; }
    public string L2_DebtorBankBranch { get; set; }

    // --- Line 03 Data ---
    public string L3_DebtorName { get; set; }
    public string L3_DebtorAccountNumber { get; set; }
    public string L3_AccountType { get; set; }
    public string L3_ContractReference { get; set; }
    public string L3_RelatedCycleDate { get; set; }

    // Helper property to easily access the sequence number for validation
    public int SequenceNumber => int.TryParse(L1_RecordSequenceNumber, out int seq) ? seq : -1;
}

// Wrapper class to hold the entire parsed file content
public class RMCollectionFile
{
    public TransmissionHeader000 TransmissionHeader { get; set; }
    public CollectionHeader080 CollectionHeader { get; set; }
    public List<RMCollectionTransaction> Transactions { get; set; } = new List<RMCollectionTransaction>();
    public CollectionTrailer080 CollectionTrailer { get; set; }
    public TransmissionTrailer999 TransmissionTrailer { get; set; }
}


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

[FixedLengthRecord] // 198 bytes (spec page 54)
public class CollectionTxLine02
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "080"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(2)] public string BankServRecordId;      // "08"
    [FieldFixedLength(4)] public string BankServUserCode;
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
    [FieldFixedLength(6)] public string DebtorBankBranch;
    [FieldFixedLength(14)] public string Filler;
}

[FixedLengthRecord] // 198 bytes (spec page 55)
public class CollectionTxLine03
{
    [FieldFixedLength(3)] public string RecordIdentifier;      // "080"
    [FieldFixedLength(1)] public string DataSetStatus;         // L / T
    [FieldFixedLength(2)] public string BankServRecordId;      // "08"
    [FieldFixedLength(4)] public string BankServUserCode;
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