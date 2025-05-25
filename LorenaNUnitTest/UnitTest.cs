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

            _salonDictionary = new Dictionary<string, string>
            {
                {"�����" , null },
                {"������" , "�����" },
                {"����1", "������" },
                {"����2", "�����"  },
                {"������", null }
            };

            _salonList = new List<Salon>
            {
                //public Salon(DB db, string name, int discount, bool hasDependency, string description, int? parentId)
                new Salon(_db, "�����", 4, false, "", null),
                new Salon(_db, "������", 5, true, "", 1),
                new Salon(_db, "����1", 2, true, "", 2),
                new Salon(_db, "����2", 0, true, "", 1 ),
                new Salon(_db, "������", 11, false, "", null)
            };

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
             
            foreach(var s in _salonList)
            {
                _db.Insert(s);
            }
            
            Salon salon = _db.SelectSalonById(id);
            Assert.That("�����", Is.EqualTo(salon.Name));
            Assert.That("������", Is.EqualTo(salon.Name));
            Assert.That("����1", Is.EqualTo(salon.Name));
            Assert.That("����2", Is.EqualTo(salon.Name));
            Assert.That("������", Is.EqualTo(salon.Name));
            
        }
    }
}