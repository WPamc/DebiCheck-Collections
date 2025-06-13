using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

public class CreditorDefaults
{
    public string UserCode { get; set; } = "07675";
    public string UserName { get; set; } = "AFRICAN UNITY LIFE LIMITED C";
    public string BankServUserCode { get; set; } = "D457";
    public string InitiatingParty { get; set; } = "AFRICAN UNITY LIFE LIMITED";
    public string CreditorName { get; set; } = "AFRICAN UNITY LIFE LIMITED";
    public string CreditorContactDetails { get; set; } = "+27-0861189202";
    public string CreditorAbbreviatedShortName { get; set; } = "AUL";
    public string CreditorEmail { get; set; } = "MEMBERSHIP@PAMC.CO.ZA";
    public string CreditorAccountNumber { get; set; } = "0000000004097772529";
    public string CreditorBankBranch { get; set; } = "632005";
}

public class DatabaseService
{
    private const string CounterDescription = "SANLAM MULTIDATA";
    private readonly string _connectionString;
    private readonly string _collectionsSql;
    private readonly string _creditorDefaultsSql;

    public DatabaseService(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection missing");

        var queriesPath = configuration["SqlQueriesPath"] ?? "SqlQueries";
        _collectionsSql = File.ReadAllText(Path.Combine(queriesPath, "Collections.sql"));
        _creditorDefaultsSql = File.ReadAllText(Path.Combine(queriesPath, "CreditorDefaults.sql"));
    }

    public async Task<List<DebtorCollectionData>> GetCollectionsAsync(int deductionDay)
    {
        var results = new List<DebtorCollectionData>();
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(_collectionsSql, conn);
        cmd.Parameters.Add(new SqlParameter("@DEDUCTIONDAY", SqlDbType.Int) { Value = deductionDay });
        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var data = new DebtorCollectionData
            {
                PaymentInformation = reader[nameof(DebtorCollectionData.PaymentInformation)].ToString() ?? string.Empty,
                RequestedCollectionDate = reader.GetDateTime(reader.GetOrdinal(nameof(DebtorCollectionData.RequestedCollectionDate))),
                TrackingPeriod = reader.GetInt32(reader.GetOrdinal(nameof(DebtorCollectionData.TrackingPeriod))),
                DebitSequence = reader[nameof(DebtorCollectionData.DebitSequence)].ToString() ?? string.Empty,
                EntryClass = reader[nameof(DebtorCollectionData.EntryClass)].ToString() ?? string.Empty,
                InstructedAmount = Convert.ToDecimal( reader["InstructedAmount"]),
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

    public  CreditorDefaults GetCreditorDefaultsAsync(int creditorId)
    {
    //    await using var conn = new SqlConnection(_connectionString);
    //    await using var cmd = new SqlCommand(_creditorDefaultsSql, conn);
    //    cmd.Parameters.Add(new SqlParameter("@CreditorId", SqlDbType.Int) { Value = creditorId });
    //    await conn.OpenAsync();
    //    await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
    //    if (await reader.ReadAsync())
    //    {
    //        return new CreditorDefaults
    //        {
    //            InitiatingParty = reader[nameof(CreditorDefaults.InitiatingParty)].ToString() ?? string.Empty,
    //            CreditorName = reader[nameof(CreditorDefaults.CreditorName)].ToString() ?? string.Empty,
    //            CreditorContactDetails = reader[nameof(CreditorDefaults.CreditorContactDetails)].ToString() ?? string.Empty,
    //            CreditorAbbreviatedShortName = reader[nameof(CreditorDefaults.CreditorAbbreviatedShortName)].ToString() ?? string.Empty,
    //            CreditorEmail = reader[nameof(CreditorDefaults.CreditorEmail)].ToString() ?? string.Empty,
    //            CreditorAccountNumber = reader[nameof(CreditorDefaults.CreditorAccountNumber)].ToString() ?? string.Empty,
    //            CreditorBankBranch = reader[nameof(CreditorDefaults.CreditorBankBranch)].ToString() ?? string.Empty
    //        };
    //    }
        return 
        new CreditorDefaults();
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
        => GetNextCounterAsync("GENERATIONNUMBER", null);

    public Task<int> GetNextDailyCounterAsync(DateTime date)
        => GetNextCounterAsync("DAILYCOUNTER", date.ToString("yyyy-MM-dd"));

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

