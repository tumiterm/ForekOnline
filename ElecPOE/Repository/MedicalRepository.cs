using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class MedicalRepository : IUnitOfWork<Medical>
    {
        private readonly ApplicationDbContext _dbContext;
        public MedicalRepository(ApplicationDbContext dbContext)
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
        public async Task<Medical> OnItemCreationAsync(Medical medical)
        {
            try
            {
                await _dbContext.AddAsync(medical);

                return medical;
            }
            catch (Exception)
            {

                throw new Exception("Error: Medical Error!!!");
            }
        }
        public async Task<Medical> OnLoadItemAsync(Guid MedicalId)
        {
            try
            {
                var medical = await _dbContext.Medicals.Where(m => m.MedicalId == MedicalId).FirstOrDefaultAsync();

                if (medical is null)
                {
                    throw new Exception("Error!");
                }

                return medical;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load Medical info");
            }
        }
        public async Task<List<Medical>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Medicals.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Medical Info could not be loaded!");
            }
        }
        public async Task<Medical> OnModifyItemAsync(Medical medical)
        {
            Medical results = new();

            try
            {
                results = await _dbContext.Medicals.FirstOrDefaultAsync(x => x.MedicalId == medical.MedicalId);

                if (results != null)
                {

                    results.Telephone = medical.Telephone;

                    results.MedicalName = medical.MedicalName;

                    results.MemberNumber = medical.MemberNumber;

                    results.Telephone = medical.Telephone;

                    results.Disability = medical.Disability;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Medical Info Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid MedicalId)
        {
            Medical medical = new();

            int record = 0;

            try
            {
                medical = await _dbContext.Medicals.FirstOrDefaultAsync(m => m.MedicalId == MedicalId);

                if (medical != null)
                {
                    _dbContext.Remove(medical);

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
