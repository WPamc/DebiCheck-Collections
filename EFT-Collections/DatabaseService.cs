using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    public List<DebtorCollectionData> GetCollections(DateTime deductionDay)
    {
        var results = new List<DebtorCollectionData>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(_collectionsSql, conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var data = new DebtorCollectionData
            {
                DebtorBankBranch = reader["HomingBranch"].ToString() ?? string.Empty,
                DebtorAccountNumber = reader["HomingAccountNumber"].ToString() ?? string.Empty,
                DebtorName = reader["HomingAccountName"].ToString() ?? string.Empty,
                AccountType = reader["AccountType"].ToString() ?? string.Empty,
                InstructedAmount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0m,
                ContractReference = reader["UserReference"].ToString() ?? string.Empty
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
}
