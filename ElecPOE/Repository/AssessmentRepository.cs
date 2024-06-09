using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class AssessmentRepository : IUnitOfWork<AssessmentAttachment>
    {
        private readonly ApplicationDbContext _dbContext;
        public AssessmentRepository(ApplicationDbContext dbContext)
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
        public async Task<AssessmentAttachment> OnItemCreationAsync(AssessmentAttachment attachment)
        {
            try
            {
                await _dbContext.AddAsync(attachment);

                return attachment;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }
        public async Task<AssessmentAttachment> OnLoadItemAsync(Guid AttachmentId)
        {
            try
            {
                var attachment = await _dbContext.AssessmentAttachments.Where(m => m.AttachmentId == AttachmentId).FirstOrDefaultAsync();

                if (attachment is null)
                {
                    throw new Exception("Error!");
                }

                return attachment;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<AssessmentAttachment>> OnLoadItemsAsync()
        {
            try
            {
                var attachments = await _dbContext.AssessmentAttachments.ToListAsync();

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
        public async Task<AssessmentAttachment> OnModifyItemAsync(AssessmentAttachment attachment)
        {
            AssessmentAttachment results = new AssessmentAttachment();

            try
            {
                results = await _dbContext.AssessmentAttachments.FirstOrDefaultAsync(x => x.AttachmentId == attachment.AttachmentId);

                if (results != null)
                {

                    results.IsActive = attachment.IsActive;

                    results.AttachmentFile = attachment.AttachmentFile;

                    results.Document = attachment.Document;

                    results.ModifiedBy = attachment.ModifiedBy;

                    results.ModifiedOn = attachment.ModifiedOn;

                    results.Percentage = attachment.Percentage;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid AttachmentId)
        {
            AssessmentAttachment attachment = new AssessmentAttachment();

            int record = 0;

            try
            {
                attachment = await _dbContext.AssessmentAttachments.FirstOrDefaultAsync(m => m.AttachmentId == AttachmentId);

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
