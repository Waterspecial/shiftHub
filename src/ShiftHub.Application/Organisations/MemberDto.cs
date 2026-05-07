namespace ShiftHub.Application.Organisations;

public record MemberDto(
    Guid UserId,
    string FullName,
    string Email,
    string Phone,
    string Role,
    string Status,
    DateTime JoinedAt
);
