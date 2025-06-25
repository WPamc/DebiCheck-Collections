using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DbConnection;
using System.IO;
using System.Linq;
using RMCollectionProcessor.Models;
using RMCollectionProcessor;
using System.Globalization;

public class DatabaseService
{
    private const string CounterDescription = "SANLAM MULTIDATA";
    private readonly string _connectionString;
    private readonly string _collectionsSql;
    private readonly string _creditorDefaultsSql;

    public DatabaseService()
    {
        var configuration = AppConfig.Configuration;
        _connectionString = AppConfig.ConnectionString;
        var queriesPath = configuration["SqlQueriesPath"] ?? "SqlQueries";
        _collectionsSql = File.ReadAllText(Path.Combine(queriesPath, "Collections.sql"));
        _creditorDefaultsSql = File.ReadAllText(Path.Combine(queriesPath, "CreditorDefaults.sql"));
    }

    public List<DebtorCollectionData> GetCollections(int deductionDay)
    {
        var results = new List<DebtorCollectionData>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(_collectionsSql, conn);
        cmd.Parameters.Add(new SqlParameter("@DEDUCTIONDAY", SqlDbType.Int) { Value = deductionDay });
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var data = new DebtorCollectionData
            {
                PaymentInformation = reader[nameof(DebtorCollectionData.PaymentInformation)].ToString() ?? string.Empty,
                RequestedCollectionDate = reader.GetDateTime(reader.GetOrdinal(nameof(DebtorCollectionData.RequestedCollectionDate))),
                TrackingPeriod = reader.GetInt32(reader.GetOrdinal(nameof(DebtorCollectionData.TrackingPeriod))),
                DebitSequence = reader[nameof(DebtorCollectionData.DebitSequence)].ToString() ?? string.Empty,
                EntryClass = reader[nameof(DebtorCollectionData.EntryClass)].ToString() ?? string.Empty,
                InstructedAmount = Convert.ToDecimal(reader["InstructedAmount"]),
                MandateReference = reader[nameof(DebtorCollectionData.MandateReference)].ToString() ?? string.Empty,
                DebtorBankBranch = reader[nameof(DebtorCollectionData.DebtorBankBranch)].ToString() ?? string.Empty,
                DebtorName = reader[nameof(DebtorCollectionData.DebtorName)].ToString() ?? string.Empty,
                DebtorAccountNumber = reader[nameof(DebtorCollectionData.DebtorAccountNumber)].ToString() ?? string.Empty,
                AccountType = reader[nameof(DebtorCollectionData.AccountType)].ToString() ?? string.Empty,
                ContractReference = reader[nameof(DebtorCollectionData.ContractReference)].ToString() ?? string.Empty,
                RelatedCycleDate = reader.GetDateTime(reader.GetOrdinal(nameof(DebtorCollectionData.RelatedCycleDate)))
            };
            results.Add(data);
        }
        return results;
    }

    public DataTable GetDuplicateCollections(int deductionDay)
    {
        var collections = GetCollections(deductionDay);
        var table = new DataTable();
        table.Columns.Add("ContractReference", typeof(string));
        table.Columns.Add("RequestedCollectionDate", typeof(DateTime));
        table.Columns.Add("PaymentInformation", typeof(string));
        table.Columns.Add("InstructedAmount", typeof(decimal));

        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        foreach (var c in collections)
        {
            using var cmd = new SqlCommand(@"SELECT count(*)
  FROM [DRC].[dbo].[BILLING_COLLECTIONREQUESTS]
  WHERE REFERENCE = @REFERENCE AND YEAR(DATEREQUESTED) = YEAR(@DATEREQUESTED)
    AND MONTH(DATEREQUESTED) = MONTH(@DATEREQUESTED)", conn);
            cmd.Parameters.Add(new SqlParameter("@REFERENCE", SqlDbType.VarChar, 23) { Value = c.ContractReference });
            cmd.Parameters.Add(new SqlParameter("@DATEREQUESTED", SqlDbType.DateTime) { Value = c.RequestedCollectionDate });
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)
            {
                table.Rows.Add(c.ContractReference, c.RequestedCollectionDate, c.PaymentInformation, c.InstructedAmount);
            }
        }

        return table;
    }

    public CreditorDefaults GetCreditorDefaults(int creditorId)
    {
        return new CreditorDefaults();
    }

    private int GetNextCounter(string subclass1, string? subclass2)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        using var cmd = new SqlCommand(@"SET NOCOUNT ON;
UPDATE dbo.EDI_GENERICCOUNTERS
   SET COUNTER = COUNTER + 1,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 OUTPUT INSERTED.COUNTER
 WHERE DESCRIPTION = @desc
   AND SUBCLASS1 = @sub1
   AND ((@sub2 IS NULL AND SUBCLASS2 IS NULL) OR SUBCLASS2 = @sub2);

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.EDI_GENERICCOUNTERS
        (DESCRIPTION, SUBCLASS1, SUBCLASS2, COUNTER, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
    VALUES (@desc, @sub1, @sub2, 1, 99, GETDATE(), 99, GETDATE());
    SELECT 1;
END", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@sub1", SqlDbType.VarChar, 100) { Value = subclass1 });
        cmd.Parameters.Add(new SqlParameter("@sub2", SqlDbType.VarChar, 100) { Value = (object?)subclass2 ?? DBNull.Value });

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    private int GetCurrentCounter(string subclass1, string? subclass2)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        using var cmd = new SqlCommand(@"SELECT COUNTER
  FROM dbo.EDI_GENERICCOUNTERS
 WHERE DESCRIPTION = @desc
   AND SUBCLASS1 = @sub1
   AND ((@sub2 IS NULL AND SUBCLASS2 IS NULL) OR SUBCLASS2 = @sub2);", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@sub1", SqlDbType.VarChar, 100) { Value = subclass1 });
        cmd.Parameters.Add(new SqlParameter("@sub2", SqlDbType.VarChar, 100) { Value = (object?)subclass2 ?? DBNull.Value });

        var result = cmd.ExecuteScalar();
        return result == null ? 0 : Convert.ToInt32(result);
    }

    public int GetNextGenerationNumber()
        => GetNextCounter("DC GENERATIONNUMBER", null);
    public int GetCurrentGenerationNumber()
        => GetCurrentCounter("DC GENERATIONNUMBER", null);
    public int GetNextDailyCounter(DateTime date)
        => GetNextCounter("DC DAILYCOUNTER", date.ToString("yyyy-MM-dd"));
    public int GetCurrentDailyCounter(DateTime date)
        => GetCurrentCounter("DC DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

    public int CreateBankFileRecord(string fileName, int generationNumber, int dailyCounterStart)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        using var cmd = new SqlCommand(@"INSERT INTO dbo.EDI_BANKFILES
                (DESCRIPTION, FILENAME, GENERATIONNUMBER, DAILYCOUNTERSTART, DAILYCOUNTEREND,
                 GENERATIONCOMPLETE, DELIVERED, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
             VALUES (@desc, @file, @gen, @start, @start, 0, 0, 99, GETDATE(), 99, GETDATE());
             SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.VarChar, 100) { Value = fileName });
        cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.Int) { Value = generationNumber });
        cmd.Parameters.Add(new SqlParameter("@start", SqlDbType.Int) { Value = dailyCounterStart });

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public int GetBankFileRowId(string fileName)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"SELECT ROWID FROM dbo.EDI_BANKFILES WHERE FILENAME = @file", conn);
        cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.VarChar, 100) { Value = fileName });
        conn.Open();
        var result = cmd.ExecuteScalar();
        return result == null ? 0 : Convert.ToInt32(result);
    }

    public void UpdateBankFileDailyCounterEnd(int rowId, int dailyCounterEnd)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANKFILES
   SET DAILYCOUNTEREND = @end,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        cmd.Parameters.Add(new SqlParameter("@end", SqlDbType.Int) { Value = dailyCounterEnd });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void MarkBankFileGenerationComplete(int rowId)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANKFILES
   SET GENERATIONCOMPLETE = 1,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void MarkBankFileDelivered(int rowId)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANKFILES
   SET DELIVERED = 1,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Updates the status of an existing bank file record using the generation number.
    /// </summary>
    /// <param name="generationNumber">The generation number of the bank file.</param>
    /// <param name="status">The status to apply.</param>
    public void UpdateBankFileStatus(int generationNumber, BankFileStatus status)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE [DRC].[dbo].[EDI_BANKFILES]
   SET STATUS = @STATUS
 WHERE [EDI_BANKFILES].GENERATIONNUMBER = @GENERATIONNUMBER;", conn);
        cmd.Parameters.Add(new SqlParameter("@STATUS", SqlDbType.VarChar, 4) { Value = ((int)status).ToString() });
        cmd.Parameters.Add(new SqlParameter("@GENERATIONNUMBER", SqlDbType.Int) { Value = generationNumber });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public BillingCollectionRequest? GetCollectionRequestByReference(string reference)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"SELECT ROWID, DATEREQUESTED, SUBSSN, REFERENCE,
                DEDUCTIONREFERENCE, AMOUNTREQUESTED, RESULT, METHOD,
                CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE
           FROM dbo.BILLING_COLLECTIONREQUESTS
          WHERE REFERENCE = @ref;", conn);
        cmd.Parameters.Add(new SqlParameter("@ref", SqlDbType.VarChar, 23) { Value = reference });
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var req = new BillingCollectionRequest
            {
                RowId = reader.GetInt32(reader.GetOrdinal("ROWID")),
                DateRequested = reader.GetDateTime(reader.GetOrdinal("DATEREQUESTED")),
                SubSSN = reader.IsDBNull(reader.GetOrdinal("SUBSSN")) ? null : reader.GetString(reader.GetOrdinal("SUBSSN")),
                Reference = reader.IsDBNull(reader.GetOrdinal("REFERENCE")) ? null : reader.GetString(reader.GetOrdinal("REFERENCE")),
                DeductionReference = reader.IsDBNull(reader.GetOrdinal("DEDUCTIONREFERENCE")) ? null : reader.GetString(reader.GetOrdinal("DEDUCTIONREFERENCE")),
                AmountRequested = reader.IsDBNull(reader.GetOrdinal("AMOUNTREQUESTED")) ? null : reader.GetString(reader.GetOrdinal("AMOUNTREQUESTED")),
                Result = reader.IsDBNull(reader.GetOrdinal("RESULT")) ? null : reader.GetBoolean(reader.GetOrdinal("RESULT")),
                Method = reader.IsDBNull(reader.GetOrdinal("METHOD")) ? null : reader.GetInt32(reader.GetOrdinal("METHOD")),
                CreateBy = reader.IsDBNull(reader.GetOrdinal("CREATEBY")) ? null : reader.GetInt32(reader.GetOrdinal("CREATEBY")),
                CreateDate = reader.IsDBNull(reader.GetOrdinal("CREATEDATE")) ? null : reader.GetDateTime(reader.GetOrdinal("CREATEDATE")),
                LastChangeBy = reader.IsDBNull(reader.GetOrdinal("LASTCHANGEBY")) ? null : reader.GetInt32(reader.GetOrdinal("LASTCHANGEBY")),
                LastChangeDate = reader.IsDBNull(reader.GetOrdinal("LASTCHANGEDATE")) ? null : reader.GetDateTime(reader.GetOrdinal("LASTCHANGEDATE"))
            };
            return req;
        }
        return null;
    }

    /// <summary>
    /// Inserts a collection request record for each transaction in the generated file.
    /// </summary>
    /// <param name="records">Transaction records extracted from the live file.</param>
    /// <param name="bankFileRowId">Row Id from EDI_BANKFILES to relate requests back to the file.</param>
    public void InsertCollectionRequests(IEnumerable<TransactionRecord> records, int bankFileRowId)
    {
        var recordList = records.ToList();
        if (!recordList.Any()) return;

        if (bankFileRowId <= 0)
        {
            var first = recordList.First();
            int.TryParse(first.GenerationNumber, out var gen);
            int.TryParse(first.RecordSequenceNumber, out var start);

            var existing = GetBankFileRowId(first.Filename);
            if (existing > 0)
            {
                bankFileRowId = existing;
            }
            else
            {
                bankFileRowId = CreateBankFileRecord(first.Filename, gen, start);
            }
        }

        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        int counter = 0;
        foreach (var r in recordList)
        {
            counter++;
            using var existsCmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.BILLING_COLLECTIONREQUESTS WHERE DEDUCTIONREFERENCE = @deductionReference", conn);
            existsCmd.Parameters.Add(new SqlParameter("@deductionReference", SqlDbType.VarChar, 50) { Value = r.PaymentInformation });
            var exists = Convert.ToInt32(existsCmd.ExecuteScalar());
            if (exists > 0) continue;

            using var cmd = new SqlCommand(@"INSERT INTO dbo.BILLING_COLLECTIONREQUESTS
                (DATEREQUESTED, SUBSSN, REFERENCE, DEDUCTIONREFERENCE, AMOUNTREQUESTED,
                 RESULT,  CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE, EDIBANKFILEROWID,METHOD)
             VALUES (@dateRequested, @subssn, @reference, @deductionReference, @amountRequested,
                 0,  99, GETDATE(), 99, GETDATE(), @fileRowId, @method);", conn);

            DateTime.TryParse(r.RequestedCollectionDate, out var dateRequested);
            decimal.TryParse(r.InstructedAmount, out var amountRequestedInCents);
            var amountRequested = amountRequestedInCents / 100m;

            cmd.Parameters.Add(new SqlParameter("@dateRequested", SqlDbType.DateTime) { Value = (object)dateRequested });
            cmd.Parameters.Add(new SqlParameter("@subssn", SqlDbType.VarChar, 23) { Value = "MGS" + r.ContractReference });
            cmd.Parameters.Add(new SqlParameter("@reference", SqlDbType.VarChar, 23) { Value = r.ContractReference });
            cmd.Parameters.Add(new SqlParameter("@deductionReference", SqlDbType.VarChar, 50) { Value = r.PaymentInformation });
            cmd.Parameters.Add(new SqlParameter("@amountRequested", SqlDbType.Decimal) { Precision = 24, Scale = 2, Value = amountRequested });
            cmd.Parameters.Add(new SqlParameter("@fileRowId", SqlDbType.Int) { Value = bankFileRowId });
            cmd.Parameters.Add(new SqlParameter("@method", SqlDbType.Int) { Value = 1 });
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
        }

        if (bankFileRowId > 0)
        {
            var maxSeq = recordList
                .Select(r => int.TryParse(r.RecordSequenceNumber, out var seq) ? seq : 0)
                .DefaultIfEmpty()
                .Max();

            UpdateBankFileDailyCounterEnd(bankFileRowId, maxSeq);
        }
    }

    /// <summary>
    /// Inserts response records from a Status Report and updates the original collection requests.
    /// </summary>
    /// <param name="statusRecords">A collection of transaction data from the status report.</param>
    /// <param name="filePath">The path of the status report file being processed.</param>
    public void InsertCollectionResponses(IEnumerable<StatusReportTransaction> statusRecords, string filePath)
    {
        var recordList = statusRecords.ToList();
        if (!recordList.Any()) return;

        string fileName = Path.GetFileName(filePath);
        int bankFileRowId = GetBankFileRowId(fileName);
        if (bankFileRowId <= 0)
        {
            bankFileRowId = CreateBankFileRecord(fileName, 0, 0);
        }

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var existingResponses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var existingCmd = new SqlCommand(@"SELECT TOP (1000) [ROWID], [COLLECTIONREQUESTSROWID], [EDIBANKFILEROWID],
            [TRANSACTIONSTATUS], [REJECTREASONCODE], [REJECTREASONDESCRIPTION], [ACTIONDATE], [EFFECTIVEDATE],
            [ORIGINALCONTRACTREFERENCE], [ORIGINALPAYMENTINFORMATION], [CREATEBY], [CREATEDATE]
            FROM [BILLING_COLLECTIONRESPONSES]
            WHERE EDIBANKFILEROWID = @bankFileRowId", conn))
        {
            existingCmd.Parameters.Add(new SqlParameter("@bankFileRowId", SqlDbType.Int) { Value = bankFileRowId });
            using var reader = existingCmd.ExecuteReader();
            while (reader.Read())
            {
                var info = reader["ORIGINALPAYMENTINFORMATION"]?.ToString();
                if (!string.IsNullOrEmpty(info)) existingResponses.Add(info);
            }
        }

        foreach (var r in recordList)
        {
            int originalRequestRowId = 0;
            using (var findCmd = new SqlCommand("SELECT ROWID FROM dbo.BILLING_COLLECTIONREQUESTS WHERE DEDUCTIONREFERENCE = @ref", conn))
            {
                findCmd.Parameters.Add(new SqlParameter("@ref", SqlDbType.VarChar, 50) { Value = r.OriginalPaymentInformation });
                var result = findCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    originalRequestRowId = Convert.ToInt32(result);
                }
            }

            if (originalRequestRowId == 0)
            {
                continue;
            }

            if (existingResponses.Contains(r.OriginalPaymentInformation))
            {
                continue;
            }

            using var insertCmd = new SqlCommand(@"INSERT INTO dbo.BILLING_COLLECTIONRESPONSES
                (COLLECTIONREQUESTSROWID, EDIBANKFILEROWID, TRANSACTIONSTATUS,
                 REJECTREASONCODE, REJECTREASONDESCRIPTION, ACTIONDATE, EFFECTIVEDATE,
                 ORIGINALCONTRACTREFERENCE, ORIGINALPAYMENTINFORMATION, CREATEBY, CREATEDATE)
               VALUES
                (@reqId, @fileId, @status, @reasonCode, @reasonDesc, @actionDate,
                 @effectiveDate, @origContractRef, @origPmtInfo, 99, GETDATE());", conn);

            DateTime.TryParse(r.ActionDate, out var actionDate);

            bool isEffectiveDateValid = DateTime.TryParseExact(
                r.EffectiveDate,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var effectiveDate);
            if (!isEffectiveDateValid)
            {

            }
            insertCmd.Parameters.Add(new SqlParameter("@reqId", SqlDbType.Int) { Value = originalRequestRowId });
            insertCmd.Parameters.Add(new SqlParameter("@fileId", SqlDbType.Int) { Value = bankFileRowId });
            insertCmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 4) { Value = r.TransactionStatus });
            insertCmd.Parameters.Add(new SqlParameter("@reasonCode", SqlDbType.VarChar, 6) { Value = (object?)r.RejectReasonCode ?? DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@reasonDesc", SqlDbType.VarChar, 135) { Value = (object?)r.RejectReasonDescription ?? DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@actionDate", SqlDbType.DateTime) { Value = (object?)actionDate ?? DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@effectiveDate", SqlDbType.DateTime) { Value = isEffectiveDateValid ? effectiveDate : DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@origContractRef", SqlDbType.VarChar, 14) { Value = r.ContractReference });
            insertCmd.Parameters.Add(new SqlParameter("@origPmtInfo", SqlDbType.VarChar, 35) { Value = r.OriginalPaymentInformation });
            try
            {
                insertCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                //throw;
            }

            existingResponses.Add(r.OriginalPaymentInformation);

            bool success = r.TransactionStatus.ToUpper() == "ACCP";
            using var updateCmd = new SqlCommand(@"UPDATE dbo.BILLING_COLLECTIONREQUESTS
                SET RESULT = @result, LASTCHANGEBY = 99, LASTCHANGEDATE = GETDATE()
                WHERE ROWID = @reqId;", conn);
            updateCmd.Parameters.Add(new SqlParameter("@result", SqlDbType.Bit) { Value = success });
            updateCmd.Parameters.Add(new SqlParameter("@reqId", SqlDbType.Int) { Value = originalRequestRowId });
            updateCmd.ExecuteNonQuery();
        }
    }
}