using LanguageExt;
using System;
using System.Linq;
using static L01.Domain.AppDomain;
using static L01.Domain.CartState;
using L01.Extensions;
using L01.Domain;
using L01.Workflow;
using static L01.Workflow.AuthEvent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using L01.Models;
using System.Threading.Tasks;

namespace L01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json").Build();
            var serviceProvider = new ServiceCollection()
                .AddDbContext<PSSCContext>(builder => builder
                .UseSqlServer(configuration.GetConnectionString("Local")))
                .BuildServiceProvider();
            var context = serviceProvider.GetService<PSSCContext>();
            var products = context.Products.AsEnumerable();

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
                var (product, quantity) = RequestItem(products);
                var result = AddItemToCart(cart, CheckProducts(cart, products, product, quantity));
                var message = string.Empty; // Mixed declarations and expressions in destruction is currently in Preview
                (cart, message) = result switch
                {
                    ValidCart a => (a.Cart, ""),
                    InvalidCart a => (a.Cart, a.Message + (Environment.NewLine * (StringMultiplication)2))
                };
                Displayitems(cart);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(message);
                Console.ResetColor();
                if (RequestPayment())
                {
                    PayCart(cart, RequestAddress(), context);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Good day {user.Name}");
                    break;
                }
            }
        }

        #region I/O
        public static Unit Displayitems(Cart cart)
        {
            Console.WriteLine("Your items:");
            Console.Write(cart.Items
                .OrderBy(a => a.Product.Id)
                .Select(a => $"\t{a.Product.Id} / {a.Product.Code} / {a.Product.UnitPrice:0.##} / {a.Quantity}" 
                + Environment.NewLine)
                .Aggregate(string.Empty, (a, b) => a + b));
            Console.Write("Total price: ");
            Console.WriteLine(cart.Items.Select(a => a.Product.UnitPrice * a.Quantity).Aggregate(0d, (a, b) => a + b).ToString("0.##"));
            Console.WriteLine();
            return Unit.Default;
        }

        public static string RequestCredentials()
        {
            Console.Write("Please enter password: ");
            return Console.ReadLine();
        }

        public static (string, string) RequestItem(IEnumerable<Product> products)
        {
            products.Iter(a => Console.WriteLine($"Id: {a.Id}, Price: {a.UnitPrice:0.##}, Quantity: {a.Stoc}, Name: {a.Code}"));
            Console.WriteLine("Please select an item by it's Id and then input the quantity");
            Console.Write("Product Id: ");
            var productId = Console.ReadLine().Trim();
            Console.Write("Quantity: ");
            var quantityRaw = Console.ReadLine().Trim();
            return (productId, quantityRaw);
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

        public static string RequestAddress()
        {
            Console.Write("Plase write your address: ");
            return Console.ReadLine().Trim();
        }
        #endregion
    }
}
