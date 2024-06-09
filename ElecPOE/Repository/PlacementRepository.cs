using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class PlacementRepository : IUnitOfWork<Placement>
    {
        private readonly ApplicationDbContext _dbContext;
        public PlacementRepository(ApplicationDbContext dbContext)
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
        public async Task<Placement> OnItemCreationAsync(Placement placement)
        {
            try
            {
                await _dbContext.AddAsync(placement);

                return placement;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add placement");
            }
        }
        public async Task<Placement> OnLoadItemAsync(Guid PlacementId)
        {
            try
            {
                var placement = await _dbContext.Placements.Where(m => m.PlacementId == PlacementId).FirstOrDefaultAsync();

                if (placement is null)
                {
                    throw new Exception("Error!");
                }

                return placement;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load placement");
            }
        }
        public async Task<List<Placement>> OnLoadItemsAsync()
        {
            try
            {
                var placement = await _dbContext.Placements.ToListAsync();

                var getActivePlacements = from n in placement

                                           where n.IsActive == true

                                           select n;

                return getActivePlacements.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Placement could not be loaded!");
            }
        }
        public async Task<Placement> OnModifyItemAsync(Placement placement)
        {
            Placement results = new Placement();

            try
            {
                results = await _dbContext.Placements.FirstOrDefaultAsync(x => x.PlacementId == placement.PlacementId);

                if (results != null)
                {

                    results.IsActive = placement.IsActive;

                    results.StartDate= placement.StartDate;

                    results.EndDate = placement.EndDate;

                    results.CompanyId = placement.CompanyId;

                    results.Student = placement.Student;

                    results.PlacedBy = placement.PlacedBy;

                    results.Status = placement.Status;

                    results.ModifiedBy = placement.ModifiedBy;

                    results.ModifiedOn = placement.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Placement Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid PlacementId)
        {
            Placement placement = new Placement();

            int record = 0;

            try
            {
                placement = await _dbContext.Placements.FirstOrDefaultAsync(m => m.PlacementId == PlacementId);

                if (placement != null)
                {
                    _dbContext.Remove(placement);

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
