using Microsoft.Build.Framework;

namespace JuanProject.ViewModels
{
    public class CategoryCreateVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
