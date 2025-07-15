using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using DbConnection;

namespace EFT_Collections;

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
        _collectionsSql = File.ReadAllText(Path.Combine(queriesPath, "EFTCollections.sql"));
        _creditorDefaultsSql = File.ReadAllText(Path.Combine(queriesPath, "CreditorDefaults.sql"));
    }

    public int InsertUnpaidTransactions(IEnumerable<UnpaidTransactionDetail013> records, int bankFileRowId, string actionDateForDataSet)
    {
        int inserted = 0;
        if (!records.Any()) return inserted;

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var existingResponses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var existingCmd = new SqlCommand(@"SELECT ORIGINALPAYMENTINFORMATION FROM dbo.BILLING_COLLECTIONRESPONSES WHERE EDIBANKFILEROWID = @fileId", conn))
        {
            existingCmd.Parameters.Add(new SqlParameter("@fileId", SqlDbType.Int) { Value = bankFileRowId });
            using var reader = existingCmd.ExecuteReader();
            while (reader.Read())
            {
                var info = reader["ORIGINALPAYMENTINFORMATION"]?.ToString();
                if (!string.IsNullOrEmpty(info)) existingResponses.Add(info);
            }
        }

        var codeLookup = AppConfig.EftRejectionCodes;
        foreach (var r in records)
        {
            int originalRequestRowId = 0;
            using (var findCmd = new SqlCommand("SELECT ROWID FROM dbo.BILLING_COLLECTIONREQUESTS WHERE DEDUCTIONREFERENCE = @ref", conn))
            {
                findCmd.Parameters.Add(new SqlParameter("@ref", SqlDbType.VarChar, 50) { Value = r.UserReference.Trim() });
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

            var paymentInfo = r.UserReference.Trim();
            if (existingResponses.Contains(paymentInfo))
            {
                continue;
            }

            if (!DateTime.TryParseExact(actionDateForDataSet, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var actionDate))
                throw new Exception($"Cannot parse dateRequested : {actionDateForDataSet}");

            using var insertCmd = new SqlCommand(@"INSERT INTO dbo.BILLING_COLLECTIONRESPONSES
                (COLLECTIONREQUESTSROWID, EDIBANKFILEROWID, TRANSACTIONSTATUS,
                 REJECTREASONCODE, REJECTREASONDESCRIPTION, ACTIONDATE, EFFECTIVEDATE,
                 ORIGINALCONTRACTREFERENCE, ORIGINALPAYMENTINFORMATION, CREATEBY, CREATEDATE)
               VALUES
                (@reqId, @fileId, 'RJCT', @reasonCode, @reasonDesc, @actionDate, NULL,
                 NULL, @origPmtInfo, 99, GETDATE());", conn);

            insertCmd.Parameters.Add(new SqlParameter("@reqId", SqlDbType.Int) { Value = originalRequestRowId });
            insertCmd.Parameters.Add(new SqlParameter("@fileId", SqlDbType.Int) { Value = bankFileRowId });
            var rejectCode = r.RejectionReason?.Trim();
            codeLookup.TryGetValue(rejectCode ?? string.Empty, out var desc);
            insertCmd.Parameters.Add(new SqlParameter("@reasonCode", SqlDbType.VarChar, 6) { Value = (object?)rejectCode ?? DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@reasonDesc", SqlDbType.VarChar, 135) { Value = (object?)desc ?? DBNull.Value });
            insertCmd.Parameters.Add(new SqlParameter("@actionDate", SqlDbType.DateTime) { Value = actionDate });
            insertCmd.Parameters.Add(new SqlParameter("@origPmtInfo", SqlDbType.VarChar, 35) { Value = paymentInfo });
            try
            {
                int rows = insertCmd.ExecuteNonQuery();

                inserted=inserted+rows;
            }
            catch
            {
            }

            using var updateCmd = new SqlCommand(@"UPDATE dbo.BILLING_COLLECTIONREQUESTS
                SET RESULT = 0, LASTCHANGEBY = 99, LASTCHANGEDATE = GETDATE()
                WHERE ROWID = @reqId;", conn);
            updateCmd.Parameters.Add(new SqlParameter("@reqId", SqlDbType.Int) { Value = originalRequestRowId });
            updateCmd.ExecuteNonQuery();

            existingResponses.Add(paymentInfo);
        }

        return inserted;
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

    public List<DebtorCollectionData> GetCollections(DateTime deductionDay)
    {
        var results = new List<DebtorCollectionData>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(_collectionsSql, conn);
        cmd.Parameters.Add(new SqlParameter("@DATEREQUESTED", SqlDbType.Date)
        {
            Value = deductionDay.Date
        });
        conn.Open();
        using var reader = cmd.ExecuteReader();

        bool HasColumn(IDataRecord record, string name)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        while (reader.Read())
        {
            string contractRef = reader["Subssn"].ToString().Substring(3);

            var data = new DebtorCollectionData
            {
                DebtorBankBranch = reader["HomingBranch"].ToString() ?? string.Empty,
                DebtorAccountNumber = reader["HomingAccountNumber"].ToString() ?? string.Empty,
                DebtorName = reader["HomingAccountName"].ToString() ?? string.Empty,
                AccountType = reader["AccountType"].ToString() ?? string.Empty,
                InstructedAmount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0m,
                ContractReference = contractRef,
                RequestedCollectionDate = HasColumn(reader, "DATEREQUESTED") ? reader.GetDateTime(reader.GetOrdinal("DATEREQUESTED")) : default,
                PaymentInformation = reader["UserReference"].ToString() ?? string.Empty,
                TrackingPeriod = HasColumn(reader, nameof(DebtorCollectionData.TrackingPeriod)) ? reader.GetInt32(reader.GetOrdinal(nameof(DebtorCollectionData.TrackingPeriod))) : 3,
                DebitSequence = HasColumn(reader, nameof(DebtorCollectionData.DebitSequence)) ? reader[nameof(DebtorCollectionData.DebitSequence)].ToString() ?? string.Empty : "RCUR",
                EntryClass = HasColumn(reader, nameof(DebtorCollectionData.EntryClass)) ? reader[nameof(DebtorCollectionData.EntryClass)].ToString() ?? string.Empty : "0021",
                MandateReference = HasColumn(reader, nameof(DebtorCollectionData.MandateReference)) ? reader[nameof(DebtorCollectionData.MandateReference)].ToString() ?? string.Empty : string.Empty,
                RelatedCycleDate = HasColumn(reader, nameof(DebtorCollectionData.RelatedCycleDate)) ? reader.GetDateTime(reader.GetOrdinal(nameof(DebtorCollectionData.RelatedCycleDate))) : DateTime.Now
            };
            results.Add(data);
        }
        return results;
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

    public int GetNextGenerationNumber()
        => GetNextCounter("EFT GENERATIONNUMBER", null);

    public int GetNextDailyCounter(DateTime date)
        => GetNextCounter("EFT DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

    private int PeekCounter(string subclass1, string? subclass2)
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

    public int PeekGenerationNumber()
        => PeekCounter("EFT GENERATIONNUMBER", null);

    public int PeekDailyCounter(DateTime date)
        => PeekCounter("EFT DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

    public void SetDailyCounter(DateTime date, int counterValue)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE dbo.EDI_GENERICCOUNTERS
   SET COUNTER = @val,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE DESCRIPTION = @desc
   AND SUBCLASS1 = 'EFT DAILYCOUNTER'
   AND SUBCLASS2 = @sub;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.EDI_GENERICCOUNTERS
        (DESCRIPTION, SUBCLASS1, SUBCLASS2, COUNTER, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
    VALUES (@desc, 'EFT DAILYCOUNTER', @sub, @val, 99, GETDATE(), 99, GETDATE());
END", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@sub", SqlDbType.VarChar, 100) { Value = date.ToString("yyyy-MM-dd") });
        cmd.Parameters.Add(new SqlParameter("@val", SqlDbType.Int) { Value = counterValue });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void EnsureDailyCounterForToday()
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"IF NOT EXISTS (
SELECT 1 FROM dbo.EDI_GENERICCOUNTERS
 WHERE DESCRIPTION = @desc
   AND SUBCLASS1 = 'EFT DAILYCOUNTER'
   AND SUBCLASS2 = @sub)
BEGIN
    INSERT INTO dbo.EDI_GENERICCOUNTERS
        (DESCRIPTION, SUBCLASS1, SUBCLASS2, COUNTER, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
    VALUES (@desc, 'EFT DAILYCOUNTER', @sub, 0, 99, GETDATE(), 99, GETDATE());
END", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@sub", SqlDbType.VarChar, 100) { Value = DateTime.Today.ToString("yyyy-MM-dd") });
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public int CreateBankFileRecord(string fileName, int generationNumber, int dailyCounterStart, string fileType, int transactionCount, decimal total)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        using var cmd = new SqlCommand(@"INSERT INTO dbo.EDI_BANKFILES
                (DESCRIPTION, FILENAME, FILETYPE, TOTAL, TRANSACCTIONCOUNT, GENERATIONNUMBER, DAILYCOUNTERSTART, DAILYCOUNTEREND,
                 GENERATIONCOMPLETE, DELIVERED, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
             VALUES (@desc, @file, @type, @total, @count, @gen, @start, @start, 0, 0, 99, GETDATE(), 99, GETDATE());
             SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.VarChar, 100) { Value = fileName });
        cmd.Parameters.Add(new SqlParameter("@type", SqlDbType.VarChar, 50) { Value = fileType });
        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = total });
        cmd.Parameters.Add(new SqlParameter("@count", SqlDbType.Int) { Value = transactionCount });
        cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.Int) { Value = generationNumber });
        cmd.Parameters.Add(new SqlParameter("@start", SqlDbType.Int) { Value = dailyCounterStart });

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
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

    public int InsertCollectionRequests(IEnumerable<DebtorCollectionData> records, int bankFileRowId)
    {
        var recordList = records.ToList();
        int inserted = 0;
        if (!recordList.Any()) return inserted;

        int transactionCount = recordList.Count;
        decimal total = recordList.Sum(r => r.InstructedAmount);

        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        foreach (var r in recordList)
        {
            using var existsCmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.BILLING_COLLECTIONREQUESTS WHERE DEDUCTIONREFERENCE = @deductionReference", conn);
            existsCmd.Parameters.Add(new SqlParameter("@deductionReference", SqlDbType.VarChar, 50) { Value = r.PaymentInformation });
            var exists = Convert.ToInt32(existsCmd.ExecuteScalar());
            if (exists > 0) continue;

            using var cmd = new SqlCommand(@"INSERT INTO dbo.BILLING_COLLECTIONREQUESTS
                (DATEREQUESTED, SUBSSN, REFERENCE, DEDUCTIONREFERENCE, AMOUNTREQUESTED,
                 RESULT, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE, EDIBANKFILEROWID, METHOD)
             VALUES (@dateRequested, @subssn, @reference, @deductionReference, @amountRequested,
                 0, 99, GETDATE(), 99, GETDATE(), @fileRowId, @method);", conn);

            cmd.Parameters.Add(new SqlParameter("@dateRequested", SqlDbType.DateTime) { Value = r.RequestedCollectionDate });
            cmd.Parameters.Add(new SqlParameter("@subssn", SqlDbType.VarChar, 23) { Value = "MGS" + r.ContractReference });
            cmd.Parameters.Add(new SqlParameter("@reference", SqlDbType.VarChar, 23) { Value = r.ContractReference });
            cmd.Parameters.Add(new SqlParameter("@deductionReference", SqlDbType.VarChar, 50) { Value = r.PaymentInformation });
            cmd.Parameters.Add(new SqlParameter("@amountRequested", SqlDbType.Decimal) { Precision = 24, Scale = 2, Value = r.InstructedAmount });
            cmd.Parameters.Add(new SqlParameter("@fileRowId", SqlDbType.Int) { Value = bankFileRowId });
            cmd.Parameters.Add(new SqlParameter("@method", SqlDbType.Int) { Value = 1 });
            inserted += cmd.ExecuteNonQuery();
        }
        if (bankFileRowId > 0)
        {
            UpdateBankFileInfo(bankFileRowId, EftFileType.CollectionSubmission.ToString(), transactionCount, total);
        }
        return inserted;
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

    public void UpdateBankFileInfo(int rowId, string fileType, int transactionCount, decimal total)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANKFILES
   SET FILETYPE = @type,
       TRANSACCTIONCOUNT = @count,
       TOTAL = @total,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        cmd.Parameters.Add(new SqlParameter("@type", SqlDbType.VarChar, 50) { Value = fileType });
        cmd.Parameters.Add(new SqlParameter("@count", SqlDbType.Int) { Value = transactionCount });
        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = total });
        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
