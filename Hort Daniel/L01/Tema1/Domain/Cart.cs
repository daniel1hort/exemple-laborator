using L01.Fake;
using L01.Models;
using System.Collections.Generic;
using System.Linq;

namespace L01.Domain
{
    public record Cart(User User, IEnumerable<(Product Product, int Quantity)> Items)
    {
        public Cart(User user) : this(user, Enumerable.Empty<(Product, int)>()) { }
    }
}
