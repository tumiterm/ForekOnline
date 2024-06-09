using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class ApplicationRepository : IUnitOfWork<Application>
    {
        private readonly ApplicationDbContext _dbContext;
        public ApplicationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool DoesEntityExist<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            IQueryable<TEntity> data = _dbContext.Set<TEntity>();

            return data.Any(predicate);
        }
        public async Task<int> ItemSaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public async Task<Application> OnItemCreationAsync(Application application)
        {
            try
            {
                await _dbContext.AddAsync(application);

                return application;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add application");
            }
        }
        public async Task<Application> OnLoadItemAsync(Guid ApplicationId)
        {
            try
            {
                var application = await _dbContext.Applications.Where(m => m.ApplicationId == ApplicationId).FirstOrDefaultAsync();

                if (application is null)
                {
                    throw new Exception("Error!");
                }

                return application;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load application");
            }
        }
        public async Task<List<Application>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Applications.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Application could not be loaded!");
            }
        }
        public async Task<Application> OnModifyItemAsync(Application application)
        {
            Application results = new();

            try
            {
                results = await _dbContext.Applications.FirstOrDefaultAsync(x => x.ApplicationId == application.ApplicationId);

                if (results != null)
                {

                    results.ApplicantAddress = application.ApplicantAddress;

                    results.ApplicantGuardian = application.ApplicantGuardian;

                    results.ApplicantSurname = application.ApplicantSurname;

                    results.Email = application.Email;

                    results.Cellphone = application.Cellphone;

                    results.HighestQualification = application.HighestQualification;

                    results.ApplicantTitle = application.ApplicantTitle;

                    results.StatusReason = application.StatusReason;

                    results.FunderType = application.FunderType;

                    results.CourseId = application.CourseId;

                    results.PassportNumber = application.PassportNumber;

                    results.IDNumber = application.IDNumber;

                    results.ApplicantName = application.ApplicantName;

                    results.ApplicantAddress = application.ApplicantAddress;

                    results.Gender = application.Gender;

                    results.HighestQualDoc = application.HighestQualDoc;

                    results.IDPassDoc = application.IDPassDoc;

                    results.Status = application.Status;

                    results.ResidenceDoc = application.ResidenceDoc;

                    results.Selection = application.Selection;

                    results.StudyPermitCategory = application.StudyPermitCategory;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Application Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ApplicationId)
        {
            Application application = new();

            int record = 0;

            try
            {
                application = await _dbContext.Applications.FirstOrDefaultAsync(m => m.ApplicationId == ApplicationId);

                if (application != null)
                {
                    _dbContext.Remove(application);

                    record = await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Failed");
            }

            return record;
        }
    }
}
