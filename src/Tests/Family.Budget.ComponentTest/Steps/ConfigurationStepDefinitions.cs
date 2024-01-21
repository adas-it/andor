namespace Family.Budget.ComponentTest.Steps;

using System;
using System.Threading.Tasks;
using Family.Budget.ComponentTest.Hooks;
using Family.Budget.Api;
using FluentAssertions;
using TechTalk.SpecFlow;
using System.Linq;
using Family.Budget.ComponentTest.Utils;
using Family.Budget.Application.Dto.Configurations.Requests;
using Family.Budget.Application.Dto.Configurations.Responses;

[Binding]
public sealed class ConfigurationStepDefinitions : Hook
{
    private RegisterConfigurationInput Data;
    private ConfigurationOutput Result { get; set; }

    private const string ConfigurationsUrl = "api/v1/configurations";

    public ConfigurationStepDefinitions(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        Data = new();
        Result = new();
        factory.InitializeAsync().GetAwaiter().GetResult();
    }

    [Given("a valid configuration")]
    public void GivenAValidConfiguration()
    {
        Data = new(
           GetStringRigthSize(3, 100),
           GetStringRigthSize(3, 1000),
            GetStringRigthSize(3, 1000),
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(15));
    }

    [When("the configuration are sended to request")]
    public async Task WhenTheConfigurationAreSendedToRequestAsync()
    {
        string url = $"{ConfigurationsUrl}";

        Result = await PostWithValidations<ConfigurationOutput>(url, Data);
    }

    [Then("the id should not be null")]
    public void ThenTheIdShouldNotBeNull()
    {
        Result?.Id.Should().NotBeEmpty();
    }

    [Then(@"the event '(.*)' has to be sended on this topic '(.*)' for this consumer '(.*)'")]
    public void ThenTheEventHasToBeSendedOnThisTopic(string topicName, 
        string eventName, 
        string fileName)
    {
        message.list.Should().NotBeNull();

        var obj = message.list.Where(x => x.Name.Equals(topicName))
            .FirstOrDefault(x => x.Data.Equals(eventName));

        PactsGenericValidator.EnsureEventApiHonoursPactWithConsumer(
            microserviceName,
            eventName,
            fileName,
            obj!);
    }
}
