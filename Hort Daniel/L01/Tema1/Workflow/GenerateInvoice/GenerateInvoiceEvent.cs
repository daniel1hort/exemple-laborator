using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Workflow.GenerateInvoice
{
    [AsChoice]
    public static partial class GenerateInvoiceEvent
    {
        public interface IGenerateInvoiceEvent { }

        public record InvoiceGeneratedEvent { }
    }
}
