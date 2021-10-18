using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Fake
{
    public static class FakeDB
    {
        public static IEnumerable<Product> LoadProducts()
        {
            return new List<Product>
            {
                new Product(1, "Vise si sperante", 99.99f),
                new Product(2, "Zile de trait", 1.00f),
                new Product(3, "Bec de iluminat alimentat cu energie solara", 69.99f),
                new Product(4, "Bere cu alcool, supliment alimentar", 4.01f),
                new Product(6, "Numarul 5", 0.00f),
            };
        }
    }
}
