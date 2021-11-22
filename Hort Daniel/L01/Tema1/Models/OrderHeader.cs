using System;
using System.Collections.Generic;

#nullable disable

namespace L01.Models
{
    public partial class OrderHeader
    {
        public OrderHeader()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public double TotalPrice { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
