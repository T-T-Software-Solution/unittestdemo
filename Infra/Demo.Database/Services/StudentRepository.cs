using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Demo.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Demo.Database.Services;

public class StudentRepository : IStudentService
{
    private readonly DemoDbContext _context;

    public StudentRepository(DemoDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetStudentByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.ExamResults)
            .ThenInclude(er => er.Exam)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        student.Id = Guid.NewGuid();
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        // Detach any existing tracked entity with the same ID
        var trackedEntity = _context.ChangeTracker.Entries<Student>()
            .FirstOrDefault(e => e.Entity.Id == student.Id);
        
        if (trackedEntity != null)
        {
            trackedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }
        
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task DeleteStudentAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.ExamResults)
            .ThenInclude(er => er.Exam)
            .ToListAsync();
    }
}