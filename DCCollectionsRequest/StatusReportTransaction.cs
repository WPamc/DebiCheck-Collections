using System;

namespace RMCollectionProcessor
{
    /// <summary>
    /// Represents a single, consolidated transaction from a Status Report file.
    /// </summary>
    public class StatusReportTransaction
    {
        public string TransactionStatus { get; set; } = string.Empty;
        public string ContractReference { get; set; } = string.Empty;
        public string OriginalPaymentInformation { get; set; } = string.Empty;
        public string? ActionDate { get; set; }
        public string? EffectiveDate { get; set; }
        public string? RejectReasonCode { get; set; }
        public string? RejectReasonDescription { get; set; }
    }
}