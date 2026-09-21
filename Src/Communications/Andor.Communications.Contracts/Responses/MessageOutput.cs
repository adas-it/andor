namespace Andor.Communications.Contracts.Responses;

public record MessageOutput(Guid Id, string Title, string Body, DateTimeOffset SentAt);
