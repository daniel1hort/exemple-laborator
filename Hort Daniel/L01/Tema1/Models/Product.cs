using System;
using System.Collections.Generic;

#nullable disable

namespace L01.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int Stoc { get; set; }
        public double UnitPrice { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
