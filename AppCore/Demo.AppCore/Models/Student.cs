using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.AppCore.Models;

public class Student
{
    public Guid Id { get; set; }
    
    [MaxLength(50)]
    public string? StudentNo { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(255)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}