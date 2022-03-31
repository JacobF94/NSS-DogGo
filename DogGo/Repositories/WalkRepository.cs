using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public List<Walk> GetWalksByWalker(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT w.Id, w.Date, w.Duration, w.WalkerId, w.DogId, d.Name as 'DogName', o.Name as 'OwnerName'
                    FROM Walks w
                    LEFT JOIN Dog d on d.Id = w.DogId
                    LEFT JOIN Owner o ON o.Id = d.OwnerId
                    WHERE w.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        List<Walk> walks = new List<Walk>();
                        while (reader.Read())
                        {
                            Walk walk = new Walk
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                DogId = reader.GetInt32(reader.GetOrdinal("DogID")),
                                DogName = reader.GetString(reader.GetOrdinal("DogName")),
                                OwnerName = reader.GetString(reader.GetOrdinal("OwnerName"))
                            };

                            walks.Add(walk);
                        }

                        return walks;
                    }
                }
            }

        }
    }
}
