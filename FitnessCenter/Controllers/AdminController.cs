using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using FitnessCenter.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();
        private readonly MongoDbContext mongo = new MongoDbContext();

        public ActionResult Index() => View();

        public ActionResult AddTrainer() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainer(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(sqlDb));
            var user = new ApplicationUser
            {
                UserName = model.Email, Email = model.Email,
                FirstName = model.FirstName, LastName = model.LastName,
                Height = model.Height, Weight = model.Weight,
                Goal = Goal.WeightLoss, Gender = model.Gender
            };
            var result = userManager.Create(user, model.Password);
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Trainer");
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(model);
        }

        public ActionResult TrainersList()
        {
            var trainerRole = sqlDb.Roles.FirstOrDefault(r => r.Name == "Trainer");
            var trainers = trainerRole != null
                ? sqlDb.Users.Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id))
                    .OrderBy(u => u.UserName).ToList()
                : new List<ApplicationUser>();
            return PartialView("_TrainerList", trainers);
        }

        public ActionResult Dashboard()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsers = sqlDb.Users.Count()
            };

            var usages = mongo.SupplementUsages.Find(_ => true).ToList();
            var suppIds = usages.Select(u => u.SupplementId).Distinct().ToList();
            var supps = mongo.Supplements.Find(s => suppIds.Contains(s.Id)).ToList();

            vm.TopSupplements = usages
                .GroupBy(u => u.SupplementId)
                .Select(g => new SupplementStatistics
                {
                    SupplementName = supps.FirstOrDefault(s => s.Id == g.Key)?.Name ?? g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .Take(5).ToList();
            
            var plans = mongo.WorkoutPlans.Find(_ => true).ToList();
            var progIds = plans.Select(p => p.WorkoutProgramId).Distinct().ToList();
            var progs = mongo.WorkoutPrograms.Find(p => progIds.Contains(p.Id)).ToList();

            vm.TopPrograms = plans
                .GroupBy(p => p.WorkoutProgramId)
                .Select(g => new ProgramStatistics
                {
                    ProgramName = progs.FirstOrDefault(p => p.Id == g.Key)?.Name ?? g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .Take(5).ToList();

            return View(vm);
        }
    }
}