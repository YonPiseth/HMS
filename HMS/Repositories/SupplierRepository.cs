using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using HMS.Models;

namespace HMS.Repositories
{
    /// <summary>
    /// Repository for Supplier entity operations
    /// Implements IRepository<Supplier> for polymorphism
    /// </summary>
    public class SupplierRepository : BaseRepository<Supplier>
    {
        protected override string TableName => "tblSupplier";
        protected override string PrimaryKey => "SupplierID";

        protected override Supplier MapDataReader(SqlDataReader reader)
        {
            return new Supplier
            {
                SupplierID = reader.GetInt32("SupplierID"),
                SupplierName = reader["SupplierName"]?.ToString() ?? string.Empty,
                ContactPerson = reader["ContactPerson"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Phone = reader["Phone"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString() ?? string.Empty,
                IsDeleted = reader["IsDeleted"] != DBNull.Value ? reader.GetBoolean("IsDeleted") : false
            };
        }

        protected override SqlCommand CreateInsertCommand(Supplier entity, SqlConnection con)
        {
            string query = @"INSERT INTO tblSupplier (SupplierName, ContactPerson, Email, Phone, Address, IsDeleted)
                          VALUES (@SupplierName, @ContactPerson, @Email, @Phone, @Address, 0)";
            
            SqlCommand cmd = new SqlCommand(query, con);
            AddSupplierParameters(cmd, entity);
            return cmd;
        }

        protected override SqlCommand CreateUpdateCommand(Supplier entity, SqlConnection con)
        {
            string query = @"UPDATE tblSupplier SET SupplierName=@SupplierName, ContactPerson=@ContactPerson, 
                          Email=@Email, Phone=@Phone, Address=@Address WHERE SupplierID=@SupplierID";
            
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SupplierID", entity.SupplierID);
            AddSupplierParameters(cmd, entity);
            return cmd;
        }

        private void AddSupplierParameters(SqlCommand cmd, Supplier entity)
        {
            cmd.Parameters.AddWithValue("@SupplierName", entity.SupplierName);
            cmd.Parameters.AddWithValue("@ContactPerson", entity.ContactPerson);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@Phone", entity.Phone);
            cmd.Parameters.AddWithValue("@Address", entity.Address);
        }

        protected override int GetEntityId(Supplier entity)
        {
            return entity.SupplierID;
        }

        protected override string[] GetSearchColumns()
        {
            return new string[] { "SupplierName", "ContactPerson", "Email", "Phone", "Address" };
        }

        /// <summary>
        /// Sets the SupplierID after insertion
        /// </summary>
        protected override void SetEntityId(Supplier entity, int id)
        {
            entity.SupplierID = id;
        }
    }
}

