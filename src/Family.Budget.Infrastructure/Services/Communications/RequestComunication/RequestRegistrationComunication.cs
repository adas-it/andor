namespace Family.Budget.Infrastructure.Services.Communications.RequestComunication;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.Entities.Registrations;
using Family.Budget.Infrastructure.Services.Communications.Models;
using Hangfire;

public class RequestRegistrationComunication : IRequestRegistrationComunication
{
    private readonly IMessageSenderInterface _messageSender;
    private readonly IConfigurationRepository _repository;

    public RequestRegistrationComunication(IMessageSenderInterface messageSender,
        IConfigurationRepository repository)
    {
        _messageSender = messageSender;
        _repository = repository;
    }

    public Task Send(Registration registration, CancellationToken cancellationToken)
    {
        BackgroundJob.Enqueue(() => Proceed(registration.Email, 
            "en", 
            registration.CheckCode, 
            registration.FirstName, 
            cancellationToken));

        return Task.CompletedTask;
    }

    public async Task Proceed(string email, string contentLanguage, string code, string firstName, CancellationToken cancellationToken )
    {
        var template = await _repository.GetByNameActive("wellcome_email", cancellationToken);

        if(template == null)
        {
            throw new Exception("Template Not Found");
        }

        var values = new Dictionary<string, string>() { 
            { "<<code>>", code },
            { "<<name>>", firstName },
        };

        var data = new SendComunicationRequest(new Guid(template.Value!), email, null, null, contentLanguage, values);
        await _messageSender.SendQueue("SendEmail", data);
    }
}
