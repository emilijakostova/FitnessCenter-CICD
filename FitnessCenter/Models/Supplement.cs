using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class Supplement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }

        [Required]
        [Url]
        public string ProductUrl { get; set; }

        public string Description { get; set; }
        public bool Availability { get; set; }
    }
}