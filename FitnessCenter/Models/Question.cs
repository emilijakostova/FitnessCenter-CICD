using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string TrainerId { get; set; }

        [Required]
        public string Text { get; set; }

        public string Answer { get; set; }
        public DateTime DateAsked { get; set; }
        public DateTime? DateAnswered { get; set; }
    }
}