using System.Configuration;
using MongoDB.Driver;

namespace FitnessCenter.Models
{
    public class MongoDbContext
    {
        private static readonly IMongoDatabase _db;

        static MongoDbContext()
        {
            var connectionString = ConfigurationManager
                .ConnectionStrings["MongoConnection"].ConnectionString;
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase("FitnessCenterDb");
        }

        public static MongoDbContext Create() => new MongoDbContext();

        public IMongoCollection<Exercise> Exercises =>
            _db.GetCollection<Exercise>("exercises");

        public IMongoCollection<WorkoutProgram> WorkoutPrograms =>
            _db.GetCollection<WorkoutProgram>("workoutprograms");

        public IMongoCollection<WorkoutPlan> WorkoutPlans =>
            _db.GetCollection<WorkoutPlan>("workoutplans");

        public IMongoCollection<ProgressEntry> ProgressEntries =>
            _db.GetCollection<ProgressEntry>("progressentries");

        public IMongoCollection<Supplement> Supplements =>
            _db.GetCollection<Supplement>("supplements");

        public IMongoCollection<SupplementUsage> SupplementUsages =>
            _db.GetCollection<SupplementUsage>("supplementusages");

        public IMongoCollection<SupplementReview> SupplementReviews =>
            _db.GetCollection<SupplementReview>("supplementreviews");

        public IMongoCollection<Question> Questions =>
            _db.GetCollection<Question>("questions");
    }
}