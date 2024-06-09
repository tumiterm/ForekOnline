using ElecPOE.Models;

namespace ElecPOE.Contract
{
    public interface IStudentService
    {
        Task<List<Student>> GetStudentsAsync(string token);
        Task<Student> GetStudentAsync(string studentNumber);
    }
}
