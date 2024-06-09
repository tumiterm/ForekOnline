using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class CourseRepository : IUnitOfWork<Course>
    {
        private readonly ApplicationDbContext _dbContext;
        public CourseRepository(ApplicationDbContext dbContext)
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
        public async Task<Course> OnItemCreationAsync(Course course)
        {
            try
            {
                await _dbContext.AddAsync(course);

                return course;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add attachment");
            }
        }
        public async Task<Course> OnLoadItemAsync(Guid CourseId)
        {
            try
            {
                var course = await _dbContext.Course.Where(m => m.CourseId == CourseId).FirstOrDefaultAsync();

                if (course is null)
                {
                    throw new Exception("Error!");
                }

                return course;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load item");
            }
        }
        public async Task<List<Course>> OnLoadItemsAsync()
        {
            try
            {
                var courses = await _dbContext.Course.ToListAsync();

                var getActiveCourses = from n in courses

                                       //where n.IsActive == true

                                       select n;

                return getActiveCourses.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: courses could not be loaded!");
            }
        }
        public async Task<Course> OnModifyItemAsync(Course course)
        {
            Course results = new Course();

            try
            {
                results = await _dbContext.Course.FirstOrDefaultAsync(x => x.CourseId == course.CourseId);

                if (results != null)
                {

                    results.IsActive = course.IsActive;

                    results.CourseName = course.CourseName;

                    results.ModifiedOn = course.ModifiedOn;

                    results.ModifiedBy = course.ModifiedBy;

                    results.Type = course.Type;

                    results.NQFLevel = course.NQFLevel;

                    results.Module = course.Module;

                    results.NType = course.NType;

                    results.Credit = course.Credit;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: course Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid CourseId)
        {
            Course course = new Course();

            int record = 0;
            try
            {
                course = await _dbContext.Course.FirstOrDefaultAsync(m => m.CourseId == CourseId);

                if (course != null)
                {
                    _dbContext.Remove(course);

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
    
