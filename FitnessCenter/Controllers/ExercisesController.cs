using System.Web.Mvc;
using System.Net;
using FitnessCenter.Models;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();

        public ActionResult Index() =>
            View(mongo.Exercises.Find(_ => true).ToList());

        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var exercise = mongo.Exercises.Find(e => e.Id == id).FirstOrDefault();
            if (exercise == null) return HttpNotFound();
            return View(exercise);
        }

        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MuscleGroup,Description,MediaUrl")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                mongo.Exercises.InsertOne(exercise);
                return RedirectToAction("Index");
            }
            return View(exercise);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var exercise = mongo.Exercises.Find(e => e.Id == id).FirstOrDefault();
            if (exercise == null) return HttpNotFound();
            return View(exercise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MuscleGroup,Description,MediaUrl")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                mongo.Exercises.ReplaceOne(e => e.Id == exercise.Id, exercise);
                return RedirectToAction("Index");
            }
            return View(exercise);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var exercise = mongo.Exercises.Find(e => e.Id == id).FirstOrDefault();
            if (exercise == null) return HttpNotFound();
            return View(exercise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            mongo.Exercises.DeleteOne(e => e.Id == id);
            return RedirectToAction("Index");
        }
    }
}