using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class TrainingRepository : IUnitOfWork<Training>
    {
        private readonly ApplicationDbContext _dbContext;
        public TrainingRepository(ApplicationDbContext dbContext)
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

        public async Task<Training> OnItemCreationAsync(Training training)
        {
            try
            {
                await _dbContext.AddAsync(training);

                return training;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }

        public async Task<Training> OnLoadItemAsync(Guid TrainingId)
        {
            try
            {
                var material = await _dbContext.Training.Where(m => m.TrainingId == TrainingId).FirstOrDefaultAsync();

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

        public async Task<List<Training>> OnLoadItemsAsync()
        {
            try
            {
                var materials = await _dbContext.Training.ToListAsync();

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

        public async Task<Training> OnModifyItemAsync(Training training)
        {
            Training results = new Training();

            try
            {
                results = await _dbContext.Training.FirstOrDefaultAsync(x => x.TrainingId == training.TrainingId);

                if (results != null)
                {

                    results.IsActive = training.IsActive;

                    results.Document = training.Document;

                    results.StudentId = training.StudentId;

                    results.StudentId = training.StudentId;

                    results.Type = training.Type;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }

        public async Task<int> OnRemoveItemAsync(Guid TrainingId)
        {
            Training training = new Training();

            int record = 0;

            try
            {
                training = await _dbContext.Training.FirstOrDefaultAsync(m => m.TrainingId == TrainingId);

                if (training != null)
                {
                    _dbContext.Remove(training);

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
