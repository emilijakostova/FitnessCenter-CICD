using System.Linq;
using System.Web.Mvc;
using FitnessCenter.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace FitnessCenter.Controllers
{
    public class SupplementReviewsController : Controller
    {
        private readonly MongoDbContext mongo = new MongoDbContext();

        [HttpPost]
        [Authorize(Roles = "Member")]
        public ActionResult SubmitReview(SupplementReview review)
        {
            review.UserId = User.Identity.GetUserId();
            mongo.SupplementReviews.InsertOne(review);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public ActionResult DeleteReview(string supplementId)
        {
            string userId = User.Identity.GetUserId();
            var review = mongo.SupplementReviews
                .Find(r => r.SupplementId == supplementId && r.UserId == userId)
                .FirstOrDefault();
            if (review == null) return HttpNotFound();
            mongo.SupplementReviews.DeleteOne(r => r.Id == review.Id);
            return new HttpStatusCodeResult(200);
        }
    }
}