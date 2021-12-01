using L01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel(Product model)
        {
            Id = model.Id;
            Code = model.Code;
            Stoc = model.Stoc;
            UnitPrice = model.UnitPrice;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int Stoc { get; set; }
        public double UnitPrice { get; set; }
    }
}
