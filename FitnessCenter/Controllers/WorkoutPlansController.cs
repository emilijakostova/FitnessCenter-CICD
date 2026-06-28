using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class WorkoutPlansController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var plans = mongo.WorkoutPlans.Find(_ => true).ToList();

            if (User.IsInRole("Member"))
                plans = plans.Where(p => p.UserId == currentUserId).ToList();
            else if (User.IsInRole("Trainer"))
            {
                var myClientIds = sqlDb.Users
                    .Where(u => u.TrainerId == currentUserId)
                    .Select(u => u.Id).ToList();
                plans = plans.Where(p => myClientIds.Contains(p.UserId)).ToList();
            }

            foreach (var plan in plans)
            {
                var user = sqlDb.Users.Find(plan.UserId);
                ViewData["User_" + plan.Id] = user != null ? user.FirstName + " " + user.LastName : "";
                var program = mongo.WorkoutPrograms.Find(p => p.Id == plan.WorkoutProgramId).FirstOrDefault();
                ViewData["Program_" + plan.Id] = program?.Name ?? "";
            }

            return View(plans);
        }

        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var plan = mongo.WorkoutPlans.Find(p => p.Id == id).FirstOrDefault();
            if (plan == null) return HttpNotFound();
            var user = sqlDb.Users.Find(plan.UserId);
            ViewBag.UserName = user != null ? user.FirstName + " " + user.LastName : "";
            var program = mongo.WorkoutPrograms.Find(p => p.Id == plan.WorkoutProgramId).FirstOrDefault();
            ViewBag.ProgramName = program?.Name ?? "";
            return View(plan);
        }

        [Authorize(Roles = "Member")]
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = sqlDb.Users.Find(currentUserId);
            if (user?.TrainerId == null)
            {
                TempData["Error"] = "Немате назначен тренер.";
                return RedirectToAction("Index");
            }
            var programs = mongo.WorkoutPrograms
                .Find(p => p.TrainerId == user.TrainerId && p.TargerGender == user.Gender)
                .ToList();
            ViewBag.WorkoutProgramId = new SelectList(programs, "Id", "Name");
            ViewBag.UserFullName = user.FirstName + " " + user.LastName;
            return View(new WorkoutPlan { UserId = currentUserId, StrartDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public ActionResult Create([Bind(Include = "WorkoutProgramId,UserId,StrartDate,Notes")] WorkoutPlan plan)
        {
            if (ModelState.IsValid)
            {
                mongo.WorkoutPlans.InsertOne(plan);
                return RedirectToAction("Index");
            }
            var programs = mongo.WorkoutPrograms.Find(_ => true).ToList();
            ViewBag.WorkoutProgramId = new SelectList(programs, "Id", "Name");
            return View(plan);
        }

        [Authorize(Roles = "Trainer,Member")]
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var plan = mongo.WorkoutPlans.Find(p => p.Id == id).FirstOrDefault();
            if (plan == null) return HttpNotFound();

            var currentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Member") && plan.UserId != currentUserId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (User.IsInRole("Trainer"))
            {
                var member = sqlDb.Users.Find(plan.UserId);
                if (member?.TrainerId != currentUserId)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var programs = mongo.WorkoutPrograms.Find(_ => true).ToList();
            ViewBag.WorkoutProgramId = new SelectList(programs, "Id", "Name", plan.WorkoutProgramId);
            var user = sqlDb.Users.Find(plan.UserId);
            ViewBag.UserFullName = user != null ? user.FirstName + " " + user.LastName : "";
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer,Member")]
        public ActionResult Edit([Bind(Include = "Id,WorkoutProgramId,UserId,StrartDate,Notes")] WorkoutPlan plan)
        {
            if (ModelState.IsValid)
            {
                mongo.WorkoutPlans.ReplaceOne(p => p.Id == plan.Id, plan);
                return RedirectToAction("Index");
            }
            var programs = mongo.WorkoutPrograms.Find(_ => true).ToList();
            ViewBag.WorkoutProgramId = new SelectList(programs, "Id", "Name", plan.WorkoutProgramId);
            return View(plan);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var plan = mongo.WorkoutPlans.Find(p => p.Id == id).FirstOrDefault();
            if (plan == null) return HttpNotFound();
            var user = sqlDb.Users.Find(plan.UserId);
            ViewBag.UserName = user != null ? user.FirstName + " " + user.LastName : "";
            var program = mongo.WorkoutPrograms.Find(p => p.Id == plan.WorkoutProgramId).FirstOrDefault();
            ViewBag.ProgramName = program?.Name ?? "";
            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            mongo.WorkoutPlans.DeleteOne(p => p.Id == id);
            return RedirectToAction("Index");
        }
    }
}