namespace X10Card.Models
{
    public class AllowanceDetails
    {
        public string? RegNo { get; set; }

        public string? AllowanceId { get; set; }

        public string? AllowanceDesc { get; set; }

        public string? ApplicationNo { get; set; }

        public string? XchNm { get; set; }

        public string? ApplicationStatus { get; set; }
        public string? ApplicationStatusCd { get; set; }
        public string? ApplicationStatusTexcolor { get; set; }

        public string? InstallmentAmt { get; set; }

        public string? StartDt { get; set; }

        public string? EndDt { get; set; }

        public string? TotInstallments { get; set; }

        public string? InstallmentsPaid { get; set; }

        public string? AmtPaid { get; set; }

        public string? LastInstallmentPaidOn { get; set; }
    }
}
