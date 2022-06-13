using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {
        // passes connection string along to Chore Repository from Base Repository
        public ChoreRepository(string connectionString) : base(connectionString) {  }

        /// Get a list of all chores
        
        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                // open the connection to the database
                conn.Open();

                // use commands as well
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // setup the command you would like to execute
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    //Execute the SQL in the db and get a "reader" that will give access to the data.
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // A list to hold the chores we recieve from db
                        List<Chore> chores = new List<Chore>();

                        // Read will return true while there is more data to read
                        while (reader.Read())
                        {
                            // the ordinal is the numeric position of the column in the query results

                            int idColumnPosition = reader.GetOrdinal("Id");

                            int idValue = reader.GetInt32(idColumnPosition);

                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            // Creat a new chore object with the data
                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue,
                            };

                            // add room object to list
                            chores.Add(chore);
                        }
                        return chores;
                    }
                }
            }
        }
        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.Name FROM Chore c LEFT JOIN RoommateChore rc ON rc.ChoreId = c.Id LEFT JOIN Roommate r ON r.Id = rc.RoommateId WHERE rc.RoommateId IS NULL";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> unassignedChores = new List<Chore>();

                        while (reader.Read())
                        {
                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            Chore newChore = new Chore
                            {
                                Name = nameValue
                            };
                            unassignedChores.Add(newChore);
                        }
                        return unassignedChores;
                    }
                }
            }
        }

        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;

                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                        return chore;
                    }
                }
            }
        }
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                        OUTPUT INSERTED.Id
                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }
        public void AssignChore(int roommateId, int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId)
                                        VALUES (@roommateId, @choreId)";
                    cmd.Parameters.AddWithValue("@roommateId", roommateId);
                    cmd.Parameters.AddWithValue("@choreId", choreId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                        SET Name = @name
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", chore.Id);
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Chore
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<ChoreCount> GetChoreCount()
        { 
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT r.FirstName, r.LastName, COUNT(rc.ChoreId) AS ChoreCount        FROM RoommateChore rc
                                        Join Roommate r ON r.Id = rc.RoommateId
                                        GROUP BY r.FirstName, r.LastName";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<ChoreCount> choreList = new List<ChoreCount>();

                        while (reader.Read())
                        {
                            int firstNameColumn = reader.GetOrdinal("FirstName");
                            string firstName = reader.GetString(firstNameColumn);
                            int lastNameColumn = reader.GetOrdinal("LastName");
                            string lastName = reader.GetString(lastNameColumn);
                            int choreCountColumn = reader.GetOrdinal("ChoreCount");
                            int choreCount = reader.GetInt32(choreCountColumn);

                            ChoreCount chore = new ChoreCount
                            {
                                Name = $"{firstName} {lastName}",
                                ChoreAmount = choreCount
                            };
                            choreList.Add(chore);
                        }
                        return choreList;
                    }
                }
            }
        
        }

    }
}


