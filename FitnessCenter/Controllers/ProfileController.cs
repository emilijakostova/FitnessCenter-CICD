using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();
        private readonly MongoDbContext mongo = new MongoDbContext();

        public ActionResult Index(string id = null)
        {
            string currentUserId = User.Identity.GetUserId();
            string targetUserId = id ?? currentUserId;

            var user = sqlDb.Users.Find(targetUserId);
            if (user == null) return HttpNotFound();

            var lastEntry = mongo.ProgressEntries
                .Find(p => p.UserId == targetUserId)
                .SortByDescending(p => p.Date)
                .FirstOrDefault();

            if (lastEntry != null && user.Weight != lastEntry.Weight)
            {
                user.Weight = lastEntry.Weight;
                sqlDb.SaveChanges();
            }

            return View(new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                Gender = user.Gender,
            });
        }

        [Authorize(Roles = "Member")]
        public ActionResult Edit()
        {
            var user = sqlDb.Users.Find(User.Identity.GetUserId());
            return View(new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                Gender = user.Gender,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public ActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = sqlDb.Users.Find(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Height = model.Height;
            user.Weight = model.Weight;
            user.Goal = model.Goal;
            user.Gender = model.Gender;
            sqlDb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}