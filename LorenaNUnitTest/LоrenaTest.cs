using Lorena;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LorenaNUnitTest
{
    [TestFixture]
    public class LоrenaTest
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

        [Test]
        public void LorenaTest()
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }
            _db.CreateDB();
            DataInitialize.DefaultData(_db);

            double[] pricesArray = new double[]
            {
                57470.00,
                5360.00,
                136540.00,
                54054.00,
                57850.00
            };

            double[] expectedFinalPrices = new double[]
            {
                55171.20,    // Миасс: 57470 - (57470 * 0.04)
                4877.60,      // Амелия: 5360 - (5360 * (0.05 + 0.04))
                121520.60,    // Тест1: 136540 - (136540 * (0.02 + 0.05 + 0.04))
                51891.84,     // Тест2: 54054 - (54054 * (0.00 + 0.04))
                51486.50      // Курган: 57850 - (57850 * 0.11)
            };

            int count = _db.SelectCountFromSalon();
            for (int i = 1; i <= count; i++)
            {
                var salon = _db.SelectSalonById(i);
                int parentDiscount = 0;

                if (salon.HasDependency && salon.ParentId != null)
                {
                    var parent = _db.SelectSalonById(salon.ParentId.Value);
                    while (parent != null)
                    {
                        parentDiscount += parent.Discount;
                        parent = parent.ParentId != null ? _db.SelectSalonById(parent.ParentId.Value) : null;
                    }
                }
                var ct = new CalculateTable(
                    salonId: i,
                    price: pricesArray[i - 1],
                    discount: salon.Discount,
                    parentDiscount: parentDiscount,
                    finalPrice: null
                );
                _db.InsertCalculateTable(ct.SalonId, ct.Price, ct.Discount, ct.ParentDiscount, ct.FinalPrice);
            }

            List<CalculateTable> ctList = DataInitialize.GetCalculateTableList(_db, count);
            for (int i = 0; i < ctList.Count; i++)
            {
                Assert.That(ctList[i].FinalPrice, Is.EqualTo(expectedFinalPrices[i]).Within(0.01),
                    $"Ошибка в расчетах для салона {_db.SelectSalonById(i + 1).Name}");
            }
        }
    }
}

