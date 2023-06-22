using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class ReportRepository : IUnitOfWork<Report>
    {
        private readonly ApplicationDbContext _dbContext;
        public ReportRepository(ApplicationDbContext dbContext)
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
        public async Task<Report> OnItemCreationAsync(Report report)
        {
            try
            {
                await _dbContext.AddAsync(report);

                return report;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add report");
            }
        }
        public async Task<Report> OnLoadItemAsync(Guid ReportId)
        {
            try
            {
                var report = await _dbContext.Reports.Where(m => m.ReportId == ReportId).FirstOrDefaultAsync();

                if (report is null)
                {
                    throw new Exception("Error!");
                }

                return report;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<Report>> OnLoadItemsAsync()
        {
            try
            {
                var reports = await _dbContext.Reports.ToListAsync();

                var getActiveReports = from n in reports

                                       where n.IsActive == true

                                       select n;

                return getActiveReports.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Reports could not be loaded!");
            }
        }
        public async Task<Report> OnModifyItemAsync(Report report)
        {
            Report results = new Report();

            try
            {
                results = await _dbContext.Reports.FirstOrDefaultAsync(x => x.ReportId == report.ReportId);

                if (results != null)
                {

                    results.IsActive = results.IsActive;

                    results.ActivityReport = results.ActivityReport;

                    results.Challenges = results.Challenges;

                    results.Date = results.Date;

                    results.FacilitatorId = results.FacilitatorId;

                    results.Module = results.Module;

                    results.Urgency = results.Urgency;

                    results.Recommendation = results.Module;

                    results.ReportType = results.ReportType;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Report Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ReportId)
        {
            Report report = new Report();

            int record = 0;

            try
            {
                report = await _dbContext.Reports.FirstOrDefaultAsync(m => m.ReportId == ReportId);

                if (report != null)
                {
                    _dbContext.Remove(report);

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

