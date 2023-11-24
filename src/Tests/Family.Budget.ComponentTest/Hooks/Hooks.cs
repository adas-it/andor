namespace Family.Budget.ComponentTest.Hooks;

using Family.Budget.Application.Dto.Models;
using Family.Budget.ComponentTest.Utils;
using Family.Budget.TestsUtil;
using Family.Budget.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using System.Security.Claims;

[Binding]
public class Hook : BaseFixture, IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> factory;
    public readonly string microserviceName;
    public TopicMessageTestHelper message;
    public string apiKey;

    public Hook(CustomWebApplicationFactory<Startup> factory)
    {
        this.factory = factory;

        this.message = factory.message;

        this.microserviceName = "Bootstrap.Api";
        this.apiKey = "836041c5-936a-4978-b3c2-f8e5b8c01445";
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        CreateWireMock();
        CreateDatabase();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        WireMockServer?.Stop();
        WireMockServer?.Dispose();
    }

    public HttpClient CreateAuthorizedClient()
    {
        return CreateAuthorizedClient(apiKey, null!);
    }
    public HttpClient CreateAuthorizedClient(string token)
    {
        return CreateAuthorizedClient(token, null!);
    }

    public HttpClient CreateAuthorizedClient(string token, string language)
    {
        HttpClient client = factory.CreateClient();

        if (!string.IsNullOrEmpty(language))
        {
            client.DefaultRequestHeaders.Add("Accept-Language", language);
        }
        else
        {
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        }

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Add("apiKey", token);
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MockJwtTokens.GenerateJwtToken(new Dictionary<string, object>()
        {
            {"Role","Fodao" }
        }));

        return client;
    }

    public Task<T> GetWithValidations<T>(string url) where T : class
    {
        return GetWithValidations<T>(url, new());
    }

    public async Task<T> GetWithValidations<T>(string url, List<ErrorModel> errorDetails) where T : class
    {
        HttpClient client;

        client = CreateAuthorizedClient();

        var response = await client.GetAsync(url);

        return await ValidateResponse<T>(errorDetails, response);
    }

    public Task<T> PostWithValidations<T>(string url, object data) where T : class
    {
        return PostWithValidations<T>(url, new(), data);
    }

    public async Task<T> PostWithValidations<T>(string url, List<ErrorModel> errorDetails, object data) where T : class
    {
        HttpClient client;

        client = CreateAuthorizedClient();

        Body(data, out StringContent httpContent);

        var response = await client.PostAsync(url, httpContent);

        return await ValidateResponse<T>(errorDetails, response);
    }

    private static void Body(object data, out StringContent httpContent)
    {
        string json = string.Empty;

        if (data != null)
        {
            json = JsonConvert.SerializeObject(data);
        }

        httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }

    private static async Task<T> ValidateResponse<T>(List<ErrorModel> errorDetails, HttpResponseMessage response) where T : class
    {
        var json =  new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() }
        };

        var content = await response.Content.ReadAsStringAsync();

        var ret = JsonConvert.DeserializeObject<DefaultResponse<T>>(content, json);

        ret.Should().NotBeNull();

        Assert.Equal(errorDetails, ret?.Errors);

        return ret?.Data!;
    }
}
