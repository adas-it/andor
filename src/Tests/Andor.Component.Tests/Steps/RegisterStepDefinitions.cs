using Andor.Api;
using Andor.Application.Dto.Onboarding.Registrations.Requests;
using Andor.Component.Tests.Hooks;
using Andor.Component.Tests.Utils;
using Andor.TestsUtil;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Andor.Component.Tests.Steps;

internal sealed class RegisterStepDefinitions : Hook
{
    private UserRegistrationInput Data;

    private string code;

    private const string base_url = "v1/registration";

    public RegisterStepDefinitions(CustomWebApplicationFactory<Startup> factory) : base(factory)

    {
        factory.InitializeAsync().GetAwaiter().GetResult();
    }

    [Given("a valid registration")]
    public void GivenAValidRegistration()
    {
        Data = RegistrationFixture.GetUserRegistrationInput();
    }

    [When("we send a request to create account")]
    public async Task WhenWeSendRequestCreateAccount()
    {
        string url = $"{base_url}";

        HttpClient client = null;
        client = factory.CreateClient();

        Body(Data, out StringContent httpContent);

        HttpResponseMessage response = null;

        response = await client.PostAsync(url, httpContent);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then("we should receive a e-mail with the code")]
    public void ThenWeShouldReceiveMailWithCode()
    {
        var times = 0;
        List<Mail> test2 = [];
        do
        {
            test2 = SMTP_Test.Emails()
                .Where(x => x.recipientMail == Data.Email && x.Subject == "confirmation_email")
                .ToList();

            Thread.Sleep(500);
            times++;

        } while (times <= 100 && test2.Count == 0);

        test2.Count.Should().BeGreaterThan(0);
        var body = test2.FirstOrDefault().body;

        var lengh = body.Length;

        code = body.Substring(lengh - 4, 4);

    }

    [When("we send a request to complete creation")]
    public async Task WhenWeSendRequestCompleteCreation()
    {
        string url = $"{base_url}/complete";

        HttpClient client = null;
        client = factory.CreateClient();

        var complete = new CompleteRegistrationInput(Data.Email,
            Data.FirstName,
            Data.LastName,
            Data.Email,
            "en",
            true,
            true,
            "hexa",
            code);

        Body(complete, out StringContent httpContent);

        HttpResponseMessage response = null;

        response = await client.PostAsync(url, httpContent);

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
