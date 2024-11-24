namespace Andor.Application.Dto.Communications.Requests;

public record NotificationsOutput(
    Guid Id,
    string Type,
    string Title,
    string Message,
    string Date,
    bool Read,
    int Cursor);
