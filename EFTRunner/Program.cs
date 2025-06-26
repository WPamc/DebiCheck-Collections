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
        await EFTService.GenerateEFTFile(deductionDate, dataSetStatus, outputPath);
    }
}
