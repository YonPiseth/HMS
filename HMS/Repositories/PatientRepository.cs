using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using HMS.Models;

namespace HMS.Repositories
{
    /// <summary>
    /// Repository for Patient entity operations
    /// Implements IRepository<Patient> for polymorphism
    /// </summary>
    public class PatientRepository : BaseRepository<Patient>
    {
        protected override string TableName => "tblPatient";
        protected override string PrimaryKey => "PatientID";

        protected override Patient MapDataReader(SqlDataReader reader)
        {
            return new Patient
            {
                PatientID = reader.GetInt32("PatientID"),
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? reader.GetDateTime("DateOfBirth") : DateTime.Now,
                Gender = reader["Gender"]?.ToString() ?? string.Empty,
                BloodType = reader["BloodType"]?.ToString() ?? string.Empty,
                Phone = reader["ContactNumber"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString() ?? string.Empty,
                InsuranceNumber = reader["InsuranceNumber"]?.ToString() ?? string.Empty,
                PatientFamily = reader["PatientFamily"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? string.Empty,
                ProfilePhoto = reader["ProfilePhoto"] != DBNull.Value ? (byte[])reader["ProfilePhoto"] : null,
                UserID = reader["UserID"] != DBNull.Value ? reader.GetInt32("UserID") : (int?)null,
                IsDeleted = reader["IsDeleted"] != DBNull.Value ? reader.GetBoolean("IsDeleted") : false
            };
        }

        protected override SqlCommand CreateInsertCommand(Patient entity, SqlConnection con)
        {
            string query = @"INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, BloodType, ContactNumber, 
                          Email, Address, InsuranceNumber, PatientFamily, Status, ProfilePhoto, IsDeleted) 
                          VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @BloodType, @ContactNumber, @Email, @Address, 
                          @InsuranceNumber, @Family, @Status, @ProfilePhoto, 0);
                          SELECT SCOPE_IDENTITY();";
            
            SqlCommand cmd = new SqlCommand(query, con);
            AddPatientParameters(cmd, entity);
            return cmd;
        }

        protected override SqlCommand CreateUpdateCommand(Patient entity, SqlConnection con)
        {
            string query = @"UPDATE tblPatient SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, 
                          Gender=@Gender, BloodType=@BloodType, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, 
                          InsuranceNumber=@InsuranceNumber, PatientFamily=@Family, Status=@Status, ProfilePhoto=@ProfilePhoto 
                          WHERE PatientID=@PatientID";
            
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PatientID", entity.PatientID);
            AddPatientParameters(cmd, entity);
            return cmd;
        }

        private void AddPatientParameters(SqlCommand cmd, Patient entity)
        {
            cmd.Parameters.AddWithValue("@FirstName", entity.FirstName);
            cmd.Parameters.AddWithValue("@LastName", entity.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", entity.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", entity.Gender);
            cmd.Parameters.AddWithValue("@BloodType", entity.BloodType);
            cmd.Parameters.AddWithValue("@ContactNumber", entity.Phone);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@Address", entity.Address);
            cmd.Parameters.AddWithValue("@InsuranceNumber", entity.InsuranceNumber);
            cmd.Parameters.AddWithValue("@Family", entity.PatientFamily);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            cmd.Parameters.Add("@ProfilePhoto", SqlDbType.VarBinary).Value = (object)entity.ProfilePhoto ?? DBNull.Value;
        }

        protected override int GetEntityId(Patient entity)
        {
            return entity.PatientID;
        }

        protected override string[] GetSearchColumns()
        {
            return new string[] { "FirstName", "LastName", "ContactNumber", "Email", "Address", "PatientID" };
        }

        /// <summary>
        /// Gets a patient with room information
        /// </summary>
        public Patient GetByIdWithRoom(int patientId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT p.*, r.RoomNumber 
                               FROM tblPatient p 
                               LEFT JOIN tblRoom r ON p.PatientID = r.PatientID
                               WHERE p.PatientID = @PatientID AND p.IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var patient = MapDataReader(reader);
                        if (reader["RoomNumber"] != DBNull.Value)
                        {
                            patient.RoomNumber = reader["RoomNumber"]?.ToString();
                        }
                        return patient;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the PatientID after insertion
        /// </summary>
        protected override void SetEntityId(Patient entity, int id)
        {
            entity.PatientID = id;
        }

        /// <summary>
        /// Gets all patients with room information
        /// </summary>
        public List<Patient> GetAllWithRooms()
        {
            List<Patient> patients = new List<Patient>();
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT p.*, r.RoomNumber 
                               FROM tblPatient p 
                               LEFT JOIN tblRoom r ON p.PatientID = r.PatientID
                               WHERE p.IsDeleted = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var patient = MapDataReader(reader);
                        if (reader["RoomNumber"] != DBNull.Value)
                        {
                            patient.RoomNumber = reader["RoomNumber"]?.ToString();
                        }
                        patients.Add(patient);
                    }
                }
            }
            return patients;
        }
    }
}

