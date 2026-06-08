using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class StudentServiceClient : IStudentServiceClient
    {
        private readonly HttpClient _httpClient;

        public StudentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<StudentDto>>("students");
                return response ?? Array.Empty<StudentDto>();
            }
            catch
            {
                return Array.Empty<StudentDto>();
            }
        }
    }
}
