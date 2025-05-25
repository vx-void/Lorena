using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace Lorena
{
    public  class DB
    {
        private readonly string _connectionString;
        public DB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void CreateDB()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string discountTable = @"CREATE TABLE IF NOT EXISTS " +
                    "Salon(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "Name TEXT UNIQUE NOT NULL, " +
                    "Discount REAL NOT NULL, " +
                    "HasDependency INTEGER NOT NULL, " +
                    "Description TEXT CHECK(LENGTH(Description) <= 124), " +
                    "ParentID INTEGER); ";

                string finalTable = @"CREATE TABLE IF NOT EXISTS " +
                     "CalculationTable(" +
                     "SalonId INTEGER UNIQUE NOT NULL," +
                     "Price DOUBLE NOT NULL, " +
                     "Discount INTEGER NOT NULL," +
                     "ParentDiscount INTEGER, " +
                     "FinalPrice REAL NOT NULL, " +
                     "FOREIGN KEY (SalonId) REFERENCES Salon(Id));";

                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SQLiteCommand(discountTable, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand(finalTable, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }


        public void Insert(Salon salon)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sqlCommandString = @"
                INSERT INTO Salon(Name, Discount, HasDependency, Description, ParentId)
                VALUES (@Name, @Discount, @HasDependency, @Description, @ParentId);";

                ExecuteNonQuery(connection, sqlCommandString, new SQLiteParameter[]
                {
                    new SQLiteParameter("@Name", salon.Name),
                    new SQLiteParameter("@Discount", salon.Discount),
                    new SQLiteParameter("@HasDependency", salon.HasDependency ? 1 : 0),
                    new SQLiteParameter("@Description", salon.Description ?? (object)DBNull.Value),
                    new SQLiteParameter("@ParentId", salon.ParentId ?? (object)DBNull.Value)
                });
            }
        }

        public Salon SelectSalonById(int Id)
        {
            Salon salon = null;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM Salon WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.CommandText = query;
                    command.Parameters.Add(new SQLiteParameter("@Id", Id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            salon = new Salon(
                                db: this,
                                name: reader["Name"].ToString(),
                                discount: Convert.ToInt32(reader["Discount"]),
                                hasDependency: Convert.ToBoolean(reader["HasDependency"]),
                                description: reader["Description"].ToString(),
                                parentId: reader["ParentId"] != DBNull.Value ? Convert.ToInt32(reader["ParentId"]) : (int?)null
                            );
                        }
                    }
                }
            }
            return salon;
        }


        private void ExecuteNonQuery(SQLiteConnection connection, string query, SQLiteParameter[] parameters = null)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

    }
}
