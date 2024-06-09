using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class CompanyRepository : IUnitOfWork<Company>
    {
        private readonly ApplicationDbContext _dbContext;
        public CompanyRepository(ApplicationDbContext dbContext)
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
        public async Task<Company> OnItemCreationAsync(Company company)
        {
            try
            {
                await _dbContext.AddAsync(company);

                return company;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add company");
            }
        }
        public async Task<Company> OnLoadItemAsync(Guid CompanyId)
        {
            try
            {
                var company = await _dbContext.Company.Where(m => m.CompanyId == CompanyId).FirstOrDefaultAsync();

                if (company is null)
                {
                    throw new Exception("Error!");
                }

                return company;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load company");
            }
        }
        public async Task<List<Company>> OnLoadItemsAsync()
        {
            try
            {
                var company = await _dbContext.Company.ToListAsync();

                var getActiveCompany = from n in company

                                           where n.IsActive == true

                                           select n;

                return getActiveCompany.ToList();

            }
            catch (Exception)
            {

                throw new Exception("Error: Company could not be loaded!");
            }
        }
        public async Task<Company> OnModifyItemAsync(Company company)
        {
            Company companies = new Company();

            try
            {
                companies = await _dbContext.Company.FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId);

                if (companies != null)
                {

                    companies.IsActive = company.IsActive;

                    companies.Address = company.Address;

                    companies.Phone = company.Phone;

                    companies.CompanyName = company.CompanyName;

                    companies.Speciality = company.Speciality;

                    companies.Contact = company.Contact;

                    companies.ModifiedBy = company.ModifiedBy;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Company Failed to Modify!");
            }

            return companies;
        }
        public async Task<int> OnRemoveItemAsync(Guid CompanyId)
        {
            Company company = new Company();

            int record = 0;

            try
            {
                company = await _dbContext.Company.FirstOrDefaultAsync(m => m.CompanyId == CompanyId);

                if (company != null)
                {
                    _dbContext.Remove(company);

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
