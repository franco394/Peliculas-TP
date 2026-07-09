using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Review.Dto
{
    public class UpdateReviewDTO
    {
        [Required]
        [MinLength(10)]
        public string? Content { get; set; } = string.Empty;
        public bool? ContainsSpoilers { get; set; } = false;
    }
}
