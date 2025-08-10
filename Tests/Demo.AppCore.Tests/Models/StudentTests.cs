using Demo.AppCore.Models;

namespace Demo.AppCore.Tests.Models;

public class StudentTests
{
    [Fact]
    public void Student_Creation_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        Assert.Equal(Guid.Empty, student.Id);
        Assert.Null(student.StudentNo);
        Assert.Null(student.FirstName);
        Assert.Null(student.LastName);
        Assert.Null(student.Email);
        Assert.Null(student.Phone);
        Assert.NotNull(student.ExamResults);
        Assert.Empty(student.ExamResults);
    }

    [Fact]
    public void Student_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new Student();

        // Act
        student.Id = studentId;
        student.StudentNo = "S001";
        student.FirstName = "John";
        student.LastName = "Doe";
        student.Email = "john.doe@example.com";
        student.Phone = "1234567890";

        // Assert
        Assert.Equal(studentId, student.Id);
        Assert.Equal("S001", student.StudentNo);
        Assert.Equal("John", student.FirstName);
        Assert.Equal("Doe", student.LastName);
        Assert.Equal("john.doe@example.com", student.Email);
        Assert.Equal("1234567890", student.Phone);
    }

    [Fact]
    public void Student_ExamResults_ShouldAcceptCollection()
    {
        // Arrange
        var student = new Student();
        var examResult1 = new ExamResult { Id = Guid.NewGuid() };
        var examResult2 = new ExamResult { Id = Guid.NewGuid() };

        // Act
        student.ExamResults.Add(examResult1);
        student.ExamResults.Add(examResult2);

        // Assert
        Assert.Equal(2, student.ExamResults.Count);
        Assert.Contains(examResult1, student.ExamResults);
        Assert.Contains(examResult2, student.ExamResults);
    }
}