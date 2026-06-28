using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public class WorkoutProgram
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Level { get; set; }

        [Range(1, 7)]
        public int DurationInDays { get; set; }

        [Required]
        public Gender TargerGender { get; set; }

        public string TrainerId { get; set; }

        public List<string> ExerciseIds { get; set; } = new List<string>();
    }
}