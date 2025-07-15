using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync();
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task AddAsync(PaymentMethod method);
        Task UpdateAsync(PaymentMethod method);
        Task DeleteAsync(int id);
    }
}
