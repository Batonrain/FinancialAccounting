using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Payments
{
    public class CreatePaymentViewModel
    {
        [Required]
        [Display(Name = "Название платежа")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Сумма платежа в рублях")]
        public decimal Summ { get; set; }

        [Required]
        [Display(Name = "Тип платежа")]
        public bool IsInCash { get; set; }

        public Dictionary<string, bool> TypesOfPayments { get; set; }
        public int ContractorId { get; set; }
    }
}