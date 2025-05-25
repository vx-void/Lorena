using Lorena;

namespace LorenaNUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            string filename = "Lorena.db";
            DB db = new DB($"Data Source={filename}; Version=3");

        }

        [Test]
        public void Test1()
        {
            db.CreateDB();
            bool DbIsExist = File.Exists(filename);
            Assert.That(DbIsExist, Is.EqualTo(true));
        }
    }
}