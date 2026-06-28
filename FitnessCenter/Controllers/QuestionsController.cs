using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        [Authorize(Roles = "Member")]
        public ActionResult Ask()
        {
            var userId = User.Identity.GetUserId();
            var member = sqlDb.Users.Find(userId);
            if (member == null || string.IsNullOrEmpty(member.TrainerId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var trainer = sqlDb.Users.Find(member.TrainerId);
            ViewBag.TrainerName = trainer?.FirstName + " " + trainer?.LastName;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Ask(string Text)
        {
            var userId = User.Identity.GetUserId();
            var member = sqlDb.Users.Find(userId);
            if (member == null || string.IsNullOrEmpty(member.TrainerId))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            mongo.Questions.InsertOne(new Question
            {
                Text = Text,
                UserId = userId,
                TrainerId = member.TrainerId,
                DateAsked = DateTime.Now
            });
            return RedirectToAction("MyQuestions");
        }

        [Authorize(Roles = "Member")]
        public ActionResult MyQuestions()
        {
            var userId = User.Identity.GetUserId();
            var member = sqlDb.Users.Find(userId);
            var trainer = member?.TrainerId != null ? sqlDb.Users.Find(member.TrainerId) : null;
            ViewBag.TrainerName = trainer != null
                ? trainer.FirstName + " " + trainer.LastName
                : "Сè уште немате назначен тренер.";

            var questions = mongo.Questions
                .Find(q => q.UserId == userId)
                .SortByDescending(q => q.DateAsked)
                .ToList();
            return View(questions);
        }

        [Authorize(Roles = "Trainer")]
        public ActionResult ClientQuestions()
        {
            var trainerId = User.Identity.GetUserId();
            var questions = mongo.Questions
                .Find(q => q.TrainerId == trainerId)
                .SortByDescending(q => q.DateAsked)
                .ToList();

            foreach (var q in questions)
            {
                var user = sqlDb.Users.Find(q.UserId);
                ViewData["QUser_" + q.Id] = user != null ? user.FirstName + " " + user.LastName : "";
            }
            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public ActionResult Answer(string id, string Answer)
        {
            var trainerId = User.Identity.GetUserId();
            var question = mongo.Questions.Find(q => q.Id == id && q.TrainerId == trainerId).FirstOrDefault();
            if (question == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var update = Builders<Question>.Update
                .Set(q => q.Answer, Answer)
                .Set(q => q.DateAnswered, DateTime.Now);
            mongo.Questions.UpdateOne(q => q.Id == id, update);
            return RedirectToAction("ClientQuestions");
        }
    }
}