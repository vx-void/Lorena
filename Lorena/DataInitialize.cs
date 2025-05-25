using Lorena;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorena
{
    public static class DataInitialize
    {
        public static void DefaultData(DB db)
        {
            List<Salon> SalonList = new List<Salon>
            {
                new Salon(db, "Миасс", 4, false, "Описание Миасс", null),
                new Salon(db, "Амелия", 5, true, "Описание Амелия", null),
                new Salon(db, "Тест1", 2, true, "Описание Тест1", null),
                new Salon(db, "Тест2", 0, true, "Описание Тест2", null),
                new Salon(db, "Курган", 11, false, "Описание Курган", null)
            };

            Dictionary<string, string> salonDictionary = new Dictionary<string, string>
            {
                { "Миасс" , null },
                { "Амелия" , "Миасс" },
                { "Тест1", "Амелия" },
                { "Тест2", "Миасс"  },
                { "Курган", null }
            };

            foreach (var salon in SalonList)
            {
                db.Insert(salon);
            }

            foreach (var kv in salonDictionary)
            {
                db.UpdateParentId(kv.Key, kv.Value);
            }

        }


    }
}
