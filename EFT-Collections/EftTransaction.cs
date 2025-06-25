using System;

namespace EFT_Collections
{
    /// <summary>
    /// A logical representation of a single EFT transaction (a collection from a customer).
    /// This class is used to pass dynamic transaction data to the Writer.
    /// </summary>
    public class EftTransaction
    {
        /// <summary>
        /// The 6-digit branch code of the customer's (debtor's) bank account.
        /// </summary>
        public string HomingBranch { get; set; }

        /// <summary>
        /// The 11-digit account number of the customer (debtor).
        /// </summary>
        public string HomingAccountNumber { get; set; }

        /// <summary>
        /// The name of the customer (debtor) account holder.
        /// </summary>
        public string HomingAccountName { get; set; }

        /// <summary>
        /// The type of the customer's (debtor's) account.
        /// 1=Current, 2=Savings, 3=Transmission, 4=Bond, 6=Subscription Share.
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// The monetary value to be collected. E.g., 1501.00 for R1501.00.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The reference that will appear on the customer's (debtor's) bank statement.
        /// This is crucial for reconciliation.
        /// </summary>
        public string UserReference { get; set; }
    }
}
