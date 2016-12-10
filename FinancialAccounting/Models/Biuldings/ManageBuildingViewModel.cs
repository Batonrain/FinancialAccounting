using System.Collections.Generic;
using FinancialAccounting.Models.Contractors;

namespace FinancialAccounting.Models.Biuldings
{
    public class ManageBuildingViewModel
    {
        public BuildingViewModel BuildingMainInfo { get; set; }
        public IEnumerable<ShortContractorInfo> Contractors { get; set; }
    }
}