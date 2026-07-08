using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andor.Authentication.Jwt;

public record IdentityProvider
{
    public List<string>? Scopes { get; init; }
    public string? SecretKey { get; init; }
    public string? Authority { get; init; }
    public string? SwaggerClientId { get; init; }
}
