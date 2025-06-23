namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents the outcome of parsing a file.
    /// </summary>
    public record ParseResult(object[] Records, FileType FileType);
}
