using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public interface IClassServiceClient
    {
        Task<IEnumerable<ClassDto>> GetClassesAsync();
    }
}
