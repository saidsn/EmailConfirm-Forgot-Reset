
using System.ComponentModel.DataAnnotations;

namespace JuanProject.Models
{
    public class Contact : BaseEntity
    {
        [Required,MaxLength(20)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
