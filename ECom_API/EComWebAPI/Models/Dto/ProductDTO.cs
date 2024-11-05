using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EComWebAPI.Models.Dto
{
    public class ProductDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        //[ValidateNever]
        public String ImageUrl { get; set; }
    }
}
