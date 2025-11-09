using System.Collections.Generic;

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

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        T GetById(int id);

        /// <summary>
        /// Inserts a new entity into the database
        /// </summary>
        bool Insert(T entity);

        /// <summary>
        /// Updates an existing entity in the database
        /// </summary>
        bool Update(T entity);

        /// <summary>
        /// Soft deletes an entity (sets IsDeleted = true)
        /// </summary>
        bool Delete(int id);

        /// <summary>
        /// Searches for entities matching the search term
        /// </summary>
        List<T> Search(string searchTerm);
    }
}

