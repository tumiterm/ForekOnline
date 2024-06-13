using Microsoft.EntityFrameworkCore;
using ElecPOE.Models;


namespace ElecPOE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<StudentAttachment> StudentAttachments { get; set; }
        public DbSet<AssessmentAttachment> AssessmentAttachments { get; set; }
        //public DbSet<Material> Material { get; set; }
        public DbSet<Training> Training { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Evidence> Evidence { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProgressReport> Results { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<LearnerFinance> Finance { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<LessonPlan> LessonPlan { get; set; }
        public DbSet<Placement> Placements { get; set; }
        public DbSet<ContactPerson> ContactPerson{ get; set; }
        public DbSet<Address> Address{ get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<NatedEngineering> NatedEngineering { get; set; }
        public DbSet<LearnerWorkplaceModules> WorkplaceModules { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Medical> Medicals { get; set; }
        public DbSet<Models.File> Files { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ApplicationRejection> Rejections { get; set; }
        public DbSet<Models.License> Licenses { get; set; }




    }
}
