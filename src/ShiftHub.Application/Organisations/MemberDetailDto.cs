namespace ShiftHub.Application.Organisations;

public record MemberDetailDto(
    Guid UserId,
    string FullName,
    string Email,
    string Phone,
    string Role,
    string Status,
    DateTime JoinedAt,
    IReadOnlyList<MemberShiftDto> RecentShifts
);

public record MemberShiftDto(
    Guid AssignmentId,
    Guid ShiftId,
    string SiteName,
    string ClientName,
    DateTime StartTime,
    DateTime EndTime,
    string AssignmentStatus,
    string ShiftStatus,
    DateTime? ClockIn,
    DateTime? ClockOut,
    decimal? HoursWorked
);
