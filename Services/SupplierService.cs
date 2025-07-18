using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        public SupplierService(ISupplierRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
        }

        // CRUD methods provided by BaseService
    }
}
