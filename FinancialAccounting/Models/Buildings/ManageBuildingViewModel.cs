using System.Collections.Generic;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Payments;

namespace FinancialAccounting.Models.Buildings
{
    public class ManageBuildingViewModel
    {
        public BuildingViewModel BuildingMainInfo { get; set; }
        public List<ContractorViewModel> Contractors { get; set; }

        public string ActualizationDate { get; set; }
        public string ActualizationPerson { get; set; }

        public TotalPaymentViewModel TotalPayment { get; set; }
    }
}