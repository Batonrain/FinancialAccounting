namespace FinancialAccountingConstruction.DAL.Models.Contracts
{
    public class Contract
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int ContractorId { get; set; }
        public int BuildingObjectId { get; set; }
        public string Description { get; set; }
    }
}