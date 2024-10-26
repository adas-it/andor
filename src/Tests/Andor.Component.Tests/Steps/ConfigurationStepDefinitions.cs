using Andor.Api;
using Andor.Application.Dto.Administrations.Configurations.Requests;
using Andor.Application.Dto.Administrations.Configurations.Responses;
using Andor.Component.Tests.Hooks;
using Andor.TestsUtil;
using FluentAssertions;
using Mapster;
using TechTalk.SpecFlow;

namespace Andor.Component.Tests.Steps;

internal sealed class ConfigurationStepDefinitions : Hook
{
    private BaseConfiguration Data;
    private ConfigurationOutput Result { get; set; }

    private const string ConfigurationsUrl = "v1/configurations";

    public ConfigurationStepDefinitions(CustomWebApplicationFactory<Startup> factory) : base(factory)

    {
        factory.InitializeAsync().GetAwaiter().GetResult();
    }

    [Given("a valid configuration")]
    public void GivenAValidConfiguration()
    {
        Data = ConfigurationFixture.GetValidConfiguration().Adapt<BaseConfiguration>();
    }

    [When("the configuration are sended to request")]
    public async Task WhenTheConfigurationAreSendedToRequestAsync()
    {
        string url = $"{ConfigurationsUrl}";

        Result = await PostWithValidations<ConfigurationOutput>(url,
            Data,
            new Dictionary<string, object>()
            {
                {
                    "realm_access", "{ 'roles': ['admin']}"
                }
            });
    }

    [Then("the id should not be null")]
    public void ThenTheIdShouldNotBeNull()
    {
        Result?.Id.Should().NotBeEmpty();
    }
}
