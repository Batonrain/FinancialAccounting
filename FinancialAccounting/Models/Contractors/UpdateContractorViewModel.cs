﻿using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Contractors
{
    public class UpdateContractorViewModel
    {
        public int Id { get; set; }

        public int BuildingObjectId { get; set; }

        [Required]
        [Display(Name = "Имя подрядчика")]
        [StringLength(100, ErrorMessage = "Поле 'Имя подрядчика' не должно превышать 100 символов и не должно быть короче 3.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Сводка по контракту")]
        [StringLength(2000, ErrorMessage = "Поле 'Сводка по подрядчику' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string Descriptions { get; set; }

        [Required]
        [Display(Name = "Оплата в наличных")]
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public decimal TotalCostsInCash { get; set; }

        [Required]
        [Display(Name = "Оплата безналом")]
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public decimal TotalCostsCashless { get; set; }

        [Display(Name = "Даты обязательных платежей")]
        public string RequriedDates { get; set; }
    }
}