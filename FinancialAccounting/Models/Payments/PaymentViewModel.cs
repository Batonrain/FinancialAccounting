using System;

namespace FinancialAccounting.Models.Payments
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Summ { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Executor { get; set; }
    }
}