using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Contractors
{
    public class ContractorViewModel
    {
        public int Id { get; set; }
        public int BuildingObjectId { get; set; }

        [Display(Name = "Наименование подрядчика")]
        public string Name { get; set; }

        [Display(Name = "Заметки по подрядчику")]
        public string Notes { get; set; }

        [Display(Name = "Номера контрактов")]
        public string ContractNumbers { get; set; }

        [Display(Name = "Сроки окончания работ")]
        public string TimingOfWorks { get; set; }

        [Display(Name = "Итоговая стоимость работ")]
        public decimal TotalCosts { get; set; }

        [Display(Name = "Описание контракта")]
        public string ContractDescriptions { get; set; }

        [Display(Name = "Ежемесячная дата оплаты")]
        public string PaymentDay { get; set; }
    }
}