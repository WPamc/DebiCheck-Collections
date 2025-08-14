using System;
using System.Collections.Generic;
using System.IO;
using EFT_Collections;
using FileHelpers;

/// <summary>
/// Generates EFT collection file content and writes it to disk.
/// Logic previously embedded in <c>Program.cs</c> is now encapsulated here.
/// </summary>
public class EFTService
{
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
    public EFTService(DateTime deductionDate, string recordStatus = "T", string typeOfService = creditorDefaults.typeOfService)
    {
        _clientCode = creditorDefaults.clientCode;
        _clientName = creditorDefaults.clientName;
        _bankservUserCode = creditorDefaults.bankservUserCode;
        _creditorBranch = creditorDefaults.creditorBranch;
        _creditorAccount = creditorDefaults.creditorAccount;
        _creditorAbbreviation = creditorDefaults.creditorAbbreviation;
        if (DateTime.Now.Year == deductionDate.Year && DateTime.Now.Month == deductionDate.Month
            && DateTime.Now.Day >= deductionDate.Day)
        {
            _deductionDate = DateTime.Now.AddDays(1);
        }
        else
        {
            _deductionDate = deductionDate;
        }
       
        _typeOfService = typeOfService;
        _recordStatus = recordStatus;
    }

    /// <summary>
    /// Generates the full, multi-line string for an EFT file based on a list of transactions.
    /// When <paramref name="path"/> is provided the content is written to disk.
    /// </summary>
    public string GenerateFile(
        List<EftTransaction> transactions,
        int transmissionNumber,
        int userGenerationNumber,
        long startSequenceNumber,
        out long lastSequenceNumber,
        string? path = null,
        string? outputPath = null)
    {
        var records = new List<object>();
        long firstSequenceNumber = startSequenceNumber;
        long currentSequenceNumber = firstSequenceNumber;

        records.Add(CreateTransmissionHeader(transmissionNumber));

        records.Add(CreateUserHeader(_deductionDate, firstSequenceNumber, userGenerationNumber));

        decimal totalDebitValue = 0;
        long homingAccountHash = 0;

        foreach (var tx in transactions)
        {
            records.Add(CreateStandardTransaction(tx, currentSequenceNumber, _deductionDate));
            totalDebitValue += tx.Amount;
            homingAccountHash += long.Parse(tx.HomingAccountNumber);
            currentSequenceNumber++;
        }

        var contraRecord = CreateContraRecord(totalDebitValue, currentSequenceNumber, _deductionDate);
        records.Add(contraRecord);
        homingAccountHash += long.Parse(_creditorAccount);
        if (homingAccountHash.ToString().Length > 12)
        {
         
        }

        string hashAsString = homingAccountHash.ToString();
        string finalHashForFile = hashAsString;
        if (hashAsString.Length > 12)
        {
            // If the total is longer than 12 digits, take the last 12 characters.
            // This is equivalent to taking the "twelve least significant digits".
            finalHashForFile = hashAsString.Substring(hashAsString.Length - 12);
        }
        records.Add(CreateUserTrailer(firstSequenceNumber, currentSequenceNumber, _deductionDate, transactions.Count, totalDebitValue, finalHashForFile));

        records.Add(CreateTransmissionTrailer(records.Count + 1));

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
        string content = engine.WriteString(records.ToArray());
        if (path != null)
        {
            string directory = outputPath ?? AppContext.BaseDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(Path.Combine(directory, path), content);
        }
        return content;
    }


    private TransmissionHeader000 CreateTransmissionHeader(int transmissionNumber)
    {
        return new TransmissionHeader000
        {
            RecordIdentifier = "000",
            RecordStatus = _recordStatus,
            TransmissionDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd"), // _deductionDate.ToString("yyyyMMdd"),
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
            FirstActionDate = actionDate.ToString("yyMMdd"),
            LastActionDate = actionDate.ToString("yyMMdd"),
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
            ActionDate = actionDate.ToString("yyMMdd"),
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
            ActionDate = actionDate.ToString("yyMMdd"),
            EntryClass = "10",
            Filler1 = "0000",
            UserReference = $"{_creditorAbbreviation}       CONTRA".PadRight(30),
            Filler2 = "".PadRight(30),
            Filler3 = "".PadRight(64)
        };
    }

    private EftUserTrailer001 CreateUserTrailer(long firstSeq, long lastSeq, DateTime actionDate, int debitCount, decimal totalDebit, string hash)
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
            FirstActionDate = actionDate.ToString("yyMMdd"),
            LastActionDate = actionDate.ToString("yyMMdd"),
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

    /// <summary>
    /// Generates an EFT file using database data and writes it to disk.
    /// </summary>
    public static FileGenerationResult GenerateEFTFile(DateTime deductionDate, bool isTest, string outputPath)
    {
        if (DateTime.Now.Year == deductionDate.Year && DateTime.Now.Month == deductionDate.Month
            && DateTime.Now.Day >= deductionDate.Day)
        {
            deductionDate = DateTime.Now.AddDays(1);
        }
        
        string fileName = string.Empty;
        var writer = new EFTService(
            deductionDate: deductionDate,
            recordStatus: isTest?"T":"L"
        );

        var db = new DatabaseService();

        int generationNumber;
        int startSequenceNumber;
        if (isTest)
        {
            generationNumber = db.PeekGenerationNumber() + 1;
            startSequenceNumber = db.PeekDailyCounter(DateTime.Today) + 1;
        }
        else
        {
            generationNumber = db.GetNextGenerationNumber();
            startSequenceNumber = db.GetNextDailyCounter(DateTime.Today);
        }

        var collections = db.GetCollections(deductionDate);
        var transactionsToProcess = new List<EftTransaction>();
        foreach (var c in collections)
        {
            int.TryParse(c.AccountType, out int accType);
            transactionsToProcess.Add(new EftTransaction
            {
                HomingBranch = c.DebtorBankBranch,
                HomingAccountNumber = c.DebtorAccountNumber,
                HomingAccountName = c.DebtorName,
                AccountType = accType,
                Amount = c.InstructedAmount,
                UserReference = c.PaymentInformation + "_" + generationNumber.ToString()
            });
        }

        fileName = $"ZR{creditorDefaults.clientCode}.AUL.DATA.{DateTime.Now:yyMMdd.HHmmss}";
        int recordId = 0;
        if (!isTest)
        {
            decimal total = transactionsToProcess.Sum(t => t.Amount);
            recordId = db.CreateBankFileRecord(
                Path.GetFileName(fileName),
                generationNumber,
                startSequenceNumber,
                EftFileType.CollectionSubmission.ToString(),
                transactionsToProcess.Count,
                total);
        }
        writer.GenerateFile(
            transactionsToProcess,
            generationNumber,
            generationNumber,
            startSequenceNumber,
            out long lastSequenceNumber,
            fileName,
            outputPath);

        int inserted = 0;
        if (!isTest)
        {
            db.LinkFileToBankFile(recordId, Path.Combine(outputPath, fileName));
            inserted = db.InsertCollectionRequests(collections, recordId);
            db.InsertAcceptedResponses(collections, recordId);
            db.UpdateBankFileDailyCounterEnd(recordId, (int)lastSequenceNumber);
            db.SetDailyCounter(DateTime.Today, (int)lastSequenceNumber);
            db.MarkBankFileGenerationComplete(recordId);
        }
        Console.WriteLine($"EFT file written to {Path.Combine(outputPath, fileName)}");
        int bankFilesUpdated = isTest ? 0 : 1;
        return new FileGenerationResult(Path.Combine(outputPath, fileName), bankFilesUpdated, inserted);

    }
}
