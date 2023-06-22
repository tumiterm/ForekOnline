using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class LearnerModsRepository : IUnitOfWork<LearnerWorkplaceModules>
    {
        private readonly ApplicationDbContext _dbContext;
        public LearnerModsRepository(ApplicationDbContext dbContext)
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
        public async Task<LearnerWorkplaceModules> OnItemCreationAsync(LearnerWorkplaceModules lwm)
        {
            try
            {
                await _dbContext.AddAsync(lwm);

                return lwm;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add modules");
            }
        }
        public async Task<LearnerWorkplaceModules> OnLoadItemAsync(Guid WorkplaceModsId)
        {
            try
            {
                var lwm = await _dbContext.WorkplaceModules.Where(m => m.LearnerWorkplaceModulesId == WorkplaceModsId).FirstOrDefaultAsync();

                if (lwm is null)
                {
                    throw new Exception("Error!");
                }

                return lwm;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load modules");
            }
        }
        public async Task<List<LearnerWorkplaceModules>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.WorkplaceModules.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Workplace Modules could not be loaded!");
            }
        }
        public async Task<LearnerWorkplaceModules> OnModifyItemAsync(LearnerWorkplaceModules lwm)
        {
            LearnerWorkplaceModules results = new LearnerWorkplaceModules();

            try
            {
                results = await _dbContext.WorkplaceModules.FirstOrDefaultAsync(x => x.LearnerWorkplaceModulesId == lwm.LearnerWorkplaceModulesId);

                if (results != null)
                {

                    results.Progress = lwm.Progress;

                    results.CreatedBy = lwm.CreatedBy;

                    results.ModifiedBy = lwm.ModifiedBy;

                    results.Days = lwm.Days;

                    results.CourseId = lwm.CourseId;


                    results.IsActive = lwm.IsActive;

                    results.ModifiedBy = lwm?.ModifiedBy;

                    results.ModifiedOn = lwm?.ModifiedOn;

                    results.PlacementId = lwm.PlacementId;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: modules Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid Id)
        {
            LearnerWorkplaceModules lwm = new LearnerWorkplaceModules();

            int record = 0;

            try
            {
                lwm = await _dbContext.WorkplaceModules.FirstOrDefaultAsync(m => m.LearnerWorkplaceModulesId == Id);

                if (lwm != null)
                {
                    _dbContext.Remove(lwm);

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
