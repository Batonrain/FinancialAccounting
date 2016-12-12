using System;

namespace FinancialAccountingConstruction.DAL.Models.Payments
{
    public class PlannedPaymentsDate
    {
        public int Id { get; set; }
        public int ContractorId { get; set; }
        public DateTime Date { get; set; }
    }
}
