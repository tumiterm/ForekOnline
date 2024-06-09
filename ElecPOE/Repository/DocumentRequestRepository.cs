using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElecPOE.Repository
{
    public class DocumentRequestRepository : IUnitOfWork<Document>
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentRequestRepository(ApplicationDbContext dbContext)
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

        public async Task<Document> OnItemCreationAsync(Document document)
        {
            try
            {
                await _dbContext.Documents.AddAsync(document);

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Failed to add document", ex);
            }
        }

        public async Task<Document> OnLoadItemAsync(Guid documentId)
        {
            try
            {
                var document = await _dbContext.Documents.FindAsync(documentId);

                if (document == null)
                {
                    throw new Exception($"Document with ID {documentId} not found.");
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Unable to load item", ex);
            }
        }

        public async Task<List<Document>> OnLoadItemsAsync()
        {
            try
            {
                var activeDocuments = await _dbContext.Documents

                    .Where(d => d.IsActive)

                    .ToListAsync();

                return activeDocuments;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Documents could not be loaded!", ex);
            }
        }

        public async Task<Document> OnModifyItemAsync(Document updatedDocument)
        {
            try
            {
                var existingDocument = await _dbContext.Documents

                    .FirstOrDefaultAsync(d => d.DocumentId == updatedDocument.DocumentId);

                if (existingDocument != null)
                {
                    existingDocument.Reference = updatedDocument.Reference;

                    existingDocument.IsReturned = updatedDocument.IsReturned;

                    existingDocument.ModuleId = updatedDocument.ModuleId;

                    existingDocument.CourseId = updatedDocument.CourseId;

                    existingDocument.DocumentType = updatedDocument.DocumentType;

                    existingDocument.RequestDate = updatedDocument.RequestDate;

                    existingDocument.ReturnDate = updatedDocument.ReturnDate;

                    existingDocument.Student = updatedDocument.Student;

                    existingDocument.RequestedBy = updatedDocument.RequestedBy;

                    existingDocument.Department = updatedDocument.Department;

                    existingDocument.Designation = updatedDocument.Designation;

                    existingDocument.Quantity = updatedDocument.Quantity;

                    existingDocument.RequestPurpose = updatedDocument.RequestPurpose;

                    existingDocument.ApprovedBy = updatedDocument.ApprovedBy;

                    existingDocument.IsEmailIssued = updatedDocument.IsEmailIssued;

                    existingDocument.IsHardCopyIssued = updatedDocument.IsHardCopyIssued;

                    existingDocument.IsActive = updatedDocument.IsActive;

                    existingDocument.ModifiedBy = updatedDocument.ModifiedBy;

                    existingDocument.DocumentUpload = updatedDocument.DocumentUpload;

                    existingDocument.ModifiedOn = updatedDocument.ModifiedOn;

                    await _dbContext.SaveChangesAsync();

                    return existingDocument;
                }

                throw new Exception($"Document with ID {updatedDocument.DocumentId} not found.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Document Failed to Modify!", ex);
            }
        }



        public async Task<int> OnRemoveItemAsync(Guid documentId)
        {
            try
            {
                var document = await _dbContext.Documents.FindAsync(documentId);

                if (document != null)
                {
                    _dbContext.Documents.Remove(document);

                    return await _dbContext.SaveChangesAsync();
                }

                throw new Exception($"Document with ID {documentId} not found.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Delete Failed", ex);
            }
        }
    }

}
