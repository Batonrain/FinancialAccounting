using System.ComponentModel.DataAnnotations;

namespace FinancialAccounting.Models.Buildings
{
    public class BuildingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название объекта")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание/справка об объекте")]
        public string Description { get; set; }
    }
}