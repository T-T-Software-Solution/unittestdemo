namespace Demo.AppCore.Events;

public record StudentCreatedEvent(Guid StudentId, string? StudentNo, string? FirstName, string? LastName, string? Email, string? Phone);

public record StudentUpdatedEvent(Guid StudentId, string? StudentNo, string? FirstName, string? LastName, string? Email, string? Phone);

public record StudentDeletedEvent(Guid StudentId, string? StudentNo, string? FirstName, string? LastName);

public record GradeChangedEvent(Guid StudentId, string? StudentNo, decimal OldGrade, decimal NewGrade, string OldLetter, string NewLetter);