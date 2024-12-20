﻿namespace Andor.Application.Common.Models.Authorizations;

using Andor.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        User = new ApplicationUser(httpContext?.HttpContext);
    }

    public ApplicationUser User { get; }
}
