using System.ComponentModel.DataAnnotations;

namespace JuanProject.ViewModels.AccountVM
{
    public class ForgotPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
