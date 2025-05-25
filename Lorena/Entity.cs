using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorena
{
    public class Salon
    {

            [StringLength(100)]
            public string Name { get; private set; }

            [Range(0, 100)]
            public int Discount { get; private set; }
            public bool HasDependency { get; private set; }

            [MaxLength(124)]
            public string Description { get; private set; }

            public int? ParentId { get; private set; }

            private DB _db { get; set; }

            public Salon(DB db, string name, int discount, bool hasDependency, string description, int? parentId)
            {
                _db = db;
                Name = name;
                Discount = discount;
                HasDependency = hasDependency;
                Description = description;
                ParentId = parentId;
            }


        
    }

    public class CalculateTable
    {
        public int SalonId { get; private set; }
        public double Price { get; private set; }

        [Range(0, 100)]
        public int Discount { get; private set; }
        [Range(0, 100)]
        public int ParentDiscount { get; private set; }

        public double FinalPrice { get; private set; }


        public CalculateTable(int salonId, double price, int discount, int parentDiscount, double? finalPrice)
        {
            SalonId = salonId;
            Price = price;
            Discount = discount;
            ParentDiscount = parentDiscount;
            FinalPrice = finalPrice is null ?
                price - (price * ((double)(discount + parentDiscount) / 100))
                : (double)finalPrice;
        }
    }
}
