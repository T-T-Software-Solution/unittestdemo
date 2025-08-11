using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Demo.AppCore.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Demo.AppCore.Tests.Services;

public class GradeCalculationServiceDatabaseTests
{
    private readonly Mock<IOptions<GradeConfiguration>> _mockOptions;
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly GradeCalculationService _gradeService;
    private readonly GradeConfiguration _gradeConfig;

    public GradeCalculationServiceDatabaseTests()
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
    public async Task CalculateGradeAsync_WithValidStudentAndExamResults_ReturnsCorrectGrade()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var exam1 = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 0.6m };
        var exam2 = new Exam { Id = Guid.NewGuid(), MaxScore = 50, Weight = 0.4m };
        
        var student = new Student
        {
            Id = studentId,
            ExamResults = new List<ExamResult>
            {
                new() { ExamId = exam1.Id, Score = 90, Exam = exam1 }, // 90/100 * 0.6 = 0.54
                new() { ExamId = exam2.Id, Score = 40, Exam = exam2 }  // 40/50 * 0.4 = 0.32
            }
        };

        _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _gradeService.CalculateGradeAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(86.00m, result.FinalPercent); // (0.54 + 0.32) / 1.0 * 100 = 86%
        Assert.Equal("B", result.Letter);
        
        _mockStudentService.Verify(s => s.GetStudentByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task CalculateGradeAsync_WithNonExistentStudent_ReturnsFailingGrade()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        
        _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _gradeService.CalculateGradeAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.FinalPercent);
        Assert.Equal("F", result.Letter);
        
        _mockStudentService.Verify(s => s.GetStudentByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task CalculateGradeAsync_WithStudentWithNoExamResults_ReturnsFailingGrade()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new Student
        {
            Id = studentId,
            ExamResults = new List<ExamResult>() // Empty exam results
        };

        _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _gradeService.CalculateGradeAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.FinalPercent);
        Assert.Equal("F", result.Letter);
        
        _mockStudentService.Verify(s => s.GetStudentByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task CalculateGradeAsync_WithPerfectScores_ReturnsAGrade()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var exam = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 1.0m };
        
        var student = new Student
        {
            Id = studentId,
            ExamResults = new List<ExamResult>
            {
                new() { ExamId = exam.Id, Score = 100, Exam = exam }
            }
        };

        _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _gradeService.CalculateGradeAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.00m, result.FinalPercent);
        Assert.Equal("A", result.Letter);
        
        _mockStudentService.Verify(s => s.GetStudentByIdAsync(studentId), Times.Once);
    }

    [Theory]
    [InlineData(95, "A")]
    [InlineData(85, "B")]
    [InlineData(75, "C")]
    [InlineData(65, "D")]
    [InlineData(55, "F")]
    public async Task CalculateGradeAsync_WithVariousScores_ReturnsCorrectLetterGrades(decimal score, string expectedLetter)
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var exam = new Exam { Id = Guid.NewGuid(), MaxScore = 100, Weight = 1.0m };
        
        var student = new Student
        {
            Id = studentId,
            ExamResults = new List<ExamResult>
            {
                new() { ExamId = exam.Id, Score = score, Exam = exam }
            }
        };

        _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _gradeService.CalculateGradeAsync(studentId);

        // Assert
        Assert.Equal(expectedLetter, result.Letter);
        Assert.Equal(score, result.FinalPercent);
        
        _mockStudentService.Verify(s => s.GetStudentByIdAsync(studentId), Times.Once);
    }
}