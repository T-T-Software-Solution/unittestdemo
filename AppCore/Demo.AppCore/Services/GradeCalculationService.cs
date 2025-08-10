using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Microsoft.Extensions.Options;

namespace Demo.AppCore.Services;

public class GradeCalculationService : IGradeCalculationService
{
    private readonly GradeConfiguration _gradeConfig;

    public GradeCalculationService(IOptions<GradeConfiguration> gradeConfig)
    {
        _gradeConfig = gradeConfig.Value;
    }

    public Task<Grade> CalculateGradeAsync(Guid studentId)
    {
        // This method would require database access to fetch student and exam data
        // For now, return a default grade - this should be implemented properly with database service injection
        return Task.FromResult(new Grade { FinalPercent = 0, Letter = "F" });
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
        if (percent >= _gradeConfig.AThreshold) return "A";
        if (percent >= _gradeConfig.BThreshold) return "B";
        if (percent >= _gradeConfig.CThreshold) return "C";
        if (percent >= _gradeConfig.DThreshold) return "D";
        return "F";
    }
}