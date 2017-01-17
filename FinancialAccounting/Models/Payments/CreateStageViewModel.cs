using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Payments
{
    public class CreateStageViewModel
    {

        public int Id { get; set; }

        public int ContractorId { get; set; }

        [Required]
        [Display(Name = "Название этапа")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Аванс")]
        public decimal Prepayment { get; set; }

        [Required]
        [Display(Name = "Окончательный платёж")]
        public decimal FinalPayment { get; set; }

        [Required]
        [Display(Name = "Дата окончания работ")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfEnding { get; set; }

        [Required]
        [Display(Name = "Дата аванса")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfPrepayment { get; set; }

        [Required]
        [Display(Name = "Дата окончательного платежа")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfFinalPayment { get; set; }

        public bool IsInCash { get; set; }
    }
}