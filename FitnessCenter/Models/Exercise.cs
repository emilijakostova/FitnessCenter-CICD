using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessCenter.Models
{
    public enum MuscleGroup
    {
        [Display(Name = "Гради")]   Chest,
        [Display(Name = "Грб")]     Back,
        [Display(Name = "Рамо")]    Shoulders,
        [Display(Name = "Нозе")]    Legs,
        [Display(Name = "Бицепс")] Biceps,
        [Display(Name = "Трицепс")] Triceps,
        [Display(Name = "Стомачни")] Abs,
        [Display(Name = "Цело тело")] FullBody
    }

    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public MuscleGroup MuscleGroup { get; set; }

        [Required]
        public string Description { get; set; }

        public string MediaUrl { get; set; }
    }
}