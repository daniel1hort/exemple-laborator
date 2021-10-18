using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Domain
{
    [AsChoice]
    public static partial class CartState
    {
        public interface ICartState { }
        public record EmptyCart(Cart Cart) : ICartState;
        public record InvalidCart(Cart Cart, string Message) : ICartState;
        public record ValidCart(Cart Cart) : ICartState;
        public record PaidCart() : ICartState;
    }
}
