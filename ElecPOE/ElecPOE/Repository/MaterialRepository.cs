using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class MaterialRepository : IUnitOfWork<Material>
    {
        private readonly ApplicationDbContext _dbContext;
        public MaterialRepository(ApplicationDbContext dbContext)
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

        public async Task<Material> OnItemCreationAsync(Material material)
        {
            try
            {
                await _dbContext.AddAsync(material);

                return material;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }

        public async Task<Material> OnLoadItemAsync(Guid MaterialId)
        {
            try
            {
                var material = await _dbContext.Material.Where(m => m.MaterialId == MaterialId).FirstOrDefaultAsync();

                if (material is null)
                {
                    throw new Exception("Error!");
                }

                return material;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }

        public async Task<List<Material>> OnLoadItemsAsync()
        {
            try
            {
                var materials = await _dbContext.Material.ToListAsync();

                var getActiveAttachments = from n in materials

                                           where n.IsActive == true

                                           select n;

                return getActiveAttachments.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Attachments could not be loaded!");
            }
        }

        public async Task<Material> OnModifyItemAsync(Material attachment)
        {
            Material results = new Material();

            try
            {
                results = await _dbContext.Material.FirstOrDefaultAsync(x => x.MaterialId == attachment.MaterialId);

                if (results != null)
                {

                    results.IsActive = attachment.IsActive;

                    results.Document = attachment.Document;

                    results.DueDate = attachment.DueDate;

                    results.ModifiedBy = attachment.ModifiedBy;

                    results.ModifiedOn = attachment.ModifiedOn;

                    results.Trade = attachment.Trade;

                    results.Message = attachment.Message;

                    results.Module = attachment.Module;

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

