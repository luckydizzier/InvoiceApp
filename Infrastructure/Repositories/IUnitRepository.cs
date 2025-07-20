using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Domain;

namespace InvoiceApp.Infrastructure.Repositories
{
    public interface IUnitRepository : ICrudRepository<Unit>
    {
    }
}
