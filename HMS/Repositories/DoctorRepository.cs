using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using HMS.Models;

namespace HMS.Repositories
{
    /// <summary>
    /// Repository for Doctor entity operations
    /// Implements IRepository<Doctor> for polymorphism
    /// </summary>
    public class DoctorRepository : BaseRepository<Doctor>
    {
        protected override string TableName => "tblDoctor";
        protected override string PrimaryKey => "DoctorID";

        protected override Doctor MapDataReader(SqlDataReader reader)
        {
            return new Doctor
            {
                DoctorID = reader.GetInt32("DoctorID"),
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                SpecializationID = reader["SpecializationID"] != DBNull.Value ? reader.GetInt32("SpecializationID") : 0,
                Phone = reader["ContactNumber"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString() ?? string.Empty,
                YearsOfExperience = reader["YearsOfExperience"] != DBNull.Value ? reader.GetInt32("YearsOfExperience") : 0,
                Qualification = reader["Qualification"]?.ToString() ?? string.Empty,
                Department = reader["Department"]?.ToString() ?? string.Empty,
                WorkingHours = reader["WorkingHours"]?.ToString() ?? string.Empty,
                IsAvailable = reader["IsAvailable"] != DBNull.Value ? reader.GetBoolean("IsAvailable") : true,
                ProfilePhoto = reader["ProfilePhoto"] != DBNull.Value ? (byte[])reader["ProfilePhoto"] : null,
                IsDeleted = reader["IsDeleted"] != DBNull.Value ? reader.GetBoolean("IsDeleted") : false
            };
        }

        protected override SqlCommand CreateInsertCommand(Doctor entity, SqlConnection con)
        {
            string query = @"INSERT INTO tblDoctor (FirstName, LastName, SpecializationID, ContactNumber, Email, Address, 
                          YearsOfExperience, Qualification, Department, WorkingHours, IsAvailable, ProfilePhoto, IsDeleted)
                          VALUES (@FirstName, @LastName, @SpecializationID, @ContactNumber, @Email, @Address, 
                          @YearsOfExperience, @Qualification, @Department, @WorkingHours, @IsAvailable, @ProfilePhoto, 0)";
            
            SqlCommand cmd = new SqlCommand(query, con);
            AddDoctorParameters(cmd, entity);
            return cmd;
        }

        protected override SqlCommand CreateUpdateCommand(Doctor entity, SqlConnection con)
        {
            string query = @"UPDATE tblDoctor SET FirstName=@FirstName, LastName=@LastName, SpecializationID=@SpecializationID, 
                          ContactNumber=@ContactNumber, Email=@Email, Address=@Address, YearsOfExperience=@YearsOfExperience, 
                          Qualification=@Qualification, Department=@Department, WorkingHours=@WorkingHours, 
                          IsAvailable=@IsAvailable, ProfilePhoto=@ProfilePhoto WHERE DoctorID=@DoctorID";
            
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@DoctorID", entity.DoctorID);
            AddDoctorParameters(cmd, entity);
            return cmd;
        }

        private void AddDoctorParameters(SqlCommand cmd, Doctor entity)
        {
            cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
            cmd.Parameters.AddWithValue("@LastName", entity.LastName);
            cmd.Parameters.AddWithValue("@SpecializationID", entity.SpecializationID);
            cmd.Parameters.AddWithValue("@ContactNumber", entity.Phone);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@Address", entity.Address);
            cmd.Parameters.AddWithValue("@YearsOfExperience", entity.YearsOfExperience);
            cmd.Parameters.AddWithValue("@Qualification", entity.Qualification ?? string.Empty);
            cmd.Parameters.AddWithValue("@Department", entity.Department ?? string.Empty);
            cmd.Parameters.AddWithValue("@WorkingHours", entity.WorkingHours ?? string.Empty);
            cmd.Parameters.AddWithValue("@IsAvailable", entity.IsAvailable);
            cmd.Parameters.Add("@ProfilePhoto", SqlDbType.VarBinary).Value = (object)entity.ProfilePhoto ?? DBNull.Value;
        }

        protected override int GetEntityId(Doctor entity)
        {
            return entity.DoctorID;
        }

        protected override string[] GetSearchColumns()
        {
            return new string[] { "FirstName", "LastName", "ContactNumber", "Email", "Qualification", "Department" };
        }

        /// <summary>
        /// Gets all doctors with their specialization names
        /// </summary>
        public List<Doctor> GetAllWithSpecialization()
        {
            List<Doctor> doctors = new List<Doctor>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT d.*, t.SpecializationName 
                               FROM tblDoctor d
                               LEFT JOIN tblDoctorType t ON d.SpecializationID = t.SpecializationID
                               WHERE d.IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doctor = MapDataReader(reader);
                        if (reader["SpecializationName"] != DBNull.Value)
                        {
                            doctor.SpecializationName = reader["SpecializationName"]?.ToString() ?? string.Empty;
                        }
                        doctors.Add(doctor);
                    }
                }
            }
            return doctors;
        }

        /// <summary>
        /// Searches doctors with specialization names
        /// </summary>
        public List<Doctor> SearchWithSpecialization(string searchTerm)
        {
            List<Doctor> doctors = new List<Doctor>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllWithSpecialization();
            }

            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT d.*, t.SpecializationName 
                               FROM tblDoctor d
                               LEFT JOIN tblDoctorType t ON d.SpecializationID = t.SpecializationID
                               WHERE d.IsDeleted = 0 AND (
                                   d.FirstName LIKE @Search OR
                                   d.LastName LIKE @Search OR
                                   d.ContactNumber LIKE @Search OR
                                   d.Email LIKE @Search OR
                                   d.Qualification LIKE @Search OR
                                   d.Department LIKE @Search OR
                                   t.SpecializationName LIKE @Search)";
                
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doctor = MapDataReader(reader);
                        if (reader["SpecializationName"] != DBNull.Value)
                        {
                            doctor.SpecializationName = reader["SpecializationName"]?.ToString() ?? string.Empty;
                        }
                        doctors.Add(doctor);
                    }
                }
            }
            return doctors;
        }
    }
}

