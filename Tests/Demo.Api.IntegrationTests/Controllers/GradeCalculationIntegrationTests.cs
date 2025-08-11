using Demo.AppCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Demo.Database.Context;
using Microsoft.EntityFrameworkCore;
using Allure.Xunit.Attributes;
using Allure.Net.Commons;

namespace Demo.Api.IntegrationTests.Controllers;

[AllureFeature("Grade Calculation API")]
[AllureSuite("Integration Tests")]
public class GradeCalculationIntegrationTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GradeCalculationIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await CleanupDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetStudentGrade_WithExamResults_ReturnsCalculatedGrade()
    {
        // Arrange - Create student, exams, and exam results directly in database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentNo = "S001",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };

        var exam1 = new Exam
        {
            Id = Guid.NewGuid(),
            Name = "Math Midterm",
            MaxScore = 100,
            Weight = 0.6m
        };

        var exam2 = new Exam
        {
            Id = Guid.NewGuid(),
            Name = "Math Final",
            MaxScore = 50,
            Weight = 0.4m
        };

        var result1 = new ExamResult
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            ExamId = exam1.Id,
            Score = 85 // 85/100 * 0.6 = 0.51
        };

        var result2 = new ExamResult
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            ExamId = exam2.Id,
            Score = 40 // 40/50 * 0.4 = 0.32
        };

        context.Students.Add(student);
        context.Exams.AddRange(exam1, exam2);
        context.ExamResults.AddRange(result1, result2);
        await context.SaveChangesAsync();

        // Act - Call the grade endpoint
        var response = await _client.GetAsync($"/api/students/{student.Id}/grade");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var grade = await response.Content.ReadFromJsonAsync<Grade>();
        Assert.NotNull(grade);
        
        // Expected: (0.51 + 0.32) / 1.0 * 100 = 83%
        Assert.Equal(83.00m, grade.FinalPercent);
        Assert.Equal("B", grade.Letter);
    }

    [Fact]
    public async Task GetStudentGrade_WithComplexWeightedExams_ReturnsCorrectCalculation()
    {
        // Arrange - Create complex scenario with multiple exams
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentNo = "S002",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com"
        };

        var homework = new Exam { Id = Guid.NewGuid(), Name = "Homework", MaxScore = 10, Weight = 0.2m };
        var quiz = new Exam { Id = Guid.NewGuid(), Name = "Quiz", MaxScore = 25, Weight = 0.3m };
        var midterm = new Exam { Id = Guid.NewGuid(), Name = "Midterm", MaxScore = 100, Weight = 0.2m };
        var final = new Exam { Id = Guid.NewGuid(), Name = "Final", MaxScore = 100, Weight = 0.3m };

        var results = new[]
        {
            new ExamResult { Id = Guid.NewGuid(), StudentId = student.Id, ExamId = homework.Id, Score = 9 },   // 9/10 * 0.2 = 0.18
            new ExamResult { Id = Guid.NewGuid(), StudentId = student.Id, ExamId = quiz.Id, Score = 20 },     // 20/25 * 0.3 = 0.24
            new ExamResult { Id = Guid.NewGuid(), StudentId = student.Id, ExamId = midterm.Id, Score = 88 },  // 88/100 * 0.2 = 0.176
            new ExamResult { Id = Guid.NewGuid(), StudentId = student.Id, ExamId = final.Id, Score = 92 }     // 92/100 * 0.3 = 0.276
        };

        context.Students.Add(student);
        context.Exams.AddRange(homework, quiz, midterm, final);
        context.ExamResults.AddRange(results);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/students/{student.Id}/grade");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var grade = await response.Content.ReadFromJsonAsync<Grade>();
        Assert.NotNull(grade);
        
        // Expected: (0.18 + 0.24 + 0.176 + 0.276) / 1.0 * 100 = 87.2%
        Assert.Equal(87.20m, grade.FinalPercent);
        Assert.Equal("B", grade.Letter);
    }

    [Fact]
    public async Task GetStudentGrade_WithNonExistentStudent_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/students/{nonExistentId}/grade");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task CleanupDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        
        try
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"ExamResults\"");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Students\"");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Exams\"");
        }
        catch
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}