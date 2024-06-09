using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class NatedRepository : IUnitOfWork<NatedEngineering>
    {
        private readonly ApplicationDbContext _dbContext;
        public NatedRepository(ApplicationDbContext dbContext)
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
        public async Task<NatedEngineering> OnItemCreationAsync(NatedEngineering nated)
        {
            try
            {
                await _dbContext.AddAsync(nated);

                return nated;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add nated");
            }
        }
        public async Task<NatedEngineering> OnLoadItemAsync(Guid Id)
        {
            try
            {
                var nated = await _dbContext.NatedEngineering.Where(m => m.NatedEngineeringId == Id).FirstOrDefaultAsync();

                if (nated is null)
                {
                    throw new Exception("Error!");
                }

                return nated;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<NatedEngineering>> OnLoadItemsAsync()
        {
            try
            {
                var nated = await _dbContext.NatedEngineering.ToListAsync();

                var getActiveRecords = from n in nated

                                           where n.IsActive == true

                                           select n;

                return getActiveRecords.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: records could not be loaded!");
            }
        }
        public async Task<NatedEngineering> OnModifyItemAsync(NatedEngineering nated)
        {
            NatedEngineering results = new NatedEngineering();

            try
            {
                results = await _dbContext.NatedEngineering.FirstOrDefaultAsync(x => x.NatedEngineeringId == nated.NatedEngineeringId);

                if (results != null)
                {

                    results.IsActive = nated.IsActive;

                    results.CourseId = nated.CourseId;

                    results.ModuleId = nated.ModuleId;

                    results.FileSelection = nated.FileSelection;

                    results.FileName = nated.FileName;

                    results.AssessmentFile = nated.AssessmentFile;

                    results.FileSelection = nated.FileSelection;

                    results.Icass1 = nated.Icass1;

                    results.Icass2 = nated.Icass2;

                    results.ModifiedBy = nated.ModifiedBy;

                    results.ModifiedOn = nated.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: record Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid Id)
        {
            NatedEngineering nated = new NatedEngineering();

            int record = 0;

            try
            {
                nated = await _dbContext.NatedEngineering.FirstOrDefaultAsync(m => m.NatedEngineeringId == Id);

                if (nated != null)
                {
                    _dbContext.Remove(nated);

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
