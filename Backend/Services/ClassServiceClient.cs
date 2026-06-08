using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class ClassServiceClient : IClassServiceClient
    {
        private readonly HttpClient _httpClient;

        public ClassServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ClassDto>> GetClassesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<ClassDto>>("classes");
                return response ?? Array.Empty<ClassDto>();
            }
            catch
            {
                return Array.Empty<ClassDto>();
            }
        }
    }
}
