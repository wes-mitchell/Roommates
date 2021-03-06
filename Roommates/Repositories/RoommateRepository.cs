using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT rm.FirstName, rm.LastName FROM Roommate rm";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Roommate> roommates = new List<Roommate>();

                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("FirstName");
                            string name = reader.GetString(nameColumnPosition);
                            int lastNameColumnPosition = reader.GetOrdinal("LastName");
                            string lname = reader.GetString(lastNameColumnPosition);
                            Roommate roommate = new Roommate
                            {
                                Id = idValue,
                                FirstName = name,
                                LastName = lname
                            };
                            roommates.Add(roommate);

                        }
                        return roommates;
                    }
                }
            }
        }
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT r.Name, rm.Id, rm.FirstName, rm.RentPortion FROM Roommate rm JOIN Room r ON r.Id = rm.RoomId WHERE rm.Id = @Id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Roommate roommate = null;
                        Room room = null;

                        if (reader.Read())
                        {
                            room = new Room()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            roommate = new Roommate
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                Room = room
                            };
                        }
                        return roommate;
                    }
                }
            }
        }
    }
}
