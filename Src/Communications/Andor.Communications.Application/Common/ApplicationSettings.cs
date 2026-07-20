namespace Andor.Application.Common;

public class ApplicationSettings
{
    public SmtpConfig? SmtpConfig { get; set; }
}

public class SmtpConfig
{
    public string? Smtp { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DisplayName { get; set; }
}
