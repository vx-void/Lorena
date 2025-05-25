using Lorena;

namespace LorenaNUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            

        }

        [Test]
        public void CreateDbTest()
        {
            string filename = "Lorena.db";
            DB db = new DB($"Data Source={filename}; Version=3");
            db.CreateDB();
            bool DbIsExist = File.Exists(filename);
            Assert.That(DbIsExist, Is.EqualTo(true));
        }


    }
}