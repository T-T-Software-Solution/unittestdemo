using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Demo.AppCore.Events;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IGradeCalculationService _gradeService;
    private readonly INotificationService _notificationService;

    public StudentsController(
        IStudentService studentService,
        IGradeCalculationService gradeService,
        INotificationService notificationService)
    {
        _studentService = studentService;
        _gradeService = gradeService;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(Guid id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpGet("{id}/grade")]
    public async Task<ActionResult<Grade>> GetStudentGrade(Guid id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
            return NotFound();

        var grade = await _gradeService.CalculateGradeAsync(id);
        return Ok(grade);
    }

    [HttpPost]
    public async Task<ActionResult<Student>> CreateStudent(Student student)
    {
        var createdStudent = await _studentService.CreateStudentAsync(student);
        
        await _notificationService.NotifyStudentCreatedAsync(new StudentCreatedEvent(
            createdStudent.Id,
            createdStudent.StudentNo,
            createdStudent.FirstName,
            createdStudent.LastName,
            createdStudent.Email,
            createdStudent.Phone
        ));

        return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Student>> UpdateStudent(Guid id, Student student)
    {
        if (id != student.Id)
            return BadRequest();

        var existingStudent = await _studentService.GetStudentByIdAsync(id);
        if (existingStudent == null)
            return NotFound();

        var updatedStudent = await _studentService.UpdateStudentAsync(student);
        
        await _notificationService.NotifyStudentUpdatedAsync(new StudentUpdatedEvent(
            updatedStudent.Id,
            updatedStudent.StudentNo,
            updatedStudent.FirstName,
            updatedStudent.LastName,
            updatedStudent.Email,
            updatedStudent.Phone
        ));

        return Ok(updatedStudent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
            return NotFound();

        await _studentService.DeleteStudentAsync(id);
        
        await _notificationService.NotifyStudentDeletedAsync(new StudentDeletedEvent(
            student.Id,
            student.StudentNo,
            student.FirstName,
            student.LastName
        ));

        return NoContent();
    }
}