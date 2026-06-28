using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FitnessCenter.Models
{
    public enum Goal
    {
        [Display(Name = "Намалување тежина")]
        WeightLoss,
        [Display(Name = "Зголемување мускулна маса")]
        MuscleGain,
        [Display(Name = "Подобрување на издржливост")]
        Endurance
    }

    public enum Gender
    {
        [Display(Name = "Машки")]
        Male,
        [Display(Name = "Женски")]
        Female
    }

    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Range(100, 250, ErrorMessage = "Внеси валидна висина (во сантиметри).")]
        public double Height { get; set; }
        [Range(30, 250, ErrorMessage = "Внеси валидна телесна маса (во килограми).")]
        public double Weight { get; set; }
        [Required]
        public Goal Goal { get; set; }
        public Gender Gender { get; set; }

        public string TrainerId { get; set; }

        public virtual ApplicationUser Trainer { get; set; }
        public virtual ICollection<ApplicationUser> Clients { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.Trainer)
                .WithMany(u => u.Clients)
                .HasForeignKey(u => u.TrainerId);
        }
    }
}