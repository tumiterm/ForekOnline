using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class StudentAdminRepository : IUnitOfWork<StudentAttachment>
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentAdminRepository(ApplicationDbContext dbContext)
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

        public async Task<StudentAttachment> OnItemCreationAsync(StudentAttachment attachment)
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

        public async Task<StudentAttachment> OnLoadItemAsync(Guid AttachmentId)
        {
            try
            {
                var attachment = await _dbContext.StudentAttachments.Where(m => m.AttachmentId == AttachmentId).FirstOrDefaultAsync();

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

        public async Task<List<StudentAttachment>> OnLoadItemsAsync()
        {
            try
            {
                var attachments = await _dbContext.StudentAttachments.ToListAsync();

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

        public async Task<StudentAttachment> OnModifyItemAsync(StudentAttachment attachment)
        {
            StudentAttachment results = new StudentAttachment();

            try
            {
                results = await _dbContext.StudentAttachments.FirstOrDefaultAsync(x => x.AttachmentId == attachment.AttachmentId);

                if (results != null)
                {

                    results.IsActive = attachment.IsActive;

                    results.AttachmentFile = attachment.AttachmentFile;

                    results.Document = attachment.Document;

                    results.DocumentName = attachment.DocumentName;

                    results.ModifiedBy = attachment.ModifiedBy;

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

        public async Task<int> OnRemoveItemAsync(Guid AttachmentId)
        {
            StudentAttachment attachment = new StudentAttachment();

            int record = 0;

            try
            {
                attachment = await _dbContext.StudentAttachments.FirstOrDefaultAsync(m => m.AttachmentId == AttachmentId);

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
