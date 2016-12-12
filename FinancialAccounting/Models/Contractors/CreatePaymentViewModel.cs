﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FinancialAccounting.Models.Contractors
{
    public class CreatePaymentViewModel
    {
        public int ContractorId { get; set; }
        public string ContractorName { get; set; }

        [Required]
        [Display(Name = "Сумма платежа в рублях")]
        public decimal Summ { get; set; }

        [Required]
        [Display(Name = "Плановый")]
        public bool IsPlanned { get; set; }

        [Display(Name = "Плановый за период")]
        public int PlannedForId { get; set; }
        public IEnumerable<SelectListItem> PeriodsOfPayments { get; set; }
    }
}