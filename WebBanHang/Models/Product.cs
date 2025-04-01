using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public List<Rating> Ratings { get; set; } = new();

        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Stars) : 0;
    }
}
