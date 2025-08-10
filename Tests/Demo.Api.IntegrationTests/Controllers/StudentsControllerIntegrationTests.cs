using Demo.AppCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Demo.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.IntegrationTests.Controllers;

public class StudentsControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public StudentsControllerIntegrationTests(TestWebApplicationFactory factory)
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
    public async Task GetStudents_ShouldReturnEmptyList_WhenNoStudentsExist()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/students");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var students = await response.Content.ReadFromJsonAsync<List<Student>>();
        Assert.NotNull(students);
        Assert.Empty(students);
    }

    [Fact]
    public async Task CreateStudent_ShouldReturnCreatedStudent_WithValidData()
    {
        // Arrange
        var newStudent = new Student
        {
            StudentNo = "S001",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Phone = "1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/students", newStudent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdStudent = await response.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(createdStudent);
        Assert.NotEqual(Guid.Empty, createdStudent.Id);
        Assert.Equal("S001", createdStudent.StudentNo);
        Assert.Equal("John", createdStudent.FirstName);
        Assert.Equal("Doe", createdStudent.LastName);
        Assert.Equal("john.doe@test.com", createdStudent.Email);
        Assert.Equal("1234567890", createdStudent.Phone);

        // Verify Location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains(createdStudent.Id.ToString(), response.Headers.Location.ToString());
    }

    [Fact]
    public async Task GetStudent_ShouldReturnStudent_WhenStudentExists()
    {
        // Arrange - Create a student first
        var student = await CreateTestStudentAsync();

        // Act
        var response = await _client.GetAsync($"/api/students/{student.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var retrievedStudent = await response.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(retrievedStudent);
        Assert.Equal(student.Id, retrievedStudent.Id);
        Assert.Equal(student.StudentNo, retrievedStudent.StudentNo);
    }

    [Fact]
    public async Task GetStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/students/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnUpdatedStudent_WithValidData()
    {
        // Arrange - Create a student first
        var student = await CreateTestStudentAsync();
        student.FirstName = "Jane";
        student.LastName = "Smith";

        // Act
        var response = await _client.PutAsJsonAsync($"/api/students/{student.Id}", student);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var updatedStudent = await response.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(updatedStudent);
        Assert.Equal("Jane", updatedStudent.FirstName);
        Assert.Equal("Smith", updatedStudent.LastName);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnNoContent_WhenStudentExists()
    {
        // Arrange - Create a student first
        var student = await CreateTestStudentAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/students/{student.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify student is deleted
        var getResponse = await _client.GetAsync($"/api/students/{student.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetStudentGrade_ShouldReturnZeroGrade_WhenNoExamResults()
    {
        // Arrange - Create a student first
        var student = await CreateTestStudentAsync();

        // Act
        var response = await _client.GetAsync($"/api/students/{student.Id}/grade");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var grade = await response.Content.ReadFromJsonAsync<Grade>();
        Assert.NotNull(grade);
        Assert.Equal(0m, grade.FinalPercent);
        Assert.Equal("F", grade.Letter);
    }

    private async Task<Student> CreateTestStudentAsync()
    {
        var student = new Student
        {
            StudentNo = $"S{Random.Shared.Next(1000, 9999)}",
            FirstName = "Test",
            LastName = "Student",
            Email = $"test{Random.Shared.Next(1000, 9999)}@test.com",
            Phone = "1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/students", student);
        response.EnsureSuccessStatusCode();
        
        var createdStudent = await response.Content.ReadFromJsonAsync<Student>();
        return createdStudent!;
    }

    private async Task CleanupDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        
        try
        {
            // Delete data in dependency order
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"ExamResults\"");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Students\"");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Exams\"");
        }
        catch (Exception ex)
        {
            // Fallback: create fresh database if cleanup fails
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}