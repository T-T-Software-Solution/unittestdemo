using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Demo.AppCore.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Demo.AppCore.Tests.Services;

public class GradeCalculationServiceTests
{
    private readonly Mock<IOptions<GradeConfiguration>> _mockOptions;
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly GradeCalculationService _gradeService;
    private readonly GradeConfiguration _gradeConfig;

    public GradeCalculationServiceTests()
    {
        // Arrange - Common setup
        _gradeConfig = new GradeConfiguration
        {
            AThreshold = 90,
            BThreshold = 80,
            CThreshold = 70,
            DThreshold = 60
        };

        _mockOptions = new Mock<IOptions<GradeConfiguration>>();
        _mockOptions.Setup(x => x.Value).Returns(_gradeConfig);

        _mockStudentService = new Mock<IStudentService>();

        _gradeService = new GradeCalculationService(_mockOptions.Object, _mockStudentService.Object);
    }

    [Fact]
    public async Task CalculateGradeFromResultsAsync_WithValidResults_ReturnsCorrectWeightedGrade()
    {
        // Arrange
        var exam1 = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 0.4m };
        var exam2 = new Exam { Id = Guid.NewGuid(), MaxScore = 50, Weight = 0.6m };
        
        var examResults = new List<ExamResult>
        {
            new() { ExamId = exam1.Id, Score = 85 }, // 85/100 = 0.85 * 0.4 = 0.34
            new() { ExamId = exam2.Id, Score = 45 }  // 45/50 = 0.9 * 0.6 = 0.54
        };
        
        var exams = new List<Exam> { exam1, exam2 };

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(88.00m, result.FinalPercent); // (0.34 + 0.54) / 1.0 * 100 = 88%
        Assert.Equal("B", result.Letter);
    }

    [Fact]
    public async Task CalculateGradeFromResultsAsync_WithNoResults_ReturnsZeroGrade()
    {
        // Arrange
        var examResults = new List<ExamResult>();
        var exams = new List<Exam>();

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.FinalPercent);
        Assert.Equal("F", result.Letter);
    }

    [Fact]
    public async Task CalculateGradeFromResultsAsync_WithScoreAboveMax_ClampsScore()
    {
        // Arrange
        var exam = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 1.0m };
        var examResults = new List<ExamResult>
        {
            new() { ExamId = exam.Id, Score = 150 } // Score above max should be clamped to 100
        };
        var exams = new List<Exam> { exam };

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.00m, result.FinalPercent);
        Assert.Equal("A", result.Letter);
    }

    [Fact]
    public async Task CalculateGradeFromResultsAsync_WithNegativeScore_ClampsToZero()
    {
        // Arrange
        var exam = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 1.0m };
        var examResults = new List<ExamResult>
        {
            new() { ExamId = exam.Id, Score = -10 } // Negative score should be clamped to 0
        };
        var exams = new List<Exam> { exam };

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.FinalPercent);
        Assert.Equal("F", result.Letter);
    }

    [Theory]
    [InlineData(95, "A")]
    [InlineData(90, "A")]
    [InlineData(89.99, "B")]
    [InlineData(80, "B")]
    [InlineData(79.99, "C")]
    [InlineData(70, "C")]
    [InlineData(69.99, "D")]
    [InlineData(60, "D")]
    [InlineData(59.99, "F")]
    [InlineData(0, "F")]
    public async Task CalculateGradeFromResultsAsync_WithDifferentScores_ReturnsCorrectLetterGrade(decimal score, string expectedLetter)
    {
        // Arrange
        var exam = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 1.0m };
        var examResults = new List<ExamResult>
        {
            new() { ExamId = exam.Id, Score = score }
        };
        var exams = new List<Exam> { exam };

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.Equal(expectedLetter, result.Letter);
    }

    [Fact]
    public async Task CalculateGradeFromResultsAsync_WithExamMaxScoreZero_IgnoresExam()
    {
        // Arrange
        var exam1 = new Exam { Id = Guid.NewGuid(), MaxScore = 0, Weight = 0.5m }; // Should be ignored
        var exam2 = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 0.5m };
        
        var examResults = new List<ExamResult>
        {
            new() { ExamId = exam1.Id, Score = 50 }, // Should be ignored
            new() { ExamId = exam2.Id, Score = 80 }  // 80/100 = 0.8 * 0.5 / 0.5 = 80%
        };
        var exams = new List<Exam> { exam1, exam2 };

        // Act
        var result = await _gradeService.CalculateGradeFromResultsAsync(examResults, exams);

        // Assert
        Assert.Equal(80.00m, result.FinalPercent);
        Assert.Equal("B", result.Letter);
    }
}