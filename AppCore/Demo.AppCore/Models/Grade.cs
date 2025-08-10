namespace Demo.AppCore.Models;

public class Grade
{
    public decimal FinalPercent { get; set; }
    public string Letter { get; set; } = string.Empty;
}

public class GradeConfiguration
{
    public decimal AThreshold { get; set; } = 90;
    public decimal BThreshold { get; set; } = 80;
    public decimal CThreshold { get; set; } = 70;
    public decimal DThreshold { get; set; } = 60;
}