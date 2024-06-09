using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class LicenseRepository : IUnitOfWork<License>
    {
        private readonly ApplicationDbContext _dbContext;
        public LicenseRepository(ApplicationDbContext dbContext)
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
        public async Task<License> OnItemCreationAsync(License license)
        {
            try
            {
                await _dbContext.AddAsync(license);

                return license;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add license");
            }
        }
        public async Task<License> OnLoadItemAsync(Guid LicenseId)
        {
            try
            {
                var license = await _dbContext.Licenses.Where(m => m.LicenseId == LicenseId).FirstOrDefaultAsync();

                if (license is null)
                {
                    throw new Exception("Error!");
                }

                return license;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load license");
            }
        }
        public async Task<List<License>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Licenses.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: License could not be loaded!");
            }
        }
        public async Task<License> OnModifyItemAsync(License license)
        {
            License results = new();

            try
            {
                results = await _dbContext.Licenses.FirstOrDefaultAsync(x => x.LicenseId == license.LicenseId);

                if (results != null)
                {
                    results.Title = license.Title;

                    results.Name = license.Name;

                    results.LastName = license.LastName;

                    results.IDNumber = license.IDNumber;

                    results.DateOfExpiry = license.DateOfExpiry;

                    results.DateOfIssue = license.DateOfIssue;

                    results.ClientType = license.ClientType;

                    results.IsActive = license.IsActive;

                    results.ModifiedBy = license.ModifiedBy;

                    results.CourseKey = license.CourseKey;

                    results.FileUpload = license.FileUpload;

                    results.Frequency = license.Frequency;


                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Address Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid LicenseId)
        {
            License license = new();

            int record = 0;

            try
            {
                license = await _dbContext.Licenses.FirstOrDefaultAsync(m => m.LicenseId == LicenseId);

                if (license != null)
                {
                    _dbContext.Remove(license);

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
