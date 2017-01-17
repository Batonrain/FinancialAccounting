using System;
using FinancialAccounting.Models.Buildings;

namespace FinancialAccounting.Models.Payments
{
    public class StageViewModel
    {
        public int Id { get; set; }

        public int ContractorId { get; set; }

        public string Name { get; set; }

        public string DateOfEnding { get; set; }

        public string DateOfPrepayment { get; set; }

        public string DateOfFinalPayment { get; set; }

        public string Prepayment { get; set; }

        public string FinalPayment { get; set; }

        public string SummOfPayment { get; set; }

        public string TotalPayment { get; set; }

        public bool IsInCash { get; set; }

        public Status Status { get; set; }
    }
}