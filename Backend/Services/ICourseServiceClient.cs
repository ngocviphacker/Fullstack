using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Services
{
    public interface ICourseServiceClient
    {
        Task<IEnumerable<CourseDto>> GetCoursesAsync();
    }
}
