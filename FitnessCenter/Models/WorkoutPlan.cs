using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class WorkoutPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string WorkoutProgramId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime StrartDate { get; set; }

        public string Notes { get; set; }
    }
}