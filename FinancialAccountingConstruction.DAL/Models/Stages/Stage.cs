using System;

namespace FinancialAccountingConstruction.DAL.Models.Stages
{
    public class Stage
    {
        public int Id { get; set; }

        public int ContractorId { get; set; }

        public string Name { get; set; }

        public DateTime DateOfEnding { get; set; }

        public DateTime DateOfPrepayment { get; set; }

        public DateTime DateOfFinalPayment { get; set; }

        public decimal Prepayment { get; set; }

        public decimal FinalPayment { get; set; }

        public decimal TotalPayment { get; set; }

        public bool IsInCash { get; set; }
    }
}
