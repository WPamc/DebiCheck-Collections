using System;

namespace RMCollectionProcessor.Models
{
    public class BillingCollectionRequest
    {
        public int RowId { get; set; }
        public DateTime DateRequested { get; set; }
        public string? SubSSN { get; set; }
        public string? Reference { get; set; }
        public string? DeductionReference { get; set; }
        public string? AmountRequested { get; set; }
        public bool? Result { get; set; }
        public int? Method { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? LastChangeBy { get; set; }
        public DateTime? LastChangeDate { get; set; }
    }
}
