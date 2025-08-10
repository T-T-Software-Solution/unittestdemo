using Demo.AppCore.Models;

namespace Demo.AppCore.Interfaces;

public interface IStudentService
{
    Task<Student?> GetStudentByIdAsync(Guid id);
    Task<Student> CreateStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    Task DeleteStudentAsync(Guid id);
    Task<IEnumerable<Student>> GetAllStudentsAsync();
}