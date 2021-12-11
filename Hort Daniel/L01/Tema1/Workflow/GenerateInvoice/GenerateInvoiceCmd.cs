using L01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Workflow.GenerateInvoice
{
    public record GenerateInvoiceCmd(Cart Cart, Address Address);
}
