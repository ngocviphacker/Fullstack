using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public interface IRegistrationServiceClient
    {
        Task<IEnumerable<RegistrationDto>> GetRegistrationsAsync();
    }
}
