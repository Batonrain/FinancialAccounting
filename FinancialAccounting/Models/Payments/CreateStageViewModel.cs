using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Payments
{
    public class CreateStageViewModel
    {

        public int Id { get; set; }

         [Display(Name = "Имя подрядчика")]
        public int ContractorId { get; set; }

        [Required]
        [Display(Name = "Название этапа")]
        public string Name { get; set; }

        
        [Display(Name = "Аванс")]
        [DisplayFormat(DataFormatString = "C2", ApplyFormatInEditMode = true)]
        public decimal Prepayment { get; set; }

        [Required]
        [Display(Name = "Окончательный платёж")]
        [DisplayFormat(DataFormatString = "C2", ApplyFormatInEditMode = true)]
        public decimal FinalPayment { get; set; }

        [Display(Name = "Дата окончания работ")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfEnding { get; set; }

        [Display(Name = "Дата аванса")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfPrepayment { get; set; }

        [Display(Name = "Дата окончательного платежа")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfFinalPayment { get; set; }

        [Display(Name = "Тип оплаты")]
        public bool IsInCash { get; set; }


        public decimal TotalPayed { get; set; }
        public decimal PrepaymentPayed { get; set; }
        public decimal FinalPaymentPayed { get; set; }
        public string ActualizedBy { get; set; }

        public List<KeyValuePair<bool, string>> Types { get; set; }
        public List<KeyValuePair<int, string>> ContractorsSelect { get; set; }
    }
}