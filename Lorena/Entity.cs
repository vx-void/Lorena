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
}
