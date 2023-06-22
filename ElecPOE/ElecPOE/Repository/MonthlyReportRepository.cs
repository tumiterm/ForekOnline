using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class MonthlyReportRepository : IUnitOfWork<User>
    {
        private readonly ApplicationDbContext _dbContext;
        public MonthlyReportRepository(ApplicationDbContext dbContext)
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

        public async Task<User> OnItemCreationAsync(User monthlyReport)
        {
            try
            {
                await _dbContext.AddAsync(monthlyReport);

                return monthlyReport;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add user");
            }
        }

        public async Task<User> OnLoadItemAsync(Guid MonthlyReportId)
        {
            try
            {
                var monthly = await _dbContext.monthlyReports.Where(m => m.Id == MonthlyReportId).FirstOrDefaultAsync();

                if (monthly is null)
                {
                    throw new Exception("Error!");
                }

                return monthly;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load report");
            }
        }

        public async Task<List<User>> OnLoadItemsAsync()
        {
            try
            {
                var months = await _dbContext.monthlyReports.ToListAsync();

                var getActiveMonths = from n in months

                                      select n;

                return getActiveMonths.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: File could not be loaded!");
            }
        }

        public async Task<User> OnModifyItemAsync(User monthlyReport)
        {
            User results = new User();

            try
            {
                results = await _dbContext.monthlyReports.FirstOrDefaultAsync(x => x.Id == monthlyReport.Id);

                if (results != null)
                {



                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }

        public async Task<int> OnRemoveItemAsync(Guid MonthlyReportId)
        {
            User user = new User();

            int record = 0;

            try
            {
                user = await _dbContext.monthlyReports.FirstOrDefaultAsync(m => m.Id == MonthlyReportId);

                if (user != null)
                {
                    _dbContext.Remove(user);

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
