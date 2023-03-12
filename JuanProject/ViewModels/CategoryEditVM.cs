using System.ComponentModel.DataAnnotations;

namespace JuanProject.ViewModels
{
    public class CategoryEditVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
