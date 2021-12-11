using CSharp.Choices;
using L01.Domain;
using L01.Fake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Workflow
{
    [AsChoice]
    public static partial class AuthEvent
    {
        public interface IAuthEvent { }
        public record AuthorizedEvent(string Message, User User, Cart Cart) : IAuthEvent;
        public record UnauthorizedEvent(string Message) : IAuthEvent;
    }
}
