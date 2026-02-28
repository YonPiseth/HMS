using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// Supports polymorphism - any entity type can implement this interface
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities that are not deleted
        /// </summary>
        List<T> GetAll();
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        T GetById(int id);
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Inserts a new entity into the database
        /// </summary>
        bool Insert(T entity);
        Task<bool> InsertAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the database
        /// </summary>
        bool Update(T entity);
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Soft deletes an entity (sets IsDeleted = true)
        /// </summary>
        bool Delete(int id);
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Searches for entities matching the search term
        /// </summary>
        List<T> Search(string searchTerm);
        Task<List<T>> SearchAsync(string searchTerm);
    }
}

