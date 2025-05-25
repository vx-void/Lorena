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

        public void UpdateParentId(string name, string parentName)
        {
            if (parentName != null)
            {
                string query = "UPDATE Salon " +
                  "SET ParentID = (Select Id From Salon WHERE Name = @parentName) " +
                  "WHERE Name = @name";
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@parentName", parentName);
                        command.Parameters.AddWithValue("@name", name);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void InsertCalculateTable(int salonId, double price, int discount, int parentalDiscount, double finalPrice)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                string query = @"INSERT INTO CalculationTable(SalonId, Price, Discount, ParentDiscount, FinalPrice) 
                VALUES(@salonId, @price, @discount, @parentalDiscount, @finalPrice)
                ON CONFLICT(SalonId) DO UPDATE SET 
                    Price = excluded.Price,
                    Discount = excluded.Discount,
                    ParentDiscount = excluded.ParentDiscount,
                    FinalPrice = excluded.FinalPrice";
                connection.Open();
                ExecuteNonQuery(connection, query, new SQLiteParameter[]
                    {
                        new SQLiteParameter("@salonId", salonId),
                        new SQLiteParameter("@price", price),
                        new SQLiteParameter("@discount", discount),
                        new SQLiteParameter("@parentalDiscount", parentalDiscount),
                        new SQLiteParameter("@finalPrice", finalPrice)
                    }
                    );
            }

        }

        public CalculateTable GetCalculateTable(int salonId)
        {
            CalculateTable ct = null;
            using (var connection = new SQLiteConnection(_connectionString))
            {

                connection.Open();
                string query = @"SELECT * FROM CalculationTable WHERE SalonId = @salonId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.CommandText = query;
                    command.Parameters.Add(new SQLiteParameter("@salonId", salonId));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ct = new CalculateTable(
                                salonId: Convert.ToInt32(reader["SalonId"]),
                                price: (double)reader["Price"],
                                discount: Convert.ToInt32(reader["Discount"]),
                                parentDiscount: Convert.ToInt32(reader["ParentDiscount"]),
                                finalPrice: (double)reader["FinalPrice"]
                            );
                        }
                    }
                }
            }
            return ct;
        }

        public int SelectCountFromSalon()
        {
            int count = 0;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Salon";
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return count;
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
