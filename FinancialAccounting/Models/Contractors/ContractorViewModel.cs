using System.Collections.Generic;
using FinancialAccounting.Models.Payments;

namespace FinancialAccounting.Models.Contractors
{
    public class ContractorPaymentsViewModel
    {
        public int Id { get; set; }
        public int BuildingObjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TotalCostsInCash { get; set; }
        public decimal TotalCostsCashless { get; set; }

        public string Status { get;set }

        public List<PaymentViewModel> Payments { get; set; }
        public PaymentSummaryViewModel PaymentsSummary { get; set; }
    }
}