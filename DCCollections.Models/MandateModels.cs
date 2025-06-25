using FileHelpers;

namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents a Mandate User Set Header Record. This structure is common for Initiation, Amendment, and Cancellation.
    /// Conforms to the specification on pages 18, 32, and 45.
    /// </summary>
    [FixedLengthRecord]
    public class MandateUserSetHeader080
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(4)] public string UserGenerationNumber;
        [FieldFixedLength(8)] public string ServiceType;
        [FieldFixedLength(1)] public string AccountTypeCorrection;
        [FieldFixedLength(169)] public string Filler;
    }

    /// <summary>
    /// Represents Line "01" of a Mandate Initiation User Set Transaction Record.
    /// Conforms to the specification on page 19.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTxLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(19)] public string CreationDateAndTime;
        [FieldFixedLength(35)] public string InitiatingParty;
        [FieldFixedLength(35)] public string ClientReference;
        [FieldFixedLength(14)] public string ContractReference;
        [FieldFixedLength(1)] public string TrackingIndicator;
        [FieldFixedLength(4)] public string InstalmentOccurrence;
        [FieldFixedLength(4)] public string InstalmentFrequency;
        [FieldFixedLength(10)] public string MandateInitiationDate;
        [FieldFixedLength(10)] public string FirstCollectionDate;
        [FieldFixedLength(3)] public string CollectionCurrency;
        [FieldFixedLength(14)] public string InstalmentAmount;
        [FieldFixedLength(3)] public string MaxCollectionCurrency;
        [FieldFixedLength(14)] public string MaximumCollectionAmount;
        [FieldFixedLength(14)] public string Filler;
    }

    /// <summary>
    /// Represents Line "02" of a Mandate Initiation User Set Transaction Record.
    /// Conforms to the specification on page 22.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTxLine02
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(11)] public string CreditorSchemeId;
        [FieldFixedLength(35)] public string CreditorName;
        [FieldFixedLength(30)] public string CreditorTelephoneContactDetails;
        [FieldFixedLength(90)] public string CreditorEmailContactDetails;
        [FieldFixedLength(6)] public string CreditorBankBranchNumber;
        [FieldFixedLength(16)] public string Filler;
    }

    /// <summary>
    /// Represents Line "03" of a Mandate Initiation User Set Transaction Record.
    /// Conforms to the specification on page 23.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTxLine03
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(19)] public string CreditorAccountNumber;
        [FieldFixedLength(35)] public string UltimateCreditorName;
        [FieldFixedLength(10)] public string CreditorAbbreviatedShortname;
        [FieldFixedLength(4)] public string EntryClass;
        [FieldFixedLength(35)] public string DebtorName;
        [FieldFixedLength(35)] public string DebtorIdentification;
        [FieldFixedLength(19)] public string DebtorAccountNumber;
        [FieldFixedLength(12)] public string DebtorAccountType;
        [FieldFixedLength(6)] public string DebtorBankBranchNumber;
        [FieldFixedLength(13)] public string Filler;
    }

    /// <summary>
    /// Represents Line "04" of a Mandate Initiation User Set Transaction Record.
    /// Conforms to the specification on page 25.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTxLine04
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(30)] public string DebtorTelephoneContactDetails;
        [FieldFixedLength(90)] public string DebtorEmailContactDetails;
        [FieldFixedLength(35)] public string UltimateDebtorName;
        [FieldFixedLength(2)] public string CollectionDay;
        [FieldFixedLength(1)] public string DateAdjustmentRuleIndicator;
        [FieldFixedLength(1)] public string AdjustmentCategory;
        [FieldFixedLength(8)] public string AdjustmentRate;
        [FieldFixedLength(3)] public string AdjustmentAmountCurrency;
        [FieldFixedLength(14)] public string AdjustmentAmount;
        [FieldFixedLength(4)] public string Filler;
    }

    /// <summary>
    /// Represents Line "05" of a Mandate Initiation User Set Transaction Record.
    /// Conforms to the specification on page 27.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTxLine05
    {
        [FieldFixedLength(2)] public string RecordIdentifier;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(3)] public string CollectionCurrency;
        [FieldFixedLength(14)] public string FirstCollectionAmount;
        [FieldFixedLength(11)] public string DebitValueType;
        [FieldFixedLength(10)] public string MandateReleaseDate;
        [FieldFixedLength(150)] public string Filler;
    }

    /// <summary>
    /// Represents a Mandate Initiation User Set Trailer Record.
    /// Conforms to the specification on page 27.
    /// </summary>
    [FixedLengthRecord]
    public class MandateInitiationTrailer080
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string FirstSequenceNumber;
        [FieldFixedLength(6)] public string LastSequenceNumber;
        [FieldFixedLength(12)] public string NumberOfMandateInitiationRecords;
        [FieldFixedLength(164)] public string Filler;
    }

    /// <summary>
    /// Represents Line "01" of a Mandate Amendment User Set Transaction Record.
    /// Conforms to the specification on page 33.
    /// </summary>
    [FixedLengthRecord]
    public class MandateAmendmentTxLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(19)] public string CreationDateAndTime;
        [FieldFixedLength(35)] public string InitiatingParty;
        [FieldFixedLength(4)] public string AmendmentReason;
        [FieldFixedLength(35)] public string ClientReference;
        [FieldFixedLength(14)] public string ContractReference;
        [FieldFixedLength(1)] public string TrackingIndicator;
        [FieldFixedLength(4)] public string InstalmentOccurrence;
        [FieldFixedLength(10)] public string FirstCollectionDate;
        [FieldFixedLength(3)] public string CollectionCurrency;
        [FieldFixedLength(14)] public string InstalmentAmount;
        [FieldFixedLength(35)] public string OriginalDebtorName;
        [FieldFixedLength(4)] public string DebtorAuthenticationRequired;
        [FieldFixedLength(2)] public string Filler;
    }

    /// <summary>
    /// Represents Line "01" of a Mandate Cancellation User Set Transaction Record.
    /// Conforms to the specification on page 45-46.
    /// </summary>
    [FixedLengthRecord]
    public class MandateCancellationTxLine01
    {
        [FieldFixedLength(3)] public string RecordIdentifier;
        [FieldFixedLength(1)] public string DataSetStatus;
        [FieldFixedLength(2)] public string BankServRecordIdentifier;
        [FieldFixedLength(4)] public string BankServUserCode;
        [FieldFixedLength(6)] public string RecordSequenceNumber;
        [FieldFixedLength(2)] public string LineCount;
        [FieldFixedLength(19)] public string CreationDateAndTime;
        [FieldFixedLength(35)] public string InitiatingParty;
        [FieldFixedLength(4)] public string CancellationReason;
        [FieldFixedLength(35)] public string ClientReference;
        [FieldFixedLength(14)] public string ContractReference;
        [FieldFixedLength(1)] public string TrackingCancellationIndicator;
        [FieldFixedLength(35)] public string CreditorName;
        [FieldFixedLength(30)] public string CreditorTelephoneContactDetails;
        [FieldFixedLength(7)] public string Filler;
    }
}
