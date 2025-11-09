using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using HMS;

namespace HMS.Repositories
{
    /// <summary>
    /// Abstract base repository providing common CRUD operations
    /// Uses Template Method Pattern - derived classes implement specific mapping logic
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected abstract string TableName { get; }
        protected abstract string PrimaryKey { get; }
        /// <summary>
        /// Maps a SqlDataReader row to an entity object
        /// </summary>
        protected abstract T MapDataReader(SqlDataReader reader);

        /// <summary>
        /// Creates an INSERT command for the entity
        /// </summary>
        protected abstract SqlCommand CreateInsertCommand(T entity, SqlConnection con);

        /// <summary>
        /// Creates an UPDATE command for the entity
        /// </summary>
        protected abstract SqlCommand CreateUpdateCommand(T entity, SqlConnection con);

        /// <summary>
        /// Gets the ID property value from the entity
        /// </summary>
        protected abstract int GetEntityId(T entity);

        /// <summary>
        /// Gets the searchable columns for the Search method
        /// </summary>
        protected abstract string[] GetSearchColumns();

        public virtual List<T> GetAll()
        {
            List<T> entities = new List<T>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = $"SELECT * FROM {TableName} WHERE IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entities.Add(MapDataReader(reader));
                    }
                }
            }
            return entities;
        }

        public virtual T GetById(int id)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = $"SELECT * FROM {TableName} WHERE {PrimaryKey} = @Id AND IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapDataReader(reader);
                    }
                }
            }
            return null;
        }

        public virtual bool Insert(T entity)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = CreateInsertCommand(entity, con);
                try
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        SetEntityId(entity, Convert.ToInt32(result));
                    else
                        cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets the entity ID after insertion (override in derived classes if needed)
        /// </summary>
        protected virtual void SetEntityId(T entity, int id)
        {
        }

        public virtual bool Update(T entity)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = CreateUpdateCommand(entity, con);
                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public virtual bool Delete(int id)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = $"UPDATE {TableName} SET IsDeleted = 1 WHERE {PrimaryKey} = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public virtual List<T> Search(string searchTerm)
        {
            List<T> entities = new List<T>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string[] searchColumns = GetSearchColumns();
                string searchConditions = string.Join(" OR ", Array.ConvertAll(searchColumns, col => $"{col} LIKE @Search"));
                string query = $"SELECT * FROM {TableName} WHERE ({searchConditions}) AND IsDeleted = 0";
                
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entities.Add(MapDataReader(reader));
                    }
                }
            }
            return entities;
        }
    }
}

