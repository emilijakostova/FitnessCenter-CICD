using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class ProgressEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(30, 250, ErrorMessage = "Внеси валидна телесна маса (во килограми)")]
        public double Weight { get; set; }

        [Display(Prompt = "Гради,Струк,Рака -> e.g. 95,80,32")]
        public string Measurements { get; set; }
    }
}