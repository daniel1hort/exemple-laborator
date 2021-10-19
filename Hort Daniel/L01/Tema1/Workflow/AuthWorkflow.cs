using L01.Fake;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static L01.Domain.CartState;
using static L01.Workflow.AuthEvent;
using static L01.Domain.AppDomain;

namespace L01.Workflow
{
    public class AuthWorkflow
    {
        public IAuthEvent Execute(AuthCmd cmd)
        {
            var userResult = Authenticate(cmd.Password);
            var (response, user, exit) = userResult.Case switch
            {
                SomeCase<User> a => ($"Welcome {a.Value.Name}", a.Value, false),
                NoneCase<User> _ => ("Invalid credentials", null, true)
            };
            var cart = (CreateCart(user) as EmptyCart).Cart;

            return exit switch
            {
                false => new AuthorizedEvent(response, user, cart),
                _ => new UnauthorizedEvent(response)
            };
        }
    }
}
