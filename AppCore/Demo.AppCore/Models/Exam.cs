using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.AppCore.Models;

public class Exam
{
    public Guid Id { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal MaxScore { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Weight { get; set; }
    
    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}