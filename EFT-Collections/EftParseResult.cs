namespace EFT_Collections;

/// <summary>
/// Represents the result of parsing an EFT file.
/// </summary>
public class EftParseResult
{
    public object[] Records { get; }
    public EftFileType FileType { get; }
    public int RecordsInserted { get; }

    public EftParseResult(object[] records, EftFileType fileType, int recordsInserted)
    {
        Records = records;
        FileType = fileType;
        RecordsInserted = recordsInserted;
    }
}
