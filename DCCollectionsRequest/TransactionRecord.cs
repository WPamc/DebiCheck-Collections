namespace RMCollectionProcessor
{
    public class TransactionRecord
    {
        public string Filename { get; set; } = string.Empty;
        public string DataSetStatus { get; set; } = string.Empty;
        public string GenerationNumber { get; set; } = string.Empty;
        public string RecordSequenceNumber { get; set; } = string.Empty;
        public string SubmissionDate { get; set; } = string.Empty;
        public string PaymentInformation { get; set; } = string.Empty;
        public string RequestedCollectionDate { get; set; } = string.Empty;
        public string InstructedAmount { get; set; } = string.Empty;
        public string MandateReference { get; set; } = string.Empty;
        public string ContractReference { get; set; } = string.Empty;
        public string RelatedCycleDate { get; set; } = string.Empty;
        public string FileType { get; set; } = "RM Collections";
    }
}
