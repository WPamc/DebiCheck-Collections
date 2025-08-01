namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents the outcome of parsing a file.
    /// </summary>
    public record ParseResult(object[] Records, DCFileType FileType, int StatusRecordsFound = 0, int StatusRecordsInserted = 0);
}
