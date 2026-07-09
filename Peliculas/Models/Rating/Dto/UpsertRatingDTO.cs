using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Rating.Dto
{
    public class UpsertRatingDTO
    {
        [Required]
        [Range(0.5, 5.0)]
        public decimal? Score { get; set; }
    }
}
