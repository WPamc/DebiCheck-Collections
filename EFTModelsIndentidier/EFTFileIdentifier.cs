using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EFT_Collections;
using FileHelpers; // Ensure this is referenced

// Keep your EftFileType enum
public enum EftFileType
{
    Unknown,
    CollectionSubmission,
    ImmediateResponse,
    EftOutput
}

public class EftFileIdentifier
{
    private readonly Dictionary<EftFileType, HashSet<Type>> _fileTypeSignatures;
    private readonly MultiRecordEngine _universalEngine;
    private Type _lastProcessedRecordType;

    public EftFileIdentifier()
    {
        _fileTypeSignatures = new Dictionary<EftFileType, HashSet<Type>>
        {
            {
                EftFileType.CollectionSubmission, new HashSet<Type>
                {
                    typeof(TransmissionHeader000),
                    typeof(EftUserHeader001), // Core for EFT user set
                    // typeof(EftStandardTransaction001), // Optional for signature, but expected
                    // typeof(EftContraRecord001),       // Optional for signature
                    // typeof(EftUserTrailer001),       // Optional for signature
                    typeof(TransmissionTrailer999)
                }
            },
            {
                EftFileType.ImmediateResponse, new HashSet<Type>
                {
                    typeof(TransmissionHeader000),
                    typeof(ResponseStatus900), // Key differentiator
                    typeof(TransmissionTrailer999)
                }
            },
            {
                EftFileType.EftOutput, new HashSet<Type> // Primarily for Unpaids based on current models
                {
                    typeof(OutputFileHeader010), // Key differentiator
                    // typeof(UnpaidSetHeader011), // Optional for signature, but expected if data
                    // typeof(UnpaidTransactionDetail013), // Optional
                    typeof(OutputFileTrailer019)
                }
            }
        };

        _universalEngine = new MultiRecordEngine(
            // Submission File Models
            typeof(TransmissionHeader000),
            typeof(EftUserHeader001),
            typeof(EftStandardTransaction001),
            typeof(EftContraRecord001),
            typeof(EftUserTrailer001),
            typeof(TransmissionTrailer999),
            // Immediate Response File Models
            typeof(ResponseStatus900),
            typeof(RejectionReason901),
            typeof(AcceptedReportRecord903),
            // Unpaid and Redirect Report Models
            typeof(OutputFileHeader010),
            typeof(UnpaidSetHeader011),
            typeof(UnpaidTransactionDetail013),
            typeof(UnpaidSetTrailer014),
            //typeof(RedirectsSetHeader016),
            //typeof(RedirectsTransactionDetail017),
            //typeof(RedirectsSetTrailer018),
            typeof(OutputFileTrailer019)
        )
        {
            RecordSelector = new RecordTypeSelector(CustomSelectorForAllEftTypes)
        };
    }

    /// <summary>
    /// Custom record selector for the universal FileHelpers engine.
    /// Distinguishes between all known EFT record types based on identifiers and file structure context.
    /// </summary>
    private Type CustomSelectorForAllEftTypes(MultiRecordEngine engine, string recordLine)
    {
        if (string.IsNullOrWhiteSpace(recordLine) || recordLine.Length < 3)
        {
            _lastProcessedRecordType = null;
            return null;
        }

        string recId = recordLine.Substring(0, 3);
        Type selectedType = null;

        switch (recId)
        {
            case "000":
                selectedType = typeof(TransmissionHeader000);
                break;
            case "999":
                selectedType = typeof(TransmissionTrailer999);
                break;
            case "001": // EFT User Set records require contextual parsing
                if (recordLine.Length < 5) break;
                string bankservId = recordLine.Substring(4, 2);

                if (bankservId == "04")
                {
                    selectedType = typeof(EftUserHeader001);
                }
                else if (bankservId == "92")
                {
                    selectedType = typeof(EftUserTrailer001);
                }
                else if (bankservId == "10" || bankservId == "50")
                {
                    // This is a standard transaction, can appear after header or another transaction
                    if (_lastProcessedRecordType == typeof(EftUserHeader001) ||
                        _lastProcessedRecordType == typeof(EftStandardTransaction001))
                    {
                        selectedType = typeof(EftStandardTransaction001);
                    }
                }
                else if (bankservId == "12" || bankservId == "52")
                {
                    // This is a contra record, must appear after a standard transaction
                    if (_lastProcessedRecordType == typeof(EftStandardTransaction001))
                    {
                        selectedType = typeof(EftContraRecord001);
                    }
                }
                break;
            case "900":
                selectedType = typeof(ResponseStatus900);
                break;
            case "901":
                selectedType = typeof(RejectionReason901);
                break;
            case "903":
                selectedType = typeof(AcceptedReportRecord903);
                break;
            case "010":
                selectedType = typeof(OutputFileHeader010);
                break;
            case "011":
                selectedType = typeof(UnpaidSetHeader011);
                break;
            case "013":
                selectedType = typeof(UnpaidTransactionDetail013);
                break;
            case "014":
                selectedType = typeof(UnpaidSetTrailer014);
                break;
            //case "016":
            //    selectedType = typeof(RedirectsSetHeader016);
            //    break;
            //case "017":
            //    selectedType = typeof(RedirectsTransactionDetail017);
            //    break;
            //case "018":
            //    selectedType = typeof(RedirectsSetTrailer018);
            //    break;
            case "019":
                selectedType = typeof(OutputFileTrailer019);
                break;
        }

        if (selectedType != null)
        {
            _lastProcessedRecordType = selectedType;
        }
        return selectedType;
    }

    /// <summary>
    /// Identifies the type of an EFT file by attempting to parse its records
    /// and matching against predefined signatures, with filename hints.
    /// </summary>
    /// <param name="filePath">The path to the EFT file.</param>
    /// <returns>The identified EftFileType.</returns>
    public EftFileType IdentifyFileType(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return EftFileType.Unknown;
        }

        _lastProcessedRecordType = null; // Reset state for each new file identification

        string fileName = Path.GetFileName(filePath).ToUpperInvariant();
        EftFileType hintedType = EftFileType.Unknown;

        // 1. Filename hints (as per spec pg 8 for ABSA files, or common patterns)
        if (fileName.StartsWith("REPLY."))
        {
            hintedType = EftFileType.ImmediateResponse;
        }
        else if (fileName.StartsWith("OUTPUT."))
        {
            hintedType = EftFileType.EftOutput;
        }
        // Check for ZR...R... or ZR...O... patterns
        // Example: "ZR07702R..." or "ZR07702O..." (assuming client code is 07702)
        // This regex is a bit basic, adjust client code part if needed or make more generic
        if (System.Text.RegularExpressions.Regex.IsMatch(fileName, @"^ZR\d+[RO]"))
        {
            if (fileName.Contains("R")) hintedType = EftFileType.ImmediateResponse;
            else if (fileName.Contains("O")) hintedType = EftFileType.EftOutput;
        }


        object[] parsedRecords = null;
        try
        {
            // It's safer to read lines and pass to engine if files can be very large
            // or have non-standard line endings that ReadFile might struggle with.
            // For simplicity, using ReadFile. Add ReadAllLines approach if issues.
            parsedRecords = _universalEngine.ReadFile(filePath);
            if (parsedRecords == null || parsedRecords.Length == 0)
            {
                return EftFileType.Unknown; // Empty or unparseable by any known model
            }
        }
        catch (Exception ex) // Catches FileHelpersException and other IOExceptions
        {
            Console.WriteLine($"Error parsing file {filePath} with universal engine: {ex.Message}");
            return EftFileType.Unknown;
        }

        var foundRecordTypes = new HashSet<Type>(parsedRecords.Select(r => r.GetType()));

        // 2. Check hinted type first if a hint was found
        if (hintedType != EftFileType.Unknown)
        {
            if (_fileTypeSignatures.TryGetValue(hintedType, out var signature) &&
                signature.IsSubsetOf(foundRecordTypes)) // All core records of hinted type are present
            {
                return hintedType;
            }
        }

        // 3. If no hint or hinted type didn't match, iterate through all signatures
        foreach (var signatureEntry in _fileTypeSignatures)
        {
            // Avoid re-checking the hinted type if it already failed
            if (hintedType != EftFileType.Unknown && signatureEntry.Key == hintedType)
            {
                continue;
            }

            if (signatureEntry.Value.IsSubsetOf(foundRecordTypes)) // All core records of this type are present
            {
                // More complex logic might be needed if signatures overlap significantly.
                // For now, first match wins.
                return signatureEntry.Key;
            }
        }

        // Fallback for CollectionSubmission if it wasn't hinted and didn't perfectly match the minimal signature
        // but contains the absolute essentials of a user-generated file.
        if (foundRecordTypes.Contains(typeof(TransmissionHeader000)) &&
            foundRecordTypes.Contains(typeof(EftUserHeader001)) && // Indicates it's likely an EFT user set
            foundRecordTypes.Contains(typeof(TransmissionTrailer999)) &&
            hintedType == EftFileType.Unknown) // And no other ABSA hint matched
        {
            return EftFileType.CollectionSubmission;
        }


        return EftFileType.Unknown;
    }
}