using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Application.Services;

namespace InvoiceApp.Application.UseCases
{
    /// <summary>
    /// Handles creating a new invoice using the domain service.
    /// </summary>
    public class CreateInvoice
    {
        private readonly IInvoiceService _service;
        public CreateInvoice(IInvoiceService service)
        {
            _service = service;
        }

        public Task ExecuteAsync(Invoice invoice)
        {
            return _service.SaveAsync(invoice);
        }
    }
}
