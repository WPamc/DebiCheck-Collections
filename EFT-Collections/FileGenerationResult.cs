namespace EFT_Collections;

/// <summary>
/// Represents the result of generating a collection file.
/// </summary>
public class FileGenerationResult
{
    public string FilePath { get; }
    public int BankFilesUpdated { get; }
    public int CollectionRequestsUpdated { get; }

    public FileGenerationResult(string filePath, int bankFilesUpdated, int collectionRequestsUpdated)
    {
        FilePath = filePath;
        BankFilesUpdated = bankFilesUpdated;
        CollectionRequestsUpdated = collectionRequestsUpdated;
    }
}
