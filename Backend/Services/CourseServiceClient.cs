using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class CourseServiceClient : ICourseServiceClient
    {
        private readonly HttpClient _httpClient;

        public CourseServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<CourseDto>>("courses");
                return response ?? Array.Empty<CourseDto>();
            }
            catch
            {
                return Array.Empty<CourseDto>();
            }
        }
    }
}
