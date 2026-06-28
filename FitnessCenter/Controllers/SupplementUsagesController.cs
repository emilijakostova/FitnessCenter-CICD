using System;
using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class SupplementUsagesController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();

        [Authorize]
        public ActionResult AddUsage(string supplementId) =>
            View(new SupplementUsage { SupplementId = supplementId.ToString(), DateStarted = DateTime.Today });

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUsage(SupplementUsage usage)
        {
            if (ModelState.IsValid)
            {
                usage.UserId = User.Identity.GetUserId();
                mongo.SupplementUsages.InsertOne(usage);
                return RedirectToAction("MyUsages");
            }
            return View(usage);
        }

        [Authorize]
        public ActionResult MyUsages()
        {
            string userId = User.Identity.GetUserId();
            var usages = mongo.SupplementUsages
                .Find(u => u.UserId == userId)
                .SortByDescending(u => u.DateStarted)
                .ToList();

            var suppIds = usages.Select(u => u.SupplementId).Distinct().ToList();
            var supps = mongo.Supplements.Find(s => suppIds.Contains(s.Id)).ToList();
            var reviews = mongo.SupplementReviews.Find(r => r.UserId == userId).ToList();

            foreach (var u in usages)
            {
                var s = supps.FirstOrDefault(x => x.Id == u.SupplementId);
                ViewData["Supp_" + u.Id] = s?.Name ?? "";

                var review = reviews.FirstOrDefault(r => r.SupplementId == u.SupplementId);
                if (review != null)
                    ViewData["Review_" + u.Id] = review.Rating;
            }
            return View(usages);
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            var usage = mongo.SupplementUsages.Find(u => u.Id == id).FirstOrDefault();
            if (usage == null || usage.UserId != User.Identity.GetUserId()) return HttpNotFound();
            var supp = mongo.Supplements.Find(s => s.Id == usage.SupplementId).FirstOrDefault();
            ViewBag.SupplementName = supp?.Name;
            return View(usage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(SupplementUsage model)
        {
            if (ModelState.IsValid)
            {
                var existing = mongo.SupplementUsages.Find(u => u.Id == model.Id).FirstOrDefault();
                if (existing == null || existing.UserId != User.Identity.GetUserId()) return HttpNotFound();
                existing.Notes = model.Notes;
                existing.Dosage = model.Dosage;
                existing.DateStarted = model.DateStarted;
                mongo.SupplementUsages.ReplaceOne(u => u.Id == model.Id, existing);
                return RedirectToAction("MyUsages");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Delete(string id)
        {
            var usage = mongo.SupplementUsages.Find(u => u.Id == id).FirstOrDefault();
            if (usage == null || usage.UserId != User.Identity.GetUserId()) return HttpNotFound();
            var supp = mongo.Supplements.Find(s => s.Id == usage.SupplementId).FirstOrDefault();
            ViewBag.SupplementName = supp?.Name ?? "";
            return View(usage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(string id)
        {
            var usage = mongo.SupplementUsages.Find(u => u.Id == id).FirstOrDefault();
            if (usage == null || usage.UserId != User.Identity.GetUserId()) return HttpNotFound();
            mongo.SupplementUsages.DeleteOne(u => u.Id == id);
            return RedirectToAction("MyUsages");
        }
    }
}