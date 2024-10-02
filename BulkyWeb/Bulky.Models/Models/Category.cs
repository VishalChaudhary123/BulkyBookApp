using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        // Which category to be displayed first on the page will be defined by the DisplayOder property
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Display order must be between 1 to 100.")]
        public int DisplayOrder { get; set; }
    }
}
