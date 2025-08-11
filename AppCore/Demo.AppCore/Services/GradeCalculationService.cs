using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Microsoft.Extensions.Options;

namespace Demo.AppCore.Services;

public class GradeCalculationService : IGradeCalculationService
{
    private readonly GradeConfiguration _gradeConfig;
    private readonly IStudentService _studentService;

    public GradeCalculationService(IOptions<GradeConfiguration> gradeConfig, IStudentService studentService)
    {
        _gradeConfig = gradeConfig.Value;
        _studentService = studentService;
    }

    public async Task<Grade> CalculateGradeAsync(Guid studentId)
    {
        // Fetch student with exam results
        var student = await _studentService.GetStudentByIdAsync(studentId);
        if (student == null)
            return new Grade { FinalPercent = 0, Letter = "F" };

        // Extract exam results and related exams
        var examResults = student.ExamResults;
        var exams = examResults.Select(er => er.Exam).Distinct();

        // Use existing calculation logic
        return await CalculateGradeFromResultsAsync(examResults, exams);
    }

    public Task<Grade> CalculateGradeFromResultsAsync(IEnumerable<ExamResult> examResults, IEnumerable<Exam> exams)
    {
        var examDict = exams.ToDictionary(e => e.Id, e => e);
        
        decimal total = 0;
        decimal weightSum = 0;

        foreach (var result in examResults)
        {
            if (examDict.TryGetValue(result.ExamId, out var exam) && exam.MaxScore > 0)
            {
                var clampedScore = Math.Max(0, Math.Min(result.Score, exam.MaxScore));
                total += (clampedScore / exam.MaxScore) * exam.Weight;
                weightSum += exam.Weight;
            }
        }

        var finalPercent = weightSum == 0 ? 0 : Math.Round((total / weightSum) * 100, 2);
        var letter = CalculateLetterGrade(finalPercent);

        return Task.FromResult(new Grade
        {
            FinalPercent = finalPercent,
            Letter = letter
        });
    }

    private string CalculateLetterGrade(decimal percent)
    {
        return "C"; // Simple code to make fail in test for validate result in Github Action and Test Result

        if (percent >= _gradeConfig.AThreshold) return "A";
        if (percent >= _gradeConfig.BThreshold) return "B";
        if (percent >= _gradeConfig.CThreshold) return "C";
        if (percent >= _gradeConfig.DThreshold) return "D";
        return "F";
    }
}