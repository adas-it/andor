using Andor.Api;
using Andor.Application.Dto.Common.ApplicationsErrors.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Component.Tests.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TechTalk.SpecFlow;

namespace Andor.Component.Tests.Hooks;

[Binding]
public class Hook : IClassFixture<WebApplicationFactory<Startup>>
{
    internal readonly CustomWebApplicationFactory<Startup> factory;
    protected readonly string microserviceName;

    public Hook(CustomWebApplicationFactory<Startup> factory)
    {
        this.factory = factory;

        this.microserviceName = "";
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {

    }

    public HttpClient CreateAuthorizedClient()
    {
        return CreateAuthorizedClient(string.Empty, new Dictionary<string, object>());
    }

    public HttpClient CreateAuthorizedClient(string language)
    {
        return CreateAuthorizedClient(language, new Dictionary<string, object>());
    }

    public HttpClient CreateAuthorizedClient(Dictionary<string, object> keyValuePairs)
    {
        return CreateAuthorizedClient(string.Empty, keyValuePairs);
    }

    public HttpClient CreateAuthorizedClient(string language, Dictionary<string, object> keyValuePairs)
    {
        HttpClient client = null;

        client = factory.CreateClient();

        if (!string.IsNullOrEmpty(language))
        {
            client.DefaultRequestHeaders.Add("Accept-Language", language);
        }
        else
        {
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MockJwtTokens.GenerateJwtToken(keyValuePairs));

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
        return PostWithValidations<T>(url,
            new(),
            data,
            new Dictionary<string, object>());
    }

    public Task<T> PostWithValidations<T>(string url,
        object data,
        Dictionary<string, object> keyValuePairs) where T : class
    {
        return PostWithValidations<T>(url,
            new(),
            data,
            keyValuePairs);
    }

    public async Task<T> PostWithValidations<T>(string url,
        List<ErrorModel> errorDetails,
        object data,
        Dictionary<string, object> keyValuePairs) where T : class
    {
        HttpClient client;

        client = CreateAuthorizedClient(keyValuePairs);

        Body(data, out StringContent httpContent);

        HttpResponseMessage response = null;

        try
        {
            response = await client.PostAsync(url, httpContent);
        }
        catch (Exception ex)
        {
        }


        return await ValidateResponse<T>(errorDetails, response);
    }

    internal static void Body(object data, out StringContent httpContent)
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
        var json = new JsonSerializerSettings()
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
