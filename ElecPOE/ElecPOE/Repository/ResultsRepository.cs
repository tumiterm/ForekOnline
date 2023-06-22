using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class ResultsRepository : IUnitOfWork<ProgressReport>
    {
        private readonly ApplicationDbContext _dbContext;
        public ResultsRepository(ApplicationDbContext dbContext)
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
        public async Task<ProgressReport> OnItemCreationAsync(ProgressReport results)
        {
            try
            {
                await _dbContext.AddAsync(results);

                return results;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add results");
            }
        }
        public async Task<ProgressReport> OnLoadItemAsync(Guid ReportId)
        {
            try
            {
                var results = await _dbContext.Results.Where(m => m.ReportId == ReportId).FirstOrDefaultAsync();

                if (results is null)
                {
                    throw new Exception("Error!");
                }

                return results;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<ProgressReport>> OnLoadItemsAsync()
        {
            try
            {
                var results = await _dbContext.Results.ToListAsync();

                var getActiveResults = from n in results

                                           where n.IsActive == true

                                           select n;

                return getActiveResults.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: results could not be loaded!");
            }
        }
        public async Task<ProgressReport> OnModifyItemAsync(ProgressReport report)
        {
            ProgressReport results = new ProgressReport();

            try
            {
                results = await _dbContext.Results.FirstOrDefaultAsync(x => x.ReportId == report.ReportId);

                if (results != null)
                {
                    results.IsActive = report.IsActive;

                    results.ReportName = report.ReportName;

                    results.ReportFile = report.ReportFile;

                    results.ModifiedBy = report.ModifiedBy;

                    results.Course = report.Course;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ReportId)
        {
            ProgressReport attachment = new ProgressReport();

            int record = 0;

            try
            {
                attachment = await _dbContext.Results.FirstOrDefaultAsync(m => m.ReportId == ReportId);

                if (attachment != null)
                {
                    _dbContext.Remove(attachment);

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

