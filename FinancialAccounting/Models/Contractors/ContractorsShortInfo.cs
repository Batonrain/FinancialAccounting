namespace FinancialAccounting.Models.Contractors
{
    public class ContractorsShortInfo
    {
        public string Name { get; set; }
        public int BuildingObjectId { get; set; }
        public decimal TotalCostsInCash { get; set; }
        public decimal TotalCostsCashless { get; set; }
    }
}