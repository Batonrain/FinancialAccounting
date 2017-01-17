using System;
using System.Collections.Generic;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Payments;

namespace FinancialAccounting.Models.Contractors
{
    public class ContractorViewModel
    {
        public int Id { get; set; }
        public int BuildingObjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsInCahs { get; set; }
        public string TypeText { get; set; }

        public DateTime ActualisationDate { get; set; }
        public string ActualisationPerson { get; set; }
        public Status Status { get; set; }
        public List<StageViewModel> Stages { get; set; }
        public PaymentSummaryViewModel PaymentsSummary { get; set; }
    }
}