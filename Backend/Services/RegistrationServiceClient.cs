using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class RegistrationServiceClient : IRegistrationServiceClient
    {
        private readonly HttpClient _httpClient;

        public RegistrationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<RegistrationDto>> GetRegistrationsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<RegistrationDto>>("registrations");
                return response ?? Array.Empty<RegistrationDto>();
            }
            catch
            {
                return Array.Empty<RegistrationDto>();
            }
        }
    }
}
