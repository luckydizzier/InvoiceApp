using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class InvoiceItemService : BaseService<InvoiceItem>, IInvoiceItemService
    {
        public InvoiceItemService(IInvoiceItemRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
        }

        // CRUD methods provided by BaseService
    }
}
