using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L01.Fake;
using static L01.Domain.CartState;

namespace L01.Domain
{
    public static class AppDomain
    {
        public static ICartState CreateCart(User user)
            => new EmptyCart(new Cart(user));

        public static ICartState AddItemToCart(Cart cart, Option<Product> product, int quantity)
        {
            if (quantity <= 0)
                return new InvalidCart(cart, "Quantity must be bigger than 0");
            if (product.IsNone)
                return new InvalidCart(cart, "The product doesn't exist");

            var item = ((Product)product, quantity);
            var oldItem = cart.Items.FirstOrDefault(a => a.Product.Id == item.Item1.Id);
            var finalCart = oldItem switch
            {
                (Product p, int q) => 
                    cart with { Items = cart.Items.Filter(a => a.Product.Id != p.Id).Append((p, q + quantity)) },
                _ => cart with { Items = cart.Items.Append(item)}
            };

            return new ValidCart(finalCart);
        }

        public static ICartState PayCart(Cart cart)
            => new PaidCart(); // Nu prea am chestii de facut in situatia asta

        public static Option<User> Authenticate(string password)
            => string.IsNullOrWhiteSpace(password) switch
            {
                false => new User("Mister Mister", password),
                _ => null
            };
    }
}
