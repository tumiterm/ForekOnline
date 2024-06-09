using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class FileRepository : IUnitOfWork<Models.File>
    {
        private readonly ApplicationDbContext _dbContext;
        public FileRepository(ApplicationDbContext dbContext)
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
        public async Task<Models.File> OnItemCreationAsync(Models.File file)
        {
            try
            {
                await _dbContext.AddAsync(file);

                return file;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add file");
            }
        }
        public async Task<Models.File> OnLoadItemAsync(Guid FileId)
        {
            try
            {
                var file = await _dbContext.Files.Where(m => m.FileId == FileId).FirstOrDefaultAsync();

                if (file is null)
                {
                    throw new Exception("Error!");
                }

                return file;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load file");
            }
        }
        public async Task<List<Models.File>> OnLoadItemsAsync()
        {
            try
            {
                var files = await _dbContext.Files.ToListAsync();

                var getActiveFiles = from n in files

                                           where n.IsActive == true

                                           select n;

                return getActiveFiles.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Files could not be loaded!");
            }
        }
        public async Task<Models.File> OnModifyItemAsync(Models.File files)
        {
            Models.File results = new();

            try
            {
                results = await _dbContext.Files.FirstOrDefaultAsync(x => x.FileId == files.FileId);

                if (results != null)
                {

                    results.IsActive = files.IsActive;

                    results.Phase = files.Phase;

                    results.StartDate = files.StartDate;
                    
                    results.Attachment = files.Attachment;

                    results.EndDate = files.EndDate;

                    results.ModifiedOn = files.ModifiedOn;

                    results.Type = files.Type;

                    results.ModifiedOn = files.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid FileId)
        {
            Models.File file = new();

            int record = 0;

            try
            {
                file = await _dbContext.Files.FirstOrDefaultAsync(m => m.FileId == FileId);

                if (file != null)
                {
                    _dbContext.Remove(file);

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
