 using System.Linq.Expressions;

namespace ElecPOE.Contract
{
    /// <summary>
    /// Interface for unit of work pattern to manage repository transactions.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IUnitOfWork<T> where T : class
    {
        /// <summary>
        /// Asynchronously loads all items of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of items of type <typeparamref name="T"/>.</returns>

        Task<List<T>> OnLoadItemsAsync();

        /// <summary>
        /// Asynchronously loads a single item of type <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the item to load.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the item of type <typeparamref name="T"/>.</returns>
        Task<T> OnLoadItemAsync(Guid Id);

        /// <summary>
        /// Asynchronously creates a new item of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="t">The item to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created item of type <typeparamref name="T"/>.</returns>
        Task<T> OnItemCreationAsync(T t);

        /// <summary>
        /// Asynchronously modifies an existing item of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="t">The item to modify.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the modified item of type <typeparamref name="T"/>.</returns>
        Task<T> OnModifyItemAsync(T t);

        /// <summary>
        /// Asynchronously removes an item of type <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the item to remove.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> OnRemoveItemAsync(Guid Id);

        /// <summary>
        /// Asynchronously saves all changes made in the context of the current unit of work.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> ItemSaveAsync();

        /// <summary>
        /// Checks if an entity of type <typeparamref name="TEntity"/> exists based on a specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">The expression to test each element for a condition.</param>
        /// <returns><c>true</c> if the entity exists; otherwise, <c>false</c>.</returns>
        bool DoesEntityExist<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class;
    }
}
