namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents the result of generating a collection file.
    /// </summary>
    public record FileGenerationResult(string FilePath, int BankFilesUpdated, int CollectionRequestsUpdated);
}
