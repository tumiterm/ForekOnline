using ElecPOE.Contract;
using ElecPOE.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ElecPOE.Models;

namespace ElecPOE.Repository
{
    public class EvidenceRepository : IUnitOfWork<Evidence>
    {
        private readonly ApplicationDbContext _dbContext;
        public EvidenceRepository(ApplicationDbContext dbContext)
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

        public async Task<Evidence> OnItemCreationAsync(Evidence evidence)
        {
            try
            {
                await _dbContext.AddAsync(evidence);

                return evidence;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }

        public async Task<Evidence> OnLoadItemAsync(Guid EvidenceId)
        {
            try
            {
                var evidences = await _dbContext.Evidence.Where(m => m.EvidenceId == EvidenceId).FirstOrDefaultAsync();

                if (evidences is null)
                {
                    throw new Exception("Error!");
                }

                return evidences;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }

        public async Task<List<Evidence>> OnLoadItemsAsync()
        {
            try
            {
                var attachments = await _dbContext.Evidence.ToListAsync();

                var getActiveAttachments = from n in attachments

                                           where n.IsActive == true

                                           select n;

                return getActiveAttachments.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Attachments could not be loaded!");
            }
        }

        public async Task<Evidence> OnModifyItemAsync(Evidence attachment)
        {
            Evidence results = new Evidence();

            try
            {
                results = await _dbContext.Evidence.FirstOrDefaultAsync(x => x.EvidenceId == attachment.EvidenceId);

                if (results != null)
                {

                    results.IsActive = attachment.IsActive;

                    results.ModifiedBy = attachment.ModifiedBy;

                    results.ModifiedOn = attachment.ModifiedOn;

                    results.Module = attachment.Module;

                    results.ModifiedOn = attachment.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }

        public async Task<int> OnRemoveItemAsync(Guid EvidenceId)
        {
            Evidence attachment = new Evidence();

            int record = 0;

            try
            {
                attachment = await _dbContext.Evidence.FirstOrDefaultAsync(m => m.EvidenceId == EvidenceId);

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

