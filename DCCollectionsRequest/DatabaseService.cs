using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class CreditorDefaults
{
    public string UserCode { get; set; } = "07675";
    public string UserName { get; set; } = "AFRICAN UNITY LIFE LIMITED C";
    public string BankServUserCode { get; set; } = "D457";
    public string InitiatingParty { get; set; } = "AFRICAN UNITY LIFE LIMITED";
    public string CreditorName { get; set; } = "AFRICAN UNITY LIFE LIMITED";
    public string CreditorContactDetails { get; set; } = "+27-0861189202";
    public string CreditorAbbreviatedShortName { get; set; } = "AUL";
    public string CreditorEmail { get; set; } = "TALKTOUS@GETSAVVI.CO.ZA";
    public string CreditorAccountNumber { get; set; } = "0000000004097772529";
    public string CreditorBankBranch { get; set; } = "632005";
}

public class DatabaseService
{
    private readonly string _connectionString;
    private readonly string _collectionsSql;
    private readonly string _creditorDefaultsSql;

    public DatabaseService(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection missing");
        _collectionsSql = configuration["SqlQueries:Collections"]
            ?? throw new InvalidOperationException("Collections SQL missing");
        _creditorDefaultsSql = configuration["SqlQueries:CreditorDefaults"]
            ?? throw new InvalidOperationException("CreditorDefaults SQL missing");
    }

    public async Task<List<DebtorCollectionData>> GetCollectionsAsync()
    {
        var results = new List<DebtorCollectionData>();
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(_collectionsSql, conn);
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
                InstructedAmount = reader.GetDecimal(reader.GetOrdinal(nameof(DebtorCollectionData.InstructedAmount))),
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

    public async Task<CreditorDefaults?> GetCreditorDefaultsAsync(int creditorId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(_creditorDefaultsSql, conn);
        cmd.Parameters.Add(new SqlParameter("@CreditorId", SqlDbType.Int) { Value = creditorId });
        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (await reader.ReadAsync())
        {
            return new CreditorDefaults
            {
                InitiatingParty = reader[nameof(CreditorDefaults.InitiatingParty)].ToString() ?? string.Empty,
                CreditorName = reader[nameof(CreditorDefaults.CreditorName)].ToString() ?? string.Empty,
                CreditorContactDetails = reader[nameof(CreditorDefaults.CreditorContactDetails)].ToString() ?? string.Empty,
                CreditorAbbreviatedShortName = reader[nameof(CreditorDefaults.CreditorAbbreviatedShortName)].ToString() ?? string.Empty,
                CreditorEmail = reader[nameof(CreditorDefaults.CreditorEmail)].ToString() ?? string.Empty,
                CreditorAccountNumber = reader[nameof(CreditorDefaults.CreditorAccountNumber)].ToString() ?? string.Empty,
                CreditorBankBranch = reader[nameof(CreditorDefaults.CreditorBankBranch)].ToString() ?? string.Empty
            };
        }
        return null;
    }
}

