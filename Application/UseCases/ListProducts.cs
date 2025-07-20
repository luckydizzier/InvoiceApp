using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Application.Services;

namespace InvoiceApp.Application.UseCases
{
    /// <summary>
    /// Retrieves all products for display.
    /// </summary>
    public class ListProducts
    {
        private readonly IProductService _service;
        public ListProducts(IProductService service)
        {
            _service = service;
        }

        public Task<IEnumerable<Product>> ExecuteAsync()
        {
            return _service.GetAllAsync();
        }
    }
}
