using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Rating.Dto
{
    public class RatingDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpsertRatingDTO
    {
        [Required]
        [Range(0.5, 5.0)]
        public decimal Score { get; set; }
    }
}