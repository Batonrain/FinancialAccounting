namespace FinancialAccounting.Models.Payments
{
    public class PaymentSummaryViewModel
    {
        public decimal SummByContract { get; set; }
        public decimal PayedByContract { get; set; }
        public decimal NeedToPayByContract { get; set; }
    }
}