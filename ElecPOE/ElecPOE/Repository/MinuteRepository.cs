using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ElecPOE.Repository
{
    public class MinuteRepository : IUnitOfWork<Minute>
    {
        private readonly ApplicationDbContext _dbContext;
        public MinuteRepository(ApplicationDbContext dbContext)
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

        public async Task<Minute> OnItemCreationAsync(Minute minute)
        {
            try
            {
                await _dbContext.AddAsync(minute);

                return minute;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add Minutes");
            }
        }

        public async Task<Minute> OnLoadItemAsync(Guid MinuteId)
        {
            try
            {
                var minutes = await _dbContext.Minutes.Where(m => m.MinuteId == MinuteId).FirstOrDefaultAsync();

                if (minutes is null)
                {
                    throw new Exception("Error!");
                }

                return minutes;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load Minutes");
            }
        }

        public async Task<List<Minute>> OnLoadItemsAsync()
        {
            try
            {
                var minutes = await _dbContext.Minutes.ToListAsync();

                var getMinutes = from n in minutes

                                 select n;

                return getMinutes.ToList();
            }
            catch
            {
                throw new Exception("Minutes couldn't be loaded");
            }
        }

        public async Task<Minute> OnModifyItemAsync(Minute minute)
        {
            Minute minutes = new Minute();
            try
            {
                minutes = await _dbContext.Minutes.Where( x=>x.MinuteId == minute.MinuteId ).FirstOrDefaultAsync();

                if(minutes != null)
                {
                    minutes.MinuteName = minute.MinuteName;

                    minutes.Venue = minute.Venue;

                    minutes.Date = minute.Date;

                    minutes.RequestedBy = minute.RequestedBy;

                    minutes.ModifiedBy = minute.ModifiedBy;

                    minutes.ModifiedOn = minute.ModifiedOn;

                    await _dbContext.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                throw new Exception("Error: Minutes Failed to Modify!");
            }
            return minutes;
        }

        public async Task<int> OnRemoveItemAsync(Guid MinuteId)
        {
            Minute Minute = new Minute();

            int record = 0;

            try
            {
                Minute = await _dbContext.Minutes.FirstOrDefaultAsync(m => m.MinuteId == MinuteId);

                if (Minute != null)
                {
                    _dbContext.Remove(Minute);

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
