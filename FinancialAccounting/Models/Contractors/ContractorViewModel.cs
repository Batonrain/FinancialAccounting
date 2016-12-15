using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Contractors
{
    public class ContractorViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Названия подрядчика")]
        [StringLength(100, ErrorMessage = "Поле 'Названия подрядчика' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание подрядчика")]
        [StringLength(1000, ErrorMessage = "Поле 'Описание подрядчика' не должно быть короче 3х символов.", MinimumLength = 3)]
        public string Notes { get; set; }
    }
}