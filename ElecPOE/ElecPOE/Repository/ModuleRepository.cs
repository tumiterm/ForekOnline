using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class ModuleRepository : IUnitOfWork<Module>
    {
        private readonly ApplicationDbContext _dbContext;
        public ModuleRepository(ApplicationDbContext dbContext)
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
        public async Task<Module> OnItemCreationAsync(Module module)
        {
            try
            {
                await _dbContext.AddAsync(module);

                return module;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add module");
            }
        }
        public async Task<Module> OnLoadItemAsync(Guid ModuleId)
        {
            try
            {
                var module = await _dbContext.Module.Where(m => m.ModuleId == ModuleId).FirstOrDefaultAsync();

                if (module is null)
                {
                    throw new Exception("Error!");
                }

                return module                                                                      ;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<Module>> OnLoadItemsAsync()
        {
            try
            {
                var module = await _dbContext.Module.ToListAsync();

                var getActiveAttachments = from n in module

                                           where n.IsActive == true

                                           select n;

                return getActiveAttachments.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: module could not be loaded!");
            }
        }
        public async Task<Module> OnModifyItemAsync(Module module)
        {
            Module results = new Module();

            try
            {
                results = await _dbContext.Module.FirstOrDefaultAsync(x => x.ModuleId == module.ModuleId);

                if (results != null)
                {
                    results.IsActive = module.IsActive;

                    results.NQFLevel = module.NQFLevel;

                    results.ModuleName = module.ModuleName;

                    results.Credit = module.Credit;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ModuleId)
        {
            Module module = new Module();

            int record = 0;

            try
            {
                module = await _dbContext.Module.FirstOrDefaultAsync(m => m.ModuleId == ModuleId);

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

