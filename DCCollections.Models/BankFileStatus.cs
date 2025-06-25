namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents the processing status of a bank file.
    /// </summary>
    public enum BankFileStatus
    {
        /// <summary>File has been accepted.</summary>
        Accepted = 1,
        /// <summary>File has been submitted but not yet accepted.</summary>
        Submitted = 2,
        /// <summary>File was rejected.</summary>
        Rejected = 3,
        /// <summary>File has been created but not yet submitted.</summary>
        Created = 7
    }
}
