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

        public static ICartState AddItemToCart(Cart cart, Option<Product> product, TryAsync<int> quantity)
        {
            var (value, message) = quantity.Case().Result switch
            {
                SuccCase<int> a => (a.Value, null),
                FailCase<int> a => (default(int), a.Error.Message)
            };
            if (message is not null)
                return new InvalidCart(cart, message);
            if (product.IsNone)
                return new InvalidCart(cart, "The product doesn't exist");

            var item = ((Product)product, value);
            var oldItem = cart.Items.FirstOrDefault(a => a.Product.Id == item.Item1.Id);
            var finalCart = oldItem switch
            {
                (Product p, int q) => 
                    cart with { Items = cart.Items.Filter(a => a.Product.Id != p.Id).Append((p, q + value)) },
                _ => cart with { Items = cart.Items.Append(item)}
            };

            return new ValidCart(finalCart);
        }

        public static ICartState PayCart(Cart cart)
            => new PaidCart();

        public static Option<User> Authenticate(string password)
            => string.IsNullOrWhiteSpace(password) switch
            {
                false => new User("Mister Mister", password),
                _ => null
            };
    }
}
