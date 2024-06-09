using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class LearnerModuleRepository : IUnitOfWork<LearnerWorkplaceModules>
    {
        private readonly ApplicationDbContext _dbContext;
        public LearnerModuleRepository(ApplicationDbContext dbContext)
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

                throw new Exception("Error: Failed to add Modules");
            }
        }
        public async Task<LearnerWorkplaceModules> OnLoadItemAsync(Guid Id)
        {
            try
            {
                var mods = await _dbContext.WorkplaceModules.Where(m => m.LearnerWorkplaceModulesId == Id).FirstOrDefaultAsync();

                if (mods is null)
                {
                    throw new Exception("Error!");
                }

                return mods;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<LearnerWorkplaceModules>> OnLoadItemsAsync()
        {
            try
            {
                var mods = await _dbContext.WorkplaceModules.ToListAsync();

                var getActiveMods = from n in mods

                                    where n.IsActive == true

                                    select n;

                return getActiveMods.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Modules could not be loaded!");
            }
        }
        public async Task<LearnerWorkplaceModules> OnModifyItemAsync(LearnerWorkplaceModules mods)
        {
            LearnerWorkplaceModules results = new LearnerWorkplaceModules();

            try
            {
                results = await _dbContext.WorkplaceModules.FirstOrDefaultAsync(x => x.LearnerWorkplaceModulesId == mods.LearnerWorkplaceModulesId);

                if (results != null)
                {

                    results.IsActive = mods.IsActive;

                    results.Student = mods.Student;

                    results.Progress = mods.Progress;

                    results.ModifiedBy = mods.ModifiedBy;

                    results.ModifiedOn = mods.ModifiedOn;

                    results.PlacementId = mods.PlacementId;

                    results.CourseId = mods.CourseId;

                    results.ModuleId = mods.ModuleId;

                    results.StartDate = mods.StartDate;

                    results.EndDate = mods.EndDate;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Module Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid Id)
        {
            LearnerWorkplaceModules module = new LearnerWorkplaceModules();

            int record = 0;

            try
            {
                module = await _dbContext.WorkplaceModules.FirstOrDefaultAsync(m => m.LearnerWorkplaceModulesId == Id);

                if (module != null)
                {
                    _dbContext.Remove(module);

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

