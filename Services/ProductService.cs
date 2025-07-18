using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(IProductRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
        }

        // GetAllAsync, GetByIdAsync, SaveAsync and DeleteAsync provided by BaseService

        // SaveAsync and DeleteAsync provided by BaseService
    }
}
