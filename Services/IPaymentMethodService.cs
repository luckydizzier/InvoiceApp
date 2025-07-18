using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync();
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task SaveAsync(PaymentMethod method);
        Task DeleteAsync(int id);
    }
}
