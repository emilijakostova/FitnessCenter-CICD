using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext sqlDb = new ApplicationDbContext();

        public ActionResult AssignClients()
        {
            var memberRole = sqlDb.Roles.FirstOrDefault(r => r.Name == "Member");
            var available = memberRole != null
                ? sqlDb.Users.Where(u =>
                    u.Roles.Any(r => r.RoleId == memberRole.Id) &&
                    u.TrainerId == null).ToList()
                : new List<ApplicationUser>();

            ViewBag.Clients = new SelectList(available, "Id", "Email");
            return View();
        }

        [HttpPost]
        public ActionResult AddClient(string clientId)
        {
            var trainerId = User.Identity.GetUserId();
            var client = sqlDb.Users.Find(clientId);
            if (client != null && client.TrainerId == null)
            {
                client.TrainerId = trainerId;
                sqlDb.SaveChanges();
            }
            return PartialView("_MyClientsPartial", GetMyClients(trainerId));
        }

        public PartialViewResult GetMyClientsPartial()
        {
            var trainerId = User.Identity.GetUserId();
            return PartialView("_MyClientsPartial", GetMyClients(trainerId));
        }

        [HttpPost]
        public ActionResult RemoveClient(string clientId)
        {
            var trainerId = User.Identity.GetUserId();
            var client = sqlDb.Users.Find(clientId);
            if (client == null || client.TrainerId != trainerId) return HttpNotFound();
            client.TrainerId = null;
            sqlDb.SaveChanges();
            return RedirectToAction("AssignClients");
        }

        private List<ApplicationUser> GetMyClients(string trainerId) =>
            sqlDb.Users.Where(u => u.TrainerId == trainerId).ToList();
    }
}