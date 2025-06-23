using System;
using System.Collections.Generic;
using RMCollectionProcessor.Models;

/// <summary>
/// Holds all the static, boilerplate information required for an RM Collection file batch.
/// This object is created once per file and provides default data to all record builders.
/// </summary>
public class StaticDataProvider
{
    // --- Transmission Level Data ---
    public string RecordStatus { get; }
    public string TransmissionDate { get; }
    public string UserCode { get; }
    public string UserName { get; }
    public string TransmissionNumber { get; }
    public string Destination { get; }
    public string ForLdUsersUse { get; }

    // --- User-Set Level Data ---
    public string BankServUserCode { get; }
    public string UserGenerationNumber { get; }
    public string CreationDateTime { get; }
    public string PaymentInfoId { get; }

    // --- Creditor (Your Company) Data ---
    public string InitiatingParty { get; }
    public string CreditorName { get; }
    public string CreditorContactDetails { get; }
    public string CreditorAbbreviatedShortName { get; }
    public string CreditorEmail { get; }
    public string CreditorAccountNumber { get; }
    public string CreditorBankBranch { get; }

    public StaticDataProvider(
        string recordStatus,
        string transmissionNumber,
        string userGenerationNumber,
        string paymentInfoId,
        CreditorDefaults creditorDefaults
        )
    {
        // Transmission Info
        RecordStatus = recordStatus; // "L" or "T"
        TransmissionDate = DateTime.Now.ToString("yyyyMMdd");
        UserCode = creditorDefaults.UserCode;
        UserName = creditorDefaults.UserName.PadRight(30);

        TransmissionNumber = transmissionNumber.PadLeft(7, '0');
        Destination = "00000";
        ForLdUsersUse = "Billing DC".PadRight(20);

        // User-Set Info
        BankServUserCode = creditorDefaults.BankServUserCode;
        UserGenerationNumber = userGenerationNumber.PadLeft(4, '0');
        CreationDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        PaymentInfoId = paymentInfoId.PadRight(20);

        // Creditor Info
        InitiatingParty = creditorDefaults.InitiatingParty.PadRight(35);
        CreditorName = creditorDefaults.CreditorName.PadRight(35);
        CreditorContactDetails = creditorDefaults.CreditorContactDetails.PadRight(30);
        CreditorAbbreviatedShortName = creditorDefaults.CreditorAbbreviatedShortName.PadRight(10);
        CreditorEmail = creditorDefaults.CreditorEmail.PadRight(90);
        CreditorAccountNumber = creditorDefaults.CreditorAccountNumber.PadLeft(19, '0');
        CreditorBankBranch = creditorDefaults.CreditorBankBranch.PadLeft(6, '0');
    }
}

/// <summary>
/// Builds FileHelpers record objects for an RM Collections request.
/// </summary>
public class RecordBuilder
{
    /// <summary>
    /// Creates the Transmission Header record (000).
    /// </summary>
    public TransmissionHeader000 BuildTransmissionHeader(StaticDataProvider staticData)
    {
        return new TransmissionHeader000
        {
            RecordIdentifier = "000",
            RecordStatus = staticData.RecordStatus,
            TransmissionDate = staticData.TransmissionDate,
            UserCode = staticData.UserCode,
            UserName = staticData.UserName,
            TransmissionNumber = staticData.TransmissionNumber,
            Destination = staticData.Destination,
            Filler1 = new string(' ', 119),
            ForLdUsersUse = staticData.ForLdUsersUse
        };
    }

    /// <summary>
    /// Creates the Collection User-Set Header record (080/04).
    /// </summary>
    public CollectionHeader080 BuildCollectionHeader(StaticDataProvider staticData, int firstSequenceNumber 
                                                        , int totalTransactions)
    {
        return new CollectionHeader080
        {
            RecordIdentifier = "080",
            DataSetStatus = staticData.RecordStatus,
            BankServRecordId = "04",
            BankServUserCode = staticData.BankServUserCode,
            FirstSequenceNumber = firstSequenceNumber.ToString().PadLeft(6, '0'),
            UserGenerationNumber = staticData.UserGenerationNumber,
            ServiceType = "COLLREQ".PadRight(8),
            CreationDateTime = staticData.CreationDateTime,
            TotalTransactions = totalTransactions.ToString().PadLeft(15, '0'),
            
            PaymentInfoId = staticData.PaymentInfoId,
            AccountTypeCorrection = "Y",
            Filler = new string(' ', 115)
        };
    }

    /// <summary>
    /// Creates all three lines for a single RM Collection Transaction.
    /// </summary>
    public (CollectionTxLine01, CollectionTxLine02, CollectionTxLine03) BuildTransactionLines(
      StaticDataProvider staticData,
      DebtorCollectionData debtorData,
      int sequenceNumber)
    {
        string seqStr = sequenceNumber.ToString().PadLeft(6, '0');

        // Line 1 is already correct
        var line1 = new CollectionTxLine01
        {
            RecordIdentifier = "080",
            DataSetStatus = staticData.RecordStatus,
            BankServRecordId = "08",
            BankServUserCode = staticData.BankServUserCode,
            RecordSequenceNumber = seqStr,
            LineCount = "01",
            InitiatingParty = staticData.InitiatingParty,
            PaymentInformation = debtorData.PaymentInformation.PadRight(35),
            RequestedCollectionDate = debtorData.RequestedCollectionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
            CreditorName = staticData.CreditorName,
            CreditorContactDetails = staticData.CreditorContactDetails,
            CreditorAbbreviatedShortName = staticData.CreditorAbbreviatedShortName,
            Filler = new string(' ', 16)
        };

        // Corrected Line 2
        var line2 = new CollectionTxLine02
        {
            RecordIdentifier = "08", 
            RecordSequenceNumber = seqStr,
            LineCount = "02",
            CreditorEmail = staticData.CreditorEmail,
            CreditorAccountNumber = staticData.CreditorAccountNumber,
            CreditorBankBranch = staticData.CreditorBankBranch,
            TrackingPeriod = debtorData.TrackingPeriod.ToString("D2"),
            DebitSequence = debtorData.DebitSequence.PadRight(4),
            EntryClass = debtorData.EntryClass.PadLeft(4, '0'),
            InstructedAmount = ((long)(debtorData.InstructedAmount * 100)).ToString().PadLeft(14, '0'),
            Currency = "ZAR",
            ChargeBearer = "SLEV".PadRight(4),
            MandateReference = debtorData.MandateReference.PadRight(22),
            DebtorBankBranch = debtorData.DebtorBankBranch.PadLeft(6, '0'),
            Filler = new string(' ', 14)
        };

        // Corrected Line 3
        var line3 = new CollectionTxLine03
        {
            RecordIdentifier = "08", 
            RecordSequenceNumber = seqStr,
            LineCount = "03",
            DebtorName = debtorData.DebtorName.PadRight(35),
            DebtorAccountNumber = debtorData.DebtorAccountNumber.PadLeft(19, '0'),
            AccountType = debtorData.AccountType.PadRight(35),
            ContractReference = debtorData.ContractReference.PadRight(14),
            RelatedCycleDate = debtorData.RelatedCycleDate.ToString("yyyy-MM-dd"),
            Filler = new string(' ', 75)
        };

        return (line1, line2, line3);
    }

    /// <summary>
    /// Creates the Collection User-Set Trailer record (080/92).
    /// </summary>
    public CollectionTrailer080 BuildCollectionTrailer(StaticDataProvider staticData, List<DebtorCollectionData> transactions, int firstSeq, int lastSeq)
    {
        long hashTotal = 0;
        foreach (var tx in transactions)
        {
            if (long.TryParse(tx.DebtorAccountNumber, out long acct))
                hashTotal += acct;
            hashTotal += (long)(tx.InstructedAmount * 100);
        }
        string hashStr = hashTotal.ToString();
        string finalHash = hashStr.Length > 18 ? hashStr[^18..] : hashStr;

        return new CollectionTrailer080
        {
            RecordIdentifier = "080",
            DataSetStatus = staticData.RecordStatus,
            BankServRecordId = "92",
            BankServUserCode = staticData.BankServUserCode,
            FirstSequenceNumber = firstSeq.ToString().PadLeft(6, '0'),
            LastSequenceNumber = lastSeq.ToString().PadLeft(6, '0'),
            NumberOfCollectionRecords = transactions.Count.ToString().PadLeft(15, '0'),
            SumOfAcctNosAndAmounts = finalHash.PadLeft(18, '0'),
            Filler = new string(' ', 143)
        };
    }

    /// <summary>
    /// Creates the Transmission Trailer record (999).
    /// </summary>
    public TransmissionTrailer999 BuildTransmissionTrailer(StaticDataProvider staticData, int totalRecordCount)
    {
        return new TransmissionTrailer999
        {
            RecordIdentifier = "999",
            DataSetStatus = staticData.RecordStatus,
            NumberOfRecords = totalRecordCount.ToString().PadLeft(9, '0'),
            Filler = new string(' ', 185)
        };
    }
}

/// <summary>
/// Helper class to hold the dynamic debtor-specific data used when creating a file.
/// </summary>
