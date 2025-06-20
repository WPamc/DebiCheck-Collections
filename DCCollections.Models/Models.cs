using FileHelpers;

namespace RMCollectionProcessor.Models
{
    [FixedLengthRecord]
    public class TransmissionHeader000
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string RecordStatus;
        [FieldFixedLength(8)] public string TransmissionDate;
        [FieldFixedLength(5)] public string UserCode;
        [FieldFixedLength(30)] public string UserName;
        [FieldFixedLength(7)] public string TransmissionNumber;
        [FieldFixedLength(5)] public string Destination;
        [FieldFixedLength(119)] public string Filler1;
        [FieldFixedLength(20)] public string ForLdUsersUse;
    }

    [FixedLengthRecord]
    public class CollectionHeader080
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordId;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(4)] public string UserGenerationNumber;
        [FieldFixedLength(8)] public string ServiceType;
        [FieldFixedLength(19)] public string CreationDateTime;
        [FieldFixedLength(15)] public string TotalTransactions;
        [FieldFixedLength(20)] public string PaymentInfoId;
        [FieldFixedLength(1)] public string AccountTypeCorrection;
        [FieldFixedLength(115)] public string Filler;
    }

    [FixedLengthRecord]
    public class CollectionTxLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordId;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
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
    [FixedLengthRecord]
    public class CollectionTxLine02
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(90)] public string CreditorEmail;
        [FieldFixedLength(19)] public string CreditorAccountNumber;
        [FieldFixedLength(6)] public string CreditorBankBranch;
        [FieldFixedLength(2)] public string TrackingPeriod;
        [FieldFixedLength(4)] public string DebitSequence;
        [FieldFixedLength(4)] public string EntryClass;
        [FieldFixedLength(14)] public string InstructedAmount;
        [FieldFixedLength(3)] public string Currency;
        [FieldFixedLength(4)] public string ChargeBearer;
        [FieldFixedLength(22)] public string MandateReference;
        [FieldFixedLength(6)] public string DebtorBankBranch;
        [FieldFixedLength(14)] public string Filler;
    }

    /// <summary>
    /// Represents Line "03" of a Collection Request User Set Transaction Record.
    /// Conforms to the specification on page 55.
    /// </summary>
    [FixedLengthRecord]
    public class CollectionTxLine03
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(35)] public string DebtorName;
        [FieldFixedLength(19)] public string DebtorAccountNumber;
        [FieldFixedLength(35)] public string AccountType;
        [FieldFixedLength(14)] public string ContractReference;
        [FieldFixedLength(10)] public string RelatedCycleDate;
        [FieldFixedLength(75)] public string Filler;
    }

    [FixedLengthRecord]
    public class CollectionTrailer080
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordId;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(6)] public string LastSequenceNumber;
        [FieldFixedLength(15)] public string NumberOfCollectionRecords;
        [FieldFixedLength(18)] public string SumOfAcctNosAndAmounts;
        [FieldFixedLength(143)] public string Filler;
    }

    [FixedLengthRecord]
    public class TransmissionTrailer999
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(9)] public string NumberOfRecords;
        [FieldFixedLength(185)] public string Filler;
    }


    /// <summary>
    /// Represents a Status User Set Header Record for output files.
    /// Conforms to the specification on page 59 (3.8.1).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetHeader080
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataStatus;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(7)] public string BankServUserCodeGenerationNumber;
        [FieldFixedLength(34)] public string RmService;
        [FieldFixedLength(149)] public string Filler;
    }

    /// <summary>
    /// Represents Line "01" of a Status User Set Header for output files.
    /// Conforms to the specification on page 59 (3.8.2).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetHeaderLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(8)] public string TransmissionDate;
        [FieldFixedLength(5)] public string UserCode;
        [FieldFixedLength(7)] public string TransmissionNumber;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(4)] public string UserGenerationNumber;
        [FieldFixedLength(6)] public string MemberNumberOfDebtorAgent;
        [FieldFixedLength(6)] public string MemberNumberOfCreditorAgent;
        [FieldFixedLength(34)] public string OriginalMessageId;
        [FieldFixedLength(6)] public string GroupLevelRejectReasonCode;
        [FieldFixedLength(15)] public string OriginalNumberOfDirectDebits;
        [FieldFixedLength(4)] public string GroupLevelStatus;
        [FieldFixedLength(4)] public string GroupLevelStatusCode;
        [FieldFixedLength(87)] public string Filler;
    }

    /// <summary>
    /// Represents Line "02" of a Status User Set Header for output files.
    /// Conforms to the specification on page 60 (3.8.3).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetHeaderLine02
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(135)] public string GroupLevelErrorCodeDescription;
        [FieldFixedLength(58)] public string Filler;
    }

    /// <summary>
    /// Represents Line "01" of a Status User Set Transaction for output files.
    /// Conforms to the specification on page 61 (3.8.4).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetTransactionLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(4)] public string UserGenerationNumber;
        [FieldFixedLength(23)] public string OriginalPmtInfId;
        [FieldFixedLength(2)] public string Filler1;
        [FieldFixedLength(35)] public string OriginalEndToEndTransactionIdentifier;
        [FieldFixedLength(4)] public string TransactionStatus;
        [FieldFixedLength(35)] public string CreditorNameNotApplicable;
        [FieldFixedLength(4)] public string Filler2;
        [FieldFixedLength(14)] public string InstructedAmount;
        [FieldFixedLength(3)] public string TransactionCurrency;
        [FieldFixedLength(4)] public string SequenceType;
        [FieldFixedLength(22)] public string MandateReferenceNumber;
        [FieldFixedLength(14)] public string ContractReferenceNumber;
        [FieldFixedLength(10)] public string AbbreviatedUltimateCreditorShortName;
        [FieldFixedLength(4)] public string EntryClassValue;
        [FieldFixedLength(2)] public string Filler3;
    }

    /// <summary>
    /// Represents Line "02" of a Status User Set Transaction for output files.
    /// Conforms to the specification on page 62 (3.8.5).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetTransactionLine02
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(10)] public string CollectionDate;
        [FieldFixedLength(10)] public string ActionDate;
        [FieldFixedLength(140)] public string Unstructured;
        [FieldFixedLength(8)] public string EffectiveDate;
        [FieldFixedLength(25)] public string Filler;
    }

    /// <summary>
    /// Represents Line "03" of a Status User Set Transaction for output files.
    /// Conforms to the specification on page 63 (3.8.6).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetTransactionLine03
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(35)] public string DebtorName;
        [FieldFixedLength(19)] public string DebtorAccount;
        [FieldFixedLength(6)] public string MemberIdOfDebtorBank;
        [FieldFixedLength(6)] public string MemberIdOfCreditorBank;
        [FieldFixedLength(35)] public string CreditorName;
        [FieldFixedLength(30)] public string CreditorContractDetails;
        [FieldFixedLength(62)] public string Filler;
    }

    /// <summary>
    /// Represents Line "04" of a Status User Set Transaction for output files.
    /// Conforms to the specification on page 63 (3.8.7).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetTransactionLine04
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(90)] public string EmailId;
        [FieldFixedLength(19)] public string CreditorAccount;
        [FieldFixedLength(1)] public string ErrorRecordPresent;
        [FieldFixedLength(83)] public string Filler;
    }

    /// <summary>
    /// Represents an "085" Error Record for a Status User Set Transaction.
    /// Conforms to the specification on page 63 (3.8.8).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetErrorRecord085
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(6)] public string TransactionLevelRejectReasonCode;
        [FieldFixedLength(135)] public string TransactionLevelErrorCodeDescription;
        [FieldFixedLength(39)] public string Filler;
    }

    /// <summary>
    /// Represents a Status User Set Trailer Record for output files.
    /// Conforms to the specification on page 64 (3.8.9).
    /// </summary>
    [FixedLengthRecord]
    public class StatusUserSetTrailer084
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(12)] public string NumberOfRmRecordsInStatusReport;
        [FieldFixedLength(182)] public string Filler;
    }

    /// <summary>
    /// Represents a Transmission Status record from a Reply File.
    /// Conforms to the specification on page 57 (3.7.1).
    /// </summary>
    [FixedLengthRecord]
    public class ReplyTransmissionStatus900
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string Status;
        [FieldFixedLength(3)] public string TransmissionIdentifier;
        [FieldFixedLength(1)] public string Filler1;
        [FieldFixedLength(12)] public string Filler2;
        [FieldFixedLength(1)] public string Filler3;
        [FieldFixedLength(5)] public string ElectronicBankingSuiteUserCode;
        [FieldFixedLength(1)] public string Filler4;
        [FieldFixedLength(7)] public string TransmissionNumber;
        [FieldFixedLength(1)] public string Filler5;
        [FieldFixedLength(8)] public string TransmissionStatus;
        [FieldFixedLength(155)] public string Filler6;
    }

    /// <summary>
    /// Represents a User Set Status record from a Reply File.
    /// Conforms to the specification on page 57 (3.7.2).
    /// </summary>
    [FixedLengthRecord]
    public class ReplyUserSetStatus900
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string Status;
        [FieldFixedLength(3)] public string ServiceIndicator;
        [FieldFixedLength(1)] public string Filler1;
        [FieldFixedLength(13)] public string Filler2;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(1)] public string Filler3;
        [FieldFixedLength(7)] public string BankServUserCodeGenerationNumber;
        [FieldFixedLength(1)] public string Filler4;
        [FieldFixedLength(6)] public string LastSequenceNumber;
        [FieldFixedLength(1)] public string Filler5;
        [FieldFixedLength(8)] public string UserSetStatus;
        [FieldFixedLength(1)] public string Filler6;
        [FieldFixedLength(1)] public string Filler7;
        [FieldFixedLength(1)] public string Filler8;
        [FieldFixedLength(8)] public string DebiCheckService;
        [FieldFixedLength(138)] public string Filler9;
    }

    /// <summary>
    /// Represents a Rejected Message record from a Reply File.
    /// Conforms to the specification on page 58 (3.7.3).
    /// </summary>
    [FixedLengthRecord]
    public class ReplyRejectedMessage901
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string Status;
        [FieldFixedLength(3)] public string ServiceIndicator;
        [FieldFixedLength(1)] public string Filler1;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(1)] public string Filler2;
        [FieldFixedLength(7)] public string BankServUserCodeGenerationNumber;
        [FieldFixedLength(1)] public string Filler3;
        [FieldFixedLength(6)] public string UserSequenceNumber;
        [FieldFixedLength(1)] public string Filler4;
        [FieldFixedLength(5)] public string ErrorCode;
        [FieldFixedLength(1)] public string Filler5;
        [FieldFixedLength(115)] public string ErrorMessage;
        [FieldFixedLength(5)] public string Filler6;
        [FieldFixedLength(35)] public string ContractRefNumber;
        [FieldFixedLength(9)] public string Filler7;
    }

    /// <summary>
    /// Represents a Transmission Reject Reason record from a Reply File.
    /// Conforms to the specification on page 58 (3.7.4).
    /// </summary>
    [FixedLengthRecord]
    public class ReplyTransmissionRejectReason901
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string Status;
        [FieldFixedLength(3)] public string HeaderRecordIdentifier;
        [FieldFixedLength(1)] public string Filler1;
        [FieldFixedLength(5)] public string ErrorCode;
        [FieldFixedLength(1)] public string Filler2;
        [FieldFixedLength(50)] public string ErrorMessage;
        [FieldFixedLength(134)] public string Filler3;
    }
}