using System;
using System.Collections.Generic;

#nullable disable

namespace L01.Models
{
    public partial class OrderLine
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public virtual OrderHeader Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
