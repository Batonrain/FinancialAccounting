using System;

namespace FinancialAccountingConstruction.DAL.Models.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public int? PlannedId { get; set; }
        public decimal Summ { get; set; }
        public string Name { get; set; }
        public bool IsInCash { get; set; }
        public DateTime Date { get; set; }
        public int ExecutorId { get; set; }
        public int ContractorId { get; set; }
    }
}
