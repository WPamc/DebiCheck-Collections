using System;
using System.Collections.Generic;
using System.IO;
using EFT_Collections;
using FileHelpers;

/// <summary>
/// Generates EFT collection file content and writes it to disk.
/// Logic previously embedded in <c>Program.cs</c> is now encapsulated here.
/// </summary>
public class RecordBuilder
{
    // Static Creditor Information
    private readonly string _clientCode;
    private readonly string _clientName;
    private readonly string _bankservUserCode;
    private readonly string _creditorBranch;
    private readonly string _creditorAccount;
    private readonly string _creditorAbbreviation;
    private readonly string _typeOfService;
    private readonly DateTime _deductionDate;
    private readonly string _recordStatus;

    /// <summary>
    /// Initializes the writer with the static details of the creditor.
    /// </summary>
    public RecordBuilder(string clientCode, string clientName, string bankservUserCode, string creditorBranch, string creditorAccount, string creditorAbbreviation, DateTime deductionDate, string recordStatus = "L", string typeOfService = "SAMEDAY")
    {
        _clientCode = clientCode;
        _clientName = clientName;
        _bankservUserCode = bankservUserCode;
        _creditorBranch = creditorBranch;
        _creditorAccount = creditorAccount;
        _creditorAbbreviation = creditorAbbreviation;
        _deductionDate = deductionDate;
        _typeOfService = typeOfService;
        _recordStatus = recordStatus;
    }

    /// <summary>
    /// Generates the full, multi-line string for an EFT file based on a list of transactions.
    /// </summary>
    public string GenerateFile(
        List<EftTransaction> transactions,
        int transmissionNumber,
        int userGenerationNumber,
        long startSequenceNumber,
        out long lastSequenceNumber)
    {
        var records = new List<object>();
        long firstSequenceNumber = startSequenceNumber;
        long currentSequenceNumber = firstSequenceNumber;

        // 1. Create Transmission Header
        records.Add(CreateTransmissionHeader(transmissionNumber));

        // 2. Create User Header
        records.Add(CreateUserHeader(_deductionDate, firstSequenceNumber, userGenerationNumber));

        // 3. Create Standard Transaction Records
        decimal totalDebitValue = 0;
        long homingAccountHash = 0;

        foreach (var tx in transactions)
        {
            records.Add(CreateStandardTransaction(tx, currentSequenceNumber, _deductionDate));
            totalDebitValue += tx.Amount;
            homingAccountHash += long.Parse(tx.HomingAccountNumber); // Simplified hash for example
            currentSequenceNumber++;
        }

        // 4. Create Contra Record
        var contraRecord = CreateContraRecord(totalDebitValue, currentSequenceNumber, _deductionDate);
        records.Add(contraRecord);
        homingAccountHash += long.Parse(_creditorAccount); // Add contra account to hash

        // 5. Create User Trailer
        records.Add(CreateUserTrailer(firstSequenceNumber, currentSequenceNumber, _deductionDate, transactions.Count, totalDebitValue, homingAccountHash));

        // 6. Create Transmission Trailer
        records.Add(CreateTransmissionTrailer(records.Count + 1)); // +1 to account for the trailer itself

        // Use FileHelpers MultiRecordEngine to write the objects to a string
        var engine = new MultiRecordEngine(
            typeof(TransmissionHeader000),
            typeof(EftUserHeader001),
            typeof(EftStandardTransaction001),
            typeof(EftContraRecord001),
            typeof(EftUserTrailer001),
            typeof(TransmissionTrailer999)
        )
        {
            RecordSelector = new RecordTypeSelector(CustomRecordSelector)
        };

        lastSequenceNumber = currentSequenceNumber;
        return engine.WriteString(records.ToArray());
    }

    /// <summary>
    /// Convenience method that writes the generated file to <paramref name="path"/>.
    /// </summary>
    public long WriteFile(
        string path,
        List<EftTransaction> transactions,
        int transmissionNumber,
        int userGenerationNumber,
        long startSequenceNumber)
    {
        string content = GenerateFile(
            transactions,
            transmissionNumber,
            userGenerationNumber,
            startSequenceNumber,
            out long lastSequenceNumber);
        File.WriteAllText(path, content);
        return lastSequenceNumber;
    }

    // --- Private Helper Methods for Record Creation ---

    private TransmissionHeader000 CreateTransmissionHeader(int transmissionNumber)
    {
        return new TransmissionHeader000
        {
            RecordIdentifier = "000",
            RecordStatus = _recordStatus,
            TransmissionDate = _deductionDate.ToString("yyyyMMdd"),
            ClientCode = _clientCode.PadLeft(5, '0'),
            ClientName = _clientName.PadRight(30),
            TransmissionNumber = transmissionNumber.ToString().PadLeft(7, '0'),
            Destination = "00000",
            ForLdUsersUse = "Billing SameDay".PadRight(20),
            Filler1 = "".PadRight(119)
        };
    }

    private EftUserHeader001 CreateUserHeader(DateTime actionDate, long firstSequence, int generationNumber)
    {
        return new EftUserHeader001
        {
            RecordIdentifier = "001",
            DataSetStatus = _recordStatus,
            BankservRecordId = "04",
            BankservUserCode = _bankservUserCode.PadRight(4),
            BankservCreationDate = _deductionDate.ToString("yyMMdd"),
            BankservPurgeDate = actionDate.AddDays(2).ToString("yyMMdd"),
            FirstActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            LastActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            FirstSequenceNumber = firstSequence.ToString().PadLeft(6, '0'),
            UserGenerationNumber = generationNumber.ToString().PadLeft(4, '0'),
            TypeOfService = _typeOfService.PadRight(10),
            AcceptedReport = "Y",
            AccountTypeCorrection = "Y",
            Filler = "".PadRight(142)
        };
    }

    private void WarnIfMissing(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine($"WARNING: Missing {fieldName} value for transaction.");
        }
    }

    private void WarnIfMissing(decimal value, string fieldName)
    {
        if (value == 0)
        {
            Console.WriteLine($"WARNING: {fieldName} is zero for transaction.");
        }
    }

    private EftStandardTransaction001 CreateStandardTransaction(EftTransaction tx, long sequenceNumber, DateTime actionDate)
    {
        WarnIfMissing(tx.HomingBranch, nameof(tx.HomingBranch));
        WarnIfMissing(tx.HomingAccountNumber, nameof(tx.HomingAccountNumber));
        WarnIfMissing(tx.HomingAccountName, nameof(tx.HomingAccountName));
        WarnIfMissing(tx.Amount, nameof(tx.Amount));
        WarnIfMissing(tx.UserReference, nameof(tx.UserReference));

        long amountInCents = (long)(tx.Amount * 100);
        return new EftStandardTransaction001
        {
            RecordIdentifier = "001",
            DataSetStatus = _recordStatus,
            BankservRecordId = "50",
            UserBranch = _creditorBranch.PadLeft(6, '0'),
            UserNominatedAccount = _creditorAccount.PadLeft(11, '0'),
            UserCode = _bankservUserCode.PadRight(4),
            UserSequenceNumber = sequenceNumber.ToString().PadLeft(6, '0'),
            HomingBranch = tx.HomingBranch.PadLeft(6, '0'),
            HomingAccountNumber = tx.HomingAccountNumber.PadLeft(11, '0'),
            TypeOfAccount = tx.AccountType.ToString(),
            AmountInCents = amountInCents.ToString().PadLeft(11, '0'),
            ActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            EntryClass = "36",
            TaxCode = "0",
            Filler1 = "",
            UserReference = tx.UserReference.PadRight(30),
            HomingAccountName = tx.HomingAccountName.PadRight(30),
            NonStandardHomingAccount = "".PadLeft(20, '0'),
            Filler2 = "".PadRight(16),
            HomingInstitution = "21",
            Filler3 = "".PadRight(26)
        };
    }

    private EftContraRecord001 CreateContraRecord(decimal totalAmount, long sequenceNumber, DateTime actionDate)
    {
        long amountInCents = (long)(totalAmount * 100);
        return new EftContraRecord001
        {
            RecordIdentifier = "001",
            DataSetStatus = _recordStatus,
            BankservRecordId = "52",
            UserBranch = _creditorBranch.PadLeft(6, '0'),
            UserNominatedAccount = _creditorAccount.PadLeft(11, '0'),
            UserCode = _bankservUserCode.PadRight(4),
            UserSequenceNumber = sequenceNumber.ToString().PadLeft(6, '0'),
            HomingBranch = _creditorBranch.PadLeft(6, '0'),
            HomingAccountNumber = _creditorAccount.PadLeft(11, '0'),
            TypeOfAccount = "1",
            AmountInCents = amountInCents.ToString().PadLeft(11, '0'),
            ActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            EntryClass = "10",
            Filler1 = "0000",
            UserReference = $"{_creditorAbbreviation}       CONTRA".PadRight(30),
            Filler2 = "".PadRight(30),
            Filler3 = "".PadRight(64)
        };
    }

    private EftUserTrailer001 CreateUserTrailer(long firstSeq, long lastSeq, DateTime actionDate, int debitCount, decimal totalDebit, long hash)
    {
        long totalDebitInCents = (long)(totalDebit * 100);
        return new EftUserTrailer001
        {
            RecordIdentifier = "001",
            DataSetStatus = _recordStatus,
            BankservRecordId = "92",
            UserCode = _bankservUserCode.PadRight(4),
            FirstSequenceNumber = firstSeq.ToString().PadLeft(6, '0'),
            LastSequenceNumber = lastSeq.ToString().PadLeft(6, '0'),
            FirstActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            LastActionDate = actionDate.AddDays(1).ToString("yyMMdd"),
            NumberOfDebitRecords = debitCount.ToString().PadLeft(6, '0'),
            NumberOfCreditRecords = "1".PadLeft(6, '0'),
            NumberOfContraRecords = "1".PadLeft(6, '0'),
            TotalDebitValueInCents = totalDebitInCents.ToString().PadLeft(12, '0'),
            TotalCreditValueInCents = totalDebitInCents.ToString().PadLeft(12, '0'),
            HashTotalOfHomingAcctNos = hash.ToString().PadLeft(12, '0'),
            Filler = "".PadRight(110)
        };
    }

    private TransmissionTrailer999 CreateTransmissionTrailer(int recordCount)
    {
        return new TransmissionTrailer999
        {
            RecordIdentifier = "999",
            DataSetStatus = _recordStatus,
            NumberOfRecords = recordCount.ToString().PadLeft(9, '0'),
            Filler = "".PadRight(185)
        };
    }

    private Type CustomRecordSelector(MultiRecordEngine engine, string recordLine)
    {
        if (recordLine.Length < 7) return null;

        string recId = recordLine.Substring(0, 3);
        if (recId == "000") return typeof(TransmissionHeader000);
        if (recId == "999") return typeof(TransmissionTrailer999);

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
