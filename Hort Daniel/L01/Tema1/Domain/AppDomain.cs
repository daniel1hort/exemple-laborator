using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L01.Fake;
using static L01.Domain.CartState;
using L01.Models;

namespace L01.Domain
{
    public static class AppDomain
    {
        public static ICartState CreateCart(User user)
            => new EmptyCart(new Cart(user));

        public static ICartState AddItemToCart(Cart cart, TryAsync<(Product, int)> item)
        {
            var (newItem, message) = item.Case().Result switch
            {
                SuccCase<(Product, int)> a => (a.Value, string.Empty),
                FailCase<(Product, int)> a => (default, a.Error.Message)
            };
            if (!string.IsNullOrWhiteSpace(message))
                return new InvalidCart(cart, message);

            var oldItem = cart.Items.FirstOrDefault(a => a.Product.Id == newItem.Item1.Id);
            var finalCart = oldItem switch
            {
                (Product p, int q) => 
                    cart with { Items = cart.Items.Filter(a => a.Product.Id != p.Id).Append((p, q + newItem.Item2)) },
                _ => cart with { Items = cart.Items.Append(newItem)}
            };

            return new ValidCart(finalCart);
        }

        private static Product ChangeStoc(Product p, int quantity) { p.Stoc = p.Stoc - quantity; return p; }
        public static ICartState PayCart(Cart cart, string address, PSSCContext context)
        {
            var order = new OrderHeader()
            {
                Address = address,
                OrderLines = cart.Items.Select(a => new OrderLine()
                {
                    ProductId = a.Product.Id,
                    Quantity = a.Quantity,
                    Price = a.Product.UnitPrice * a.Quantity
                }).ToList(),
                TotalPrice = cart.Items.Select(a => a.Product.UnitPrice * a.Quantity).Aggregate((a, b) => a + b)
            };

            var updatedProducts = from product in context.Products.AsEnumerable()
                                  from item in cart.Items
                                  where product.Id == item.Product.Id
                                  select ChangeStoc(product, item.Quantity);

            context.OrderHeaders.Add(order);
            context.Products.UpdateRange(updatedProducts);
            return new PaidCart();
        }

        public static Option<User> Authenticate(string password)
            => string.IsNullOrWhiteSpace(password) switch
            {
                false => new User("Mister Mister", password),
                _ => null
            };

        public static TryAsync<(Product, int)> CheckProducts(Cart cart, IEnumerable<Product> products,
            string productIdRaw, string quantityRaw)
            => async () =>
            {
                var product = products.FirstOrDefault(a => a.Id.ToString() == productIdRaw);
                if (product is null)
                    throw new Exception("The product doesn't exist");
                var oldQuantity = cart.Items.FirstOrDefault(a => a.Product.Id.ToString() == productIdRaw).Quantity;
                var quantity = int.Parse(quantityRaw);
                if (quantity <= 0)
                    throw new Exception("Quantity must be bigger than 0");
                if (oldQuantity + quantity > product.Stoc)
                    throw new Exception("Not enough items in stock");
                return (product, quantity);
            };
    }
}
