namespace EFT_Collections;

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

public class CreditorDefaults
{
    public string ClientCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string BankservUserCode { get; set; } = string.Empty;
    public string CreditorBranch { get; set; } = string.Empty;
    public string CreditorAccount { get; set; } = string.Empty;
    public string CreditorAbbreviation { get; set; } = string.Empty;
    public string TypeOfService { get; set; } = string.Empty;
}
