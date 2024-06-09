using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class FinanceRepository : IUnitOfWork<LearnerFinance>
    {
        private readonly ApplicationDbContext _dbContext;
        public FinanceRepository(ApplicationDbContext dbContext)
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
        public async Task<LearnerFinance> OnItemCreationAsync(LearnerFinance finance)
        {
            try
            {
                await _dbContext.AddAsync(finance);

                return finance;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }
        public async Task<LearnerFinance> OnLoadItemAsync(Guid Id)
        {
            try
            {
                var finances = await _dbContext.Finance.Where(m => m.Id == Id).FirstOrDefaultAsync();

                if (finances is null)
                {
                    throw new Exception("Error!");
                }

                return finances;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<LearnerFinance>> OnLoadItemsAsync()
        {
            try
            {
                var finance = await _dbContext.Finance.ToListAsync();

                var getActiveAttachments = from n in finance

                                           where n.IsActive == true

                                           select n;

                return getActiveAttachments.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Attachments could not be loaded!");
            }
        }
        public async Task<LearnerFinance> OnModifyItemAsync(LearnerFinance finance)
        {
            LearnerFinance results = new LearnerFinance();

            try
            {
                results = await _dbContext.Finance.FirstOrDefaultAsync(x => x.Id == finance.Id);

                if (results != null)
                {
                    results.StatementName= finance.StatementName;   

                    results.IsActive = finance.IsActive;

                    results.StudentNumber = finance.StudentNumber;

                    results.StudentId = finance.StudentId;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid Id)
        {
            LearnerFinance attachment = new LearnerFinance();

            int record = 0;

            try
            {
                attachment = await _dbContext.Finance.FirstOrDefaultAsync(m => m.Id == Id);

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
