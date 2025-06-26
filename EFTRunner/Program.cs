using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EFT_Collections;

/// <summary>
/// Console runner for generating EFT collection files.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        DateTime deductionDate = DateTime.Now;
        string dataSetStatus = "T";
        string fileName = "";
        string outputPath = AppContext.BaseDirectory;
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
            else if (name == "outputpath")
            {
                outputPath = value;
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

        var writer = new EFTService(
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
            startSequenceNumber = await db.GetNextDailyCounterAsync(DateTime.Today);
        }

        var collections = await db.GetCollectionsAsync(deductionDate);
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

        fileName = $"ZR{creditorDefaults.clientCode}.AUL.DATA.{DateTime.Now:yyMMdd.HHmmss}";
        int recordId = 0;
        if (dataSetStatus != "T")
        {
            recordId = await db.CreateBankFileRecordAsync(Path.GetFileName(fileName), generationNumber, startSequenceNumber);
        }
        writer.GenerateFile(
            transactionsToProcess,
            generationNumber,
            generationNumber,
            startSequenceNumber,
            out long lastSequenceNumber,
            fileName,
            outputPath);

        if (dataSetStatus != "T")
        {
            await db.UpdateBankFileDailyCounterEndAsync(recordId, (int)lastSequenceNumber);
            await db.SetDailyCounterAsync(deductionDate, (int)lastSequenceNumber);
        }
        Console.WriteLine($"EFT file written to {Path.Combine(outputPath, fileName)}");
    }
}
