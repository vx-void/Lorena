namespace Lorena
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dbFile = "LorenaTest.db";
            DB db = new DB($"Data Source={dbFile};Version=3");
            if (!File.Exists(dbFile))
            {
                db.CreateDB();
                DataInitialize.DefaultData(db);
            }
            int count = db.SelectCountFromSalon();

            for (int i = 1; i <= count; i++)
            {
                Salon salon = db.SelectSalonById(i);
                Console.Write($"Введите цену для {salon.Name}: ");
                double price = Convert.ToDouble(Console.ReadLine());
                int parentDiscount = 0;
                if (salon.HasDependency && salon.ParentId != null)
                {
                    var parent = db.SelectSalonById(salon.ParentId.Value);
                    while (parent != null)
                    {
                        parentDiscount += parent.Discount;
                        parent = parent.ParentId != null ? db.SelectSalonById(parent.ParentId.Value) : null;
                    }
                }
                CalculateTable ct = new CalculateTable(i, price, salon.Discount, parentDiscount, null);
                db.InsertCalculateTable(i, ct.Price, ct.Discount, ct.ParentDiscount, ct.FinalPrice);
            }

            List<CalculateTable> ctList = DataInitialize.GetCalculateTableList(db, db.SelectCountFromSalon());
            GetCalculateTable(db, ctList);
            Console.ReadLine();
        }



        private static void GetCalculateTable(DB db, List<CalculateTable> list)
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("|   Салон   |   Цена   |  Общая скидка   |  Итоговая цена |");
            Console.WriteLine("-----------------------------------------------------------");
            foreach (var ct in list)
            {
                int totalDiscount = ct.Discount + ct.ParentDiscount;
                string name = db.SelectSalonById(ct.SalonId).Name;
                Console.WriteLine($" {name}\t\t{ct.Price}\t\t{totalDiscount}\t\t{ct.FinalPrice}");
            }
        }
    }
}