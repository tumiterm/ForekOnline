using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class RejectionsRepository : IUnitOfWork<ApplicationRejection>
    {
        private readonly ApplicationDbContext _dbContext;
        public RejectionsRepository(ApplicationDbContext dbContext)
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
        public async Task<ApplicationRejection> OnItemCreationAsync(ApplicationRejection rejection)
        {
            try
            {
                await _dbContext.AddAsync(rejection);

                return rejection;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add rejection reason!");
            }
        }
        public async Task<ApplicationRejection> OnLoadItemAsync(Guid Id)
        {
            try
            {
                var rej = await _dbContext.Rejections.Where(m => m.Id == Id).FirstOrDefaultAsync();

                if (rej is null)
                {
                    throw new Exception("Error!");
                }

                return rej;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load rejection(s)");
            }
        }
        public async Task<List<ApplicationRejection>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Rejections.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Rejection(s) could not be loaded!");
            }
        }
        public async Task<ApplicationRejection> OnModifyItemAsync(ApplicationRejection rejection)
        {
            ApplicationRejection rej = new();

            try
            {
                rej = await _dbContext.Rejections.FirstOrDefaultAsync(x => x.Id == rejection.Id);

                if (rej != null)
                {

                    rej.NextSteps = rejection.NextSteps;

                    rej.AdditionalComments = rejection.AdditionalComments;

                    rej.FollowUpDate = rejection.FollowUpDate;

                    rej.IsActive = rejection.IsActive;

                    rej.Reason = rejection.Reason;

                    rej.IsFinal = rejection.IsFinal;

                    rej.ModifiedBy = rejection.ModifiedBy;

                    rej.ModifiedOn = rejection.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Rejection Failed!");
            }

            return rej;
        }
        public async Task<int> OnRemoveItemAsync(Guid AddressId)
        {
            throw new NotImplementedException();
        }
    }
}
