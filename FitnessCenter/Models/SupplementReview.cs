using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class SupplementReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string SupplementId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Оцената мора да биде помеѓу 1 и 5.")]
        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}