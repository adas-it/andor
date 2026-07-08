using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andor.Documentation.Swagger;

public record IdentityProvider
{
    public string Authority { get; set; }
    public string[] Scopes { get; set; }
    public string SwaggerClientId { get; set; }
    public string SecretKey { get; set; }
}
