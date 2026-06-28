using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class ProgressEntriesController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        [Authorize(Roles = "Member")]
        
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var entries = mongo.ProgressEntries
                .Find(p => p.UserId == userId)
                .SortByDescending(p => p.Date)
                .ToList();

            foreach (var e in entries)
            {
                var user = sqlDb.Users.Find(e.UserId);
                ViewData["User_" + e.Id] = user != null ? user.FirstName + " " + user.LastName : "";
            }
            return View(entries);
        }

        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var entry = mongo.ProgressEntries.Find(e => e.Id == id).FirstOrDefault();
            if (entry == null) return HttpNotFound();
            var user = sqlDb.Users.Find(entry.UserId);
            ViewBag.UserName = user != null ? user.FirstName + " " + user.LastName : "";
            return View(entry);
        }

        public ActionResult Create() => View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date,Weight,Measurements")] ProgressEntry entry)
        {
            ModelState.Remove("Id");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                entry.UserId = User.Identity.GetUserId();
                mongo.ProgressEntries.InsertOne(entry);

                var user = sqlDb.Users.Find(entry.UserId);
                if (user != null && user.Weight != entry.Weight)
                {
                    user.Weight = entry.Weight;
                    sqlDb.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(entry);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var entry = mongo.ProgressEntries.Find(e => e.Id == id).FirstOrDefault();
            if (entry == null) return HttpNotFound();
            var user = sqlDb.Users.Find(entry.UserId);
            ViewBag.UserName = user != null ? user.FirstName + " " + user.LastName : "";
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Date,Weight,Measurements")] ProgressEntry entry)
        {
            if (ModelState.IsValid)
            {
                mongo.ProgressEntries.ReplaceOne(e => e.Id == entry.Id, entry);
                return RedirectToAction("Index");
            }
            return View(entry);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var entry = mongo.ProgressEntries.Find(e => e.Id == id).FirstOrDefault();
            if (entry == null) return HttpNotFound();
            var user = sqlDb.Users.Find(entry.UserId);
            ViewBag.UserName = user != null ? user.FirstName + " " + user.LastName : "";
            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            mongo.ProgressEntries.DeleteOne(e => e.Id == id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Member")]
        public ActionResult Chart()
        {
            var userId = User.Identity.GetUserId();
            var entries = mongo.ProgressEntries
                .Find(p => p.UserId == userId)
                .SortBy(p => p.Date)
                .ToList();
            return View(entries);
        }
    }
}