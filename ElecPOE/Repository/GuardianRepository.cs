using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class GuardianRepository : IUnitOfWork<Guardian>
    {
        private readonly ApplicationDbContext _dbContext;
        public GuardianRepository(ApplicationDbContext dbContext)
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
        public async Task<Guardian> OnItemCreationAsync(Guardian guardian)
        {
            try
            {
                await _dbContext.AddAsync(guardian);

                return guardian;
            }
            catch (Exception)
            {

                throw new Exception("Error: Guardian Error!!!");
            }
        }
        public async Task<Guardian> OnLoadItemAsync(Guid GuardianId)
        {
            try
            {
                var guardian = await _dbContext.Guardians.Where(m => m.GuardianId == GuardianId).FirstOrDefaultAsync();

                if (guardian is null)
                {
                    throw new Exception("Error!");
                }

                return guardian;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load guardian details");
            }
        }
        public async Task<List<Guardian>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Guardians.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Guardian could not be loaded!");
            }
        }
        public async Task<Guardian> OnModifyItemAsync(Guardian guardian)
        {
            Guardian results = new();

            try
            {
                results = await _dbContext.Guardians.FirstOrDefaultAsync(x => x.GuardianId == guardian.GuardianId);

                if (results != null)
                {

                    results.Cellphone = guardian.Cellphone;

                    results.Relationship = guardian.Relationship;

                    results.LastName = guardian.LastName;

                    results.FirstName = guardian.FirstName;

                    results.LastName = guardian.LastName;

                    results.IDDoc = guardian.IDDoc;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Address Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid GuardianId)
        {
            Guardian guardian = new();

            int record = 0;

            try
            {
                guardian = await _dbContext.Guardians.FirstOrDefaultAsync(m => m.GuardianId == GuardianId);

                if (guardian != null)
                {
                    _dbContext.Remove(guardian);

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
