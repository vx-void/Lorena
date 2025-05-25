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
    }
}
