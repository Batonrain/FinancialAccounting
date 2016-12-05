using System;
using System.ComponentModel.DataAnnotations;
using FinancialAccountingConstruction.DAL.Models.Users;

namespace FinancialAccounting.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        public Guid? UserId { get;set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Выберите роль")]
        public UserRoles Role { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Текущий пароль")]
        //public string OldPassword { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "{0} должен быть хотя бы {2} символов в длину.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Новый пароль")]
        //public string NewPassword { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Подтверждение нового пароля")]
        //[Compare("NewPassword", ErrorMessage = "Поля 'Новый пароль' и 'Подтверждение нового пароля' должны совпадать.")]
        //public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть хотя бы {2} символов в длину.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Поля 'Пароль' и 'Подтверждение пароля' должны совпадать.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Выберите роль")]
        public UserRoles Role { get; set; }
    }
}
