﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialAccounting.Models.Contractors
{
    public class UpdateContractorViewModel
    {

        public int Id { get; set; }

        public int BuildingObjectId { get; set; }

        [Required]
        [Display(Name = "Наименование подрядчика")]
        [StringLength(100, ErrorMessage = "Поле 'Именование подрядчика' не должно превышать 100 символов и не должно быть короче 3.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание контракта")]
        [StringLength(2000, ErrorMessage = "Поле 'Описание контракта' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string ContractDescriptions { get; set; }

        [Required]
        [Display(Name = "Заметки по подрядчику")]
        [StringLength(2000, ErrorMessage = "Поле 'Описание контракта' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Номера контрактов")]
        [StringLength(2000, ErrorMessage = "Поле 'Описание контракта' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string ContractNumbers { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(Name = "Сроки окончания работ в формате ДД.ММ.ГГГГ")]
        public DateTime TimingOfWorks { get; set; }

        [Required]
        [Range(1, 30)]
        [Display(Name = "Ежемесячная дата оплаты в формате ДД")]
        public int PaymentDay { get; set; }

        [Required]
        [Display(Name = "Итоговая стоимость работ")]
        public decimal TotalCosts { get; set; }
    }
}