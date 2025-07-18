using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Repositories;
using Serilog;

namespace InvoiceApp.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        public UnitService(IUnitRepository repository, IChangeLogService logService)
            : base(repository, logService)
        {
        }

        // CRUD methods provided by BaseService
    }
}
