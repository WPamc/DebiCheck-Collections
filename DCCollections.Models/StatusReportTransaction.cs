using System;

namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Represents a single, consolidated transaction from a Status Report file.
    /// </summary>
    public class StatusReportTransaction
    {
        /// <summary>
        /// The BankServ User Code Generation Number from the Status User Set Header.
        /// </summary>
        public string GenerationNumber { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string ContractReference { get; set; } = string.Empty;
        public string OriginalPaymentInformation { get; set; } = string.Empty;
        public string? ActionDate { get; set; }
        public string? EffectiveDate { get; set; }
        public string? RejectReasonCode { get; set; }
        public string? RejectReasonDescription { get; set; }
        public string? InstructedAmount { get; set; }
    }
}