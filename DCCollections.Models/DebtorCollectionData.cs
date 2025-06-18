using System;

namespace RMCollectionProcessor.Models
{
    /// <summary>
    /// Helper class to hold the dynamic debtor-specific data used when creating a file.
    /// </summary>
    public class DebtorCollectionData
    {
        public string PaymentInformation { get; set; } = string.Empty;
        public DateTime RequestedCollectionDate { get; set; }
        public int TrackingPeriod { get; set; }
        public string DebitSequence { get; set; } = string.Empty;
        public string EntryClass { get; set; } = string.Empty;
        public decimal InstructedAmount { get; set; }
        public string MandateReference { get; set; } = string.Empty;
        public string DebtorBankBranch { get; set; } = string.Empty;
        public string DebtorName { get; set; } = string.Empty;
        public string DebtorAccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string ContractReference { get; set; } = string.Empty;
        public DateTime RelatedCycleDate { get; set; }
    }
}
