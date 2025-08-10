using Demo.AppCore.Models;

namespace Demo.AppCore.Interfaces;

public interface IGradeCalculationService
{
    Task<Grade> CalculateGradeAsync(Guid studentId);
    Task<Grade> CalculateGradeFromResultsAsync(IEnumerable<ExamResult> examResults, IEnumerable<Exam> exams);
}