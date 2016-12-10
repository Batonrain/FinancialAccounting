using System;

namespace FinancialAccountingConstruction.DAL.Models.Contractors
{
    public class Contractor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int BuildingObjectId { get; set; }
        public string ContractNumbers { get; set; }
        public DateTime TimingOfWorks { get; set; }
        public decimal TotalCosts { get; set; }
        public string ContractDescriptions { get; set; }
        public int PaymentDay { get; set; }
    }
}
