namespace Family.Budget.Infrastructure.Services.Communications.Models;
using System;
using System.Collections.Generic;

public record SendComunicationRequest(Guid RuleId,
    string? Email,
    string? Phone,
    Guid? UserId,
    string? ContentLanguage,
    Dictionary<string, string>? Values);
