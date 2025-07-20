using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Infrastructure.Repositories
{
    public interface IInvoiceItemRepository : ICrudRepository<InvoiceItem>
    {
    }
}
