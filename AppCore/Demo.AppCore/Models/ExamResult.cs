using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.AppCore.Models;

public class ExamResult
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ExamId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Score { get; set; }
    
    public virtual Student Student { get; set; } = null!;
    public virtual Exam Exam { get; set; } = null!;
}