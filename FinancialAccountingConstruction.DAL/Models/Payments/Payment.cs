namespace FinancialAccountingConstruction.DAL.Models.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public int? PlannedId { get; set; }
        public decimal Sum { get; set; }
        public string Comment { get; set; }
        public bool IsInCash { get; set; }
    }
}
