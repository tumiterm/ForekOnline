using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class LessonPlanRepository : IUnitOfWork<LessonPlan>
    {
        private readonly ApplicationDbContext _dbContext;
        public LessonPlanRepository(ApplicationDbContext dbContext)
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
        public async Task<LessonPlan> OnItemCreationAsync(LessonPlan plan)
        {
            try
            {
                await _dbContext.AddAsync(plan);

                return plan;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add plan");
            }
        }
        public async Task<LessonPlan> OnLoadItemAsync(Guid LessonPlanId)
        {
            try
            {
                var plan = await _dbContext.LessonPlan.Where(m => m.LessonPlanId == LessonPlanId).FirstOrDefaultAsync();

                if (plan is null)
                {
                    throw new Exception("Error!");
                }

                return plan;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load plan");
            }
        }
        public async Task<List<LessonPlan>> OnLoadItemsAsync()
        {
            try
            {
                var plan = await _dbContext.LessonPlan.ToListAsync();

                var getActivePlans = from n in plan

                                           where n.IsActive == true

                                           select n;

                return getActivePlans.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Plans could not be loaded!");
            }
        }
        public async Task<LessonPlan> OnModifyItemAsync(LessonPlan plan)
        {
            LessonPlan results = new LessonPlan();

            try
            {
                results = await _dbContext.LessonPlan.FirstOrDefaultAsync(x => x.LessonPlanId == plan.LessonPlanId);

                if (results != null)
                {

                    results.IsActive = plan.IsActive;

                    results.Phase = plan.Phase;

                    results.Course = plan.Course;

                    results.Module = plan.Module;

                    results.Funder = plan.Funder;

                    results.Approval = plan.Approval; 

                    results.IsApprovedBy = plan.IsApprovedBy;

                    results.Reason = plan.Reason;

                    results.Document= plan.Document;    

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid LessonPlanId)
        {
            LessonPlan plan = new LessonPlan();

            int record = 0;

            try
            {
                plan = await _dbContext.LessonPlan.FirstOrDefaultAsync(m => m.LessonPlanId == LessonPlanId);

                if (plan != null)
                {
                    _dbContext.Remove(plan);

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

