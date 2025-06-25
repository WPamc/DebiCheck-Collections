using System;
using System.Collections.Generic;
using EFT_Collections;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// A logical representation of a single EFT transaction (a collection from a customer).
/// This class is used to pass dynamic transaction data to the Writer.
/// </summary>
public class EftTransaction
{
    /// <summary>
    /// The 6-digit branch code of the customer's (debtor's) bank account.
    /// </summary>
    public string HomingBranch { get; set; }

    /// <summary>
    /// The 11-digit account number of the customer (debtor).
    /// </eummary>
    public string HomingAccountNumber { get; set; }

    /// <summary>
    /// The name of the customer (debtor) account holder.
    /// </summary>
    public string HomingAccountName { get; set; }

    /// <summary>
    /// The type of the customer's (debtor's) account.
    /// 1=Current, 2=Savings, 3=Transmission, 4=Bond, 6=Subscription Share.
    /// </summary>
    public int AccountType { get; set; }

    /// <summary>
    /// The monetary value to be collected. E.g., 1501.00 for R1501.00.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The reference that will appear on the customer's (debtor's) bank statement.
    /// This is crucial for reconciliation.
    /// </summary>
    public string UserReference { get; set; }
}

/// <summary>
/// Main program to demonstrate the Writer usage.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        
        DateTime deductionDate = DateTime.Now;
        string dataSetStatus = "T";
        string fileName = "";
        foreach (string arg in args)
        {
            var parts = arg.Split('=', 2);
            if (parts.Length != 2) continue;
            var name = parts[0].TrimStart('-').ToLowerInvariant();
            var value = parts[1];
            if (name == "output")
            {
                fileName = value;
            }
            else if (name == "date")
            {
                deductionDate = DateTime.Parse(value);
            }
            else if (name == "mode")
            {
                dataSetStatus = value.ToUpperInvariant() == "T" ? "T" : "L";
            }
        }



        // 1. Initialize the writer with your static creditor data from the sample file.

        var writer = new Writer(
            clientCode: creditorDefaults.clientCode,
            clientName: creditorDefaults.clientName,
            bankservUserCode: creditorDefaults.bankservUserCode,
            creditorBranch: creditorDefaults.creditorBranch,
            creditorAccount: creditorDefaults.creditorAccount,
            creditorAbbreviation: creditorDefaults.creditorAbbreviation,
            deductionDate: deductionDate,
            recordStatus: dataSetStatus
        );

        var db = new DatabaseService();


        int generationNumber;
        int startSequenceNumber;
        if (dataSetStatus == "T")
        {
            generationNumber = (await db.PeekGenerationNumberAsync()) + 1;
            startSequenceNumber = (await db.PeekDailyCounterAsync(deductionDate)) + 1;
        }
        else
        {
            generationNumber = await db.GetNextGenerationNumberAsync();
            startSequenceNumber = await db.GetNextDailyCounterAsync(deductionDate);
        }


        // 2. Load collection transactions from the database.
        var collections = await db.GetCollectionsAsync(deductionDate.Day);
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
                UserReference = c.ContractReference + "_" + generationNumber.ToString()
            });
        }

        // 3. Define the parameters for this specific file generation.
       
        fileName = $"ZR{creditorDefaults.clientCode}.AUL.DATA.{DateTime.Now:yyMMdd.HHmmss}";
        int recordId = await db.CreateBankFileRecordAsync(Path.GetFileName(fileName), generationNumber, startSequenceNumber);

        // 4. Generate and write the file.
        long lastSequenceNumber = writer.WriteFile(
            fileName,
            transactionsToProcess,
            generationNumber,
            generationNumber,
            startSequenceNumber);
        await db.UpdateBankFileDailyCounterEndAsync(recordId, (int)lastSequenceNumber);
        if (dataSetStatus != "T")
        {
            await db.SetDailyCounterAsync(deductionDate, (int)lastSequenceNumber);
        }
        Console.WriteLine($"EFT file written to {fileName}");
    }
}
