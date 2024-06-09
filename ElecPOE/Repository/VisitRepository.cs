using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class VisitRepository : IUnitOfWork<Visit>
    {
        private readonly ApplicationDbContext _dbContext;
        public VisitRepository(ApplicationDbContext dbContext)
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
        public async Task<Visit> OnItemCreationAsync(Visit visit)
        {
            try
            {
                await _dbContext.AddAsync(visit);

                return visit;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add Visitation");
            }
        }
        public async Task<Visit> OnLoadItemAsync(Guid VisitId)
        {
            try
            {
                var visit = await _dbContext.Visits.Where(m => m.VisitId == VisitId).FirstOrDefaultAsync();

                if (visit is null)
                {
                    throw new Exception("Error!");
                }

                return visit;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load visit");
            }
        }
        public async Task<List<Visit>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Visits.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: visitation could not be loaded!");
            }
        }
        public async Task<Visit> OnModifyItemAsync(Visit visit)
        {
            Visit results = new Visit();

            try
            {
                results = await _dbContext.Visits.FirstOrDefaultAsync(x => x.VisitId == visit.VisitId);

                if (results != null)
                {

                    results.VisitPurpose = visit.VisitPurpose;

                    results.Report = visit.Report;

                    results.Date = visit.Date;

                    results.IsActive = visit.IsActive;

                    results.HasReport = visit.HasReport;

                    results.CompanyId = visit.CompanyId;

                    results.VisitBy = visit.VisitBy;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Visitation Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid VisitId)
        {
            Visit visit = new Visit();

            int record = 0;

            try
            {
                visit = await _dbContext.Visits.FirstOrDefaultAsync(m => m.VisitId == VisitId);

                if (visit != null)
                {
                    _dbContext.Remove(visit);

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

