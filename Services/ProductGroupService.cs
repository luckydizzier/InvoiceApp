using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class ProductGroupService : BaseService<ProductGroup>, IProductGroupService
    {
        public ProductGroupService(IProductGroupRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
        }

        // CRUD operations handled by BaseService
    }
}
