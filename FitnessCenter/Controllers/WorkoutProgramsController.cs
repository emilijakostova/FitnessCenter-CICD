using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class WorkoutProgramsController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        public ActionResult Index()
        {
            var programs = mongo.WorkoutPrograms.Find(_ => true).ToList();
            foreach (var p in programs)
            {
                if (!string.IsNullOrEmpty(p.TrainerId))
                {
                    var trainer = sqlDb.Users.Find(p.TrainerId);
                    ViewData["Trainer_" + p.Id] = trainer != null
                        ? trainer.FirstName + " " + trainer.LastName : "";
                }
            }
            return View(programs);
        }

        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var program = mongo.WorkoutPrograms.Find(p => p.Id == id).FirstOrDefault();
            if (program == null) return HttpNotFound();

            var exercises = program.ExerciseIds.Any()
                ? mongo.Exercises.Find(e => program.ExerciseIds.Contains(e.Id)).ToList()
                : new List<Exercise>();

            ViewBag.Exercises = exercises;
            ViewBag.AllExercises = new SelectList(
                mongo.Exercises.Find(_ => true).ToList(), "Id", "Description");
            return View(program);
        }

        public ActionResult Create()
        {
            var trainerRole = sqlDb.Roles.FirstOrDefault(r => r.Name == "Trainer");
            var trainers = trainerRole != null
                ? sqlDb.Users.Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id)).ToList()
                : new List<ApplicationUser>();
            ViewBag.TrainerId = new SelectList(trainers, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,Level,DurationInDays,TargerGender,TrainerId")] WorkoutProgram program)
        {
            if (ModelState.IsValid)
            {
                mongo.WorkoutPrograms.InsertOne(program);
                return RedirectToAction("Index");
            }
            ViewBag.TrainerId = new SelectList(sqlDb.Users.ToList(), "Id", "FirstName");
            return View(program);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var program = mongo.WorkoutPrograms.Find(p => p.Id == id).FirstOrDefault();
            if (program == null) return HttpNotFound();
            var trainerRole = sqlDb.Roles.FirstOrDefault(r => r.Name == "Trainer");
            var trainers = trainerRole != null
                ? sqlDb.Users.Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id)).ToList()
                : new List<ApplicationUser>();
            ViewBag.TrainerId = new SelectList(trainers, "Id", "FirstName", program.TrainerId);
            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Level,DurationInDays,TargerGender,TrainerId")] WorkoutProgram program)
        {
            if (ModelState.IsValid)
            {
                mongo.WorkoutPrograms.ReplaceOne(p => p.Id == program.Id, program);
                return RedirectToAction("Index");
            }
            ViewBag.TrainerId = new SelectList(sqlDb.Users.ToList(), "Id", "FirstName");
            return View(program);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var program = mongo.WorkoutPrograms.Find(p => p.Id == id).FirstOrDefault();
            if (program == null) return HttpNotFound();
            return View(program);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            mongo.WorkoutPrograms.DeleteOne(p => p.Id == id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddExercise(string WorkoutProgramId, string ExerciseId)
        {
            var update = Builders<WorkoutProgram>.Update.AddToSet(p => p.ExerciseIds, ExerciseId);
            mongo.WorkoutPrograms.UpdateOne(p => p.Id == WorkoutProgramId, update);
            return RedirectToAction("Details", new { id = WorkoutProgramId });
        }

        public ActionResult RemoveExercise(string workoutProgramId, string exerciseId)
        {
            var update = Builders<WorkoutProgram>.Update.Pull(p => p.ExerciseIds, exerciseId);
            mongo.WorkoutPrograms.UpdateOne(p => p.Id == workoutProgramId, update);
            return RedirectToAction("Details", new { id = workoutProgramId });
        }
    }
}