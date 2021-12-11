using Example.Events;
using Example.Events.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static L01.Workflow.GenerateInvoice.GenerateInvoiceEvent;

namespace Example.Accomodation.EventProcessor
{
    internal class InvoicedGeneratedEventHandler : AbstractEventHandler<InvoiceGeneratedEvent>
    {
        public override string[] EventTypes => new string[]{typeof(InvoiceGeneratedEvent).Name};

        protected override Task<EventProcessingResult> OnHandleAsync(InvoiceGeneratedEvent eventData)
        {
            // call GenerateInvoiceWorkflow
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
