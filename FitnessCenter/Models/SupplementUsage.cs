using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class SupplementUsage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SupplementId { get; set; }
        public string UserId { get; set; }
        public string Notes { get; set; }
        public string Dosage { get; set; }
        public DateTime DateStarted { get; set; }
    }
}