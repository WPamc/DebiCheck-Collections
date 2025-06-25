using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
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
        _collectionsSql = File.ReadAllText(Path.Combine(queriesPath, "Collections.sql"));
        _creditorDefaultsSql = File.ReadAllText(Path.Combine(queriesPath, "CreditorDefaults.sql"));
    }

    public async Task<List<DebtorCollectionData>> GetCollectionsAsync(int deductionDay)
    {
        var results = new List<DebtorCollectionData>();
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(_collectionsSql, conn);
        //cmd.Parameters.Add(new SqlParameter("@DEDUCTIONDAY", SqlDbType.Int) { Value = deductionDay });
        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
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

    public CreditorDefaults GetCreditorDefaultsAsync(int creditorId)
    {
        // Stubbed method - would normally query using _creditorDefaultsSql
        return new CreditorDefaults();
    }

    private async Task<int> GetNextCounterAsync(string subclass1, string? subclass2)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(@"SET NOCOUNT ON;
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

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public Task<int> GetNextGenerationNumberAsync()
        => GetNextCounterAsync("EFT GENERATIONNUMBER", null);

    public Task<int> GetNextDailyCounterAsync(DateTime date)
        => GetNextCounterAsync("EFT DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

    private async Task<int> PeekCounterAsync(string subclass1, string? subclass2)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(@"SELECT COUNTER
  FROM dbo.EDI_GENERICCOUNTERS
 WHERE DESCRIPTION = @desc
   AND SUBCLASS1 = @sub1
   AND ((@sub2 IS NULL AND SUBCLASS2 IS NULL) OR SUBCLASS2 = @sub2);", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@sub1", SqlDbType.VarChar, 100) { Value = subclass1 });
        cmd.Parameters.Add(new SqlParameter("@sub2", SqlDbType.VarChar, 100) { Value = (object?)subclass2 ?? DBNull.Value });

        var result = await cmd.ExecuteScalarAsync();
        return result == null ? 0 : Convert.ToInt32(result);
    }

    public Task<int> PeekGenerationNumberAsync()
        => PeekCounterAsync("EFT GENERATIONNUMBER", null);

    public Task<int> PeekDailyCounterAsync(DateTime date)
        => PeekCounterAsync("EFT DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

    public async Task SetDailyCounterAsync(DateTime date, int counterValue)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"UPDATE dbo.EDI_GENERICCOUNTERS
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
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> CreateBankFileRecordAsync(string fileName, int generationNumber, int dailyCounterStart)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(@"INSERT INTO dbo.EDI_BANK_FILES
                (DESCRIPTION, FILENAME, GENERATIONNUMBER, DAILYCOUNTERSTART, DAILYCOUNTEREND,
                 GENERATIONCOMPLETE, DELIVERED, CREATEBY, CREATEDATE, LASTCHANGEBY, LASTCHANGEDATE)
             VALUES (@desc, @file, @gen, @start, @start, 0, 0, 99, GETDATE(), 99, GETDATE());
             SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.Add(new SqlParameter("@desc", SqlDbType.VarChar, 100) { Value = CounterDescription });
        cmd.Parameters.Add(new SqlParameter("@file", SqlDbType.VarChar, 100) { Value = fileName });
        cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.Int) { Value = generationNumber });
        cmd.Parameters.Add(new SqlParameter("@start", SqlDbType.Int) { Value = dailyCounterStart });

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateBankFileDailyCounterEndAsync(int rowId, int dailyCounterEnd)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANK_FILES
   SET DAILYCOUNTEREND = @end,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        cmd.Parameters.Add(new SqlParameter("@end", SqlDbType.Int) { Value = dailyCounterEnd });
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task MarkBankFileGenerationCompleteAsync(int rowId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANK_FILES
   SET GENERATIONCOMPLETE = 1,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task MarkBankFileDeliveredAsync(int rowId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"UPDATE dbo.EDI_BANK_FILES
   SET DELIVERED = 1,
       LASTCHANGEBY = 99,
       LASTCHANGEDATE = GETDATE()
 WHERE ROWID = @id;", conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = rowId });
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
