using LanguageExt;
using System;
using L01.Fake;
using System.Linq;
using static L01.Domain.AppDomain;
using static L01.Domain.CartState;
using L01.Extensions;
using L01.Domain;
using L01.Workflow;
using static L01.Workflow.AuthEvent;

namespace L01
{
    class Program
    {
        static void Main(string[] args)
        {
            var pass = RequestCredentials();
            var workflow = new AuthWorkflow();
            var authEvent = workflow.Execute(new AuthCmd(pass));
            var (response, user, cart, exit) = authEvent switch
            {
                AuthorizedEvent a => (a.Message, a.User, a.Cart, false),
                UnauthorizedEvent a => (a.Message, null, null, true)
            };
            Console.WriteLine(response);
            if (exit) return;

            while (true)
            {
                var (product, quantity) = RequestItem();
                var result = AddItemToCart(cart, product, quantity);
                var message = string.Empty; // Mixed declarations and expressions in destruction is currently in Preview
                (cart, message) = result switch
                {
                    ValidCart a => (a.Cart, ""),
                    InvalidCart a => (a.Cart, a.Message + (Environment.NewLine * (StringMultiplication)2))
                };
                Displayitems(cart);

                Console.Write(message);
                if (RequestPayment())
                {
                    PayCart(cart);
                    Console.WriteLine($"Good day {user.Name}");
                    break;
                }
            }
        }

        public static Unit Displayitems(Cart cart)
        {
            Console.WriteLine("Your items:");
            Console.Write(cart.Items
                .OrderBy(a => a.Product.Id)
                .Select(a => $"\t{a.Product.Id} / {a.Product.Name} / {a.Product.Price} / {a.Quantity}" + Environment.NewLine)
                .Aggregate(string.Empty, (a, b) => a + b));
            Console.Write("Total price: ");
            Console.WriteLine(cart.Items.Select(a => a.Product.Price * a.Quantity).Aggregate(0f, (a, b) => a + b));
            Console.WriteLine();
            return Unit.Default;
        }

        public static string RequestCredentials()
        {
            Console.Write("Please enter password: ");
            return Console.ReadLine();
        }

        public static (Option<Product>, TryAsync<int>) RequestItem()
        {
            var products = FakeDB.LoadProducts();
            products.Iter(a => Console.WriteLine($"Id: {a.Id}, Price: {a.Price}, Name: {a.Name}"));
            Console.WriteLine("Please select an item by it's Id and then input the quantity");
            Console.Write("Product Id: ");
            var productId = Console.ReadLine().Trim();
            Console.Write("Quantity: ");
            var quantityRaw = Console.ReadLine().Trim();
            var product = products.FirstOrDefault(a => a.Id.ToString() == productId);
            return (product, async () => {
                var quantity = int.Parse(quantityRaw);
                if (quantity <= 0)
                    throw new Exception("Quantity must be bigger than 0");
                return quantity;
            });
        }

        public static bool RequestPayment()
        {
            Console.WriteLine("Please insert \"Y\" if you want to pay or anything else to continue shopping:");
            return Console.ReadLine().Trim().ToLower() switch
            {
                "y" => true,
                _ => false
            };
        }
    }
}
