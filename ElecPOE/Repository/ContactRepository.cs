using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class ContactRepository : IUnitOfWork<ContactPerson>
    {
        private readonly ApplicationDbContext _dbContext;
        public ContactRepository(ApplicationDbContext dbContext)
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
        public async Task<ContactPerson> OnItemCreationAsync(ContactPerson contact)
        {
            try
            {
                await _dbContext.AddAsync(contact);

                return contact;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add contact");
            }
        }
        public async Task<ContactPerson> OnLoadItemAsync(Guid ContactId)
        {
            try
            {
                var attachment = await _dbContext.ContactPerson.Where(m => m.ContactId == ContactId).FirstOrDefaultAsync();

                if (attachment is null)
                {
                    throw new Exception("Error!");
                }

                return attachment;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load contact person");
            }
        }
        public async Task<List<ContactPerson>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.ContactPerson.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: contacts could not be loaded!");
            }
        }
        public async Task<ContactPerson> OnModifyItemAsync(ContactPerson contact)
        {
            ContactPerson results = new ContactPerson();

            try
            {
                results = await _dbContext.ContactPerson.FirstOrDefaultAsync(x => x.ContactId == contact.ContactId);

                if (results != null)
                {

                    results.Cellphone = contact.Cellphone;
                    results.Email = contact.Email;
                    results.Name = contact.Name;
                    results.LastName = contact.LastName;
                    results.AssociativeId = contact.AssociativeId;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Attachment Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ContactId)
        {
            ContactPerson contact = new ContactPerson();

            int record = 0;

            try
            {
                contact = await _dbContext.ContactPerson.FirstOrDefaultAsync(m => m.ContactId == ContactId);

                if (contact != null)
                {
                    _dbContext.Remove(contact);

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
