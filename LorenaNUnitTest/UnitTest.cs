using Lorena;
using System.Data.Common;
using System.Data.SQLite;

namespace LorenaNUnitTest
{
    [TestFixture]
    public class Tests
    {

        private DB _db;
        private string _filename = "Lorena.db";

        private List<Salon> _salonList;
        private Dictionary<string, string> _salonDictionary;
       


        [SetUp]
        public void Setup()
        {
            _db = new DB($"Data Source={_filename}; Version=3");  
        }


        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }
        }

        [Test]
        public void CreateDb()
        {
            _db.CreateDB();
            bool DbIsExist = File.Exists(_filename);
            Assert.That(DbIsExist, Is.EqualTo(true));
        }


        [Test]
        public void DefaultData_InsertsCorrectsalonCount()
        {
            _db.CreateDB();
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }
            _db.CreateDB();
            DataInitialize.DefaultData(_db);
            int count = _db.SelectCountFromSalon();
            Assert.AreEqual(5, count);
        }



        [Test]
        public void SalonConstuctorCorrectTest()
        {
            _db.CreateDB();
            string name = "Test Salon";
            int discount = 10;
            bool hasDependency = true;
            string description = "Test description";
            int? parentId = 1;

            var salon = new Salon(_db, name, discount, hasDependency, description, parentId);

            Assert.AreEqual(name, salon.Name);
            Assert.AreEqual(discount, salon.Discount);
            Assert.AreEqual(hasDependency, salon.HasDependency);
            Assert.AreEqual(description, salon.Description);
            Assert.AreEqual(parentId, salon.ParentId);
        }

        [Test]
        public void SalonNullDescription()
        {
            _db.CreateDB();
            var salon = new Salon(_db, "Test", 5, false, null, null);
            Assert.IsNull(salon.Description);
        }


        [Test]
        public void CalculateTable_FinalPrice()
        {
            _db.CreateDB();
            int salonId = 1;
            double price = 100.0;
            int discount = 10;
            int parentDiscount = 5;
            //double? finalPrice = 85.0;

            var ct = new CalculateTable(salonId, price, discount, parentDiscount, null);

            Assert.AreEqual(85.0, ct.FinalPrice);
        }

        [Test]
        public void CalculateTable_PropertiesCorrectly()
        {
            _db.CreateDB();
            int salonId = 1;
            double price = 100.0;
            int discount = 10;
            int parentDiscount = 5;
            double? finalPrice = 80.0;

            var ct = new CalculateTable(salonId, price, discount, parentDiscount, finalPrice);

            Assert.AreEqual(salonId, ct.SalonId);
            Assert.AreEqual(price, ct.Price);
            Assert.AreEqual(discount, ct.Discount);
            Assert.AreEqual(parentDiscount, ct.ParentDiscount);
            Assert.AreEqual(finalPrice, ct.FinalPrice);
        }


        [Test]
        public void GetCalculateTableList_ReturnsCorrectNumberOfItems()
        {
            _db.CreateDB();
            int count = _db.SelectCountFromSalon();

            var result = DataInitialize.GetCalculateTableList(_db, count);

            Assert.AreEqual(count, result.Count);
        }

        [Test]
        public void GetCalculateTableList_ReturnsEmptyListForZeroIterations()
        {
            _db.CreateDB();
            var result = DataInitialize.GetCalculateTableList(_db, 0);

            Assert.IsEmpty(result);
        }

        [Test]
        public void DefaultData_SetsCorrectParentRelationships()
        {
            _db.CreateDB();
            DataInitialize.DefaultData(_db);

            var amelia = _db.SelectSalonById(2);
            var test1 = _db.SelectSalonById(3);
            var test2 = _db.SelectSalonById(4);

            Assert.IsNotNull(amelia.ParentId);
            Assert.IsNotNull(test1.ParentId);
            Assert.IsNotNull(test2.ParentId);
        }


        [Test]
        public void SelectSalonById_ReturnsNullForNonexistentId()
        {
            _db.CreateDB();
            var result = _db.SelectSalonById(999);

            Assert.IsNull(result);
        }

        [Test]
        public void UpdateParentId_SetsCorrectParentId()
        {
            _db.CreateDB();
            var parent = new Salon(_db, "Parent", 10, false, null, null);
            var child = new Salon(_db, "Child", 5, true, null, null);
            _db.Insert(parent);
            _db.Insert(child);

            int count = _db.SelectCountFromSalon();

            _db.UpdateParentId("Child", "Parent");


            var updatedChild = _db.SelectSalonById(count);
            Assert.AreEqual(1, updatedChild.ParentId);
        }

        [Test]
        public void SelectCountFromSalon_ReturnsZeroForEmptyDatabase()
        {
            _db.CreateDB();
            int count = _db.SelectCountFromSalon();

            Assert.AreEqual(0, count);
        }

        [Test]
        public void SelectCountFromSalon_ReturnsCorrectCount()
        {
            _db.CreateDB();
            _db.Insert(new Salon(_db, "Test1", 10, false, null, null));
            _db.Insert(new Salon(_db, "Test2", 20, false, null, null));

            // Act
            int count = _db.SelectCountFromSalon();

            // Assert
            Assert.AreEqual(2, count);
        }
    }

}
