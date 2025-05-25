using Lorena;
using System.Data.Common;
using System.Data.SQLite;

namespace LorenaNUnitTest
{
    [TestFixture]
    public class Tests
    {

        private DB _db;
        string filename = "Lorena.db";

        private List<Salon> _salonList;
        private Dictionary<string, string> _salonDictionary;
       

        [SetUp]
        public void Setup()
        {
            _db = new DB($"Data Source={filename}; Version=3");
            
        }

        [Test]
        public void CreateDbTest()
        {
           
            _db.CreateDB();
            bool DbIsExist = File.Exists(filename);
            Assert.That(DbIsExist, Is.EqualTo(true));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void InsertSalonToDatabase(int id)
        {
            DataInitialize.DefaultData(_db);
            Salon salon = _db.SelectSalonById(id);
            Assert.That("Миасс", Is.EqualTo(salon.Name));
            Assert.That("Амелия", Is.EqualTo(salon.Name));
            Assert.That("Тест1", Is.EqualTo(salon.Name));
            Assert.That("Тест2", Is.EqualTo(salon.Name));
            Assert.That("Курган", Is.EqualTo(salon.Name));
            
        }
    }
}