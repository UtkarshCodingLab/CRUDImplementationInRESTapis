using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace EComWebAPI.Models.Dto
{
    public class CategoryCreateDTO
    {
        [Required]
        //[DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        //[DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }

    }
}
