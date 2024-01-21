namespace Family.Budget.TestsUtil;

using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Models.FeatureFlag;
using Family.Budget.Infrastructure.Repositories.Context;
using AutoFixture;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public abstract class BaseFixture
{
    protected Fixture fixture;
    public Faker Faker { get; set; }
    public static WireMockServer? WireMockServer { get; set; }
    public static DbContextOptions<PrincipalContext>? DbContextOptions { get; set; }

    public BaseFixture()
    {
        fixture = new();
        Faker = new();
    }

    public string GetStringRigthSize(int minLength, int maxlength)
    {
        var stringValue = Faker.Lorem.Random.Words(2);

        while (stringValue.Length < minLength)
        {
            stringValue += Faker.Lorem.Random.Words(2);
        }

        if (stringValue.Length > maxlength)
        {
            stringValue = stringValue[..maxlength];
        }

        return stringValue;
    }

    public static void CreateWireMock()
    {
        if (WireMockServer == null || !WireMockServer.IsStarted)
        {
            WireMockServer = WireMockServer.Start(9999);
        }
    }

    public static void ResetWireMock()
    {
        WireMockServer?.Reset();
    }

    public static IRequestBuilder CreateWiremockRequest(HttpMethod method, string path)
    {
        return CreateWiremockRequest(method, path, new Dictionary<string, string>(), null!);
    }

    public static IRequestBuilder CreateWiremockRequest(HttpMethod method, string path, object body)
    {
        return CreateWiremockRequest(method, path, new Dictionary<string, string>(), body);
    }

    public static IRequestBuilder CreateWiremockRequest(HttpMethod method, string path, Dictionary<string, string> prams)
    {
        return CreateWiremockRequest(method, path, prams, null!);
    }

    public static IRequestBuilder CreateWiremockRequest(HttpMethod method, string path, Dictionary<string, string> prams, object body)
    {
        IRequestBuilder requestBuilder = Request.Create();
        requestBuilder.WithPath(path);

        foreach (KeyValuePair<string, string> keyValue in prams)
        {
            requestBuilder.WithParam(keyValue.Key, keyValue.Value);
        }

        if (body != null)
        {
            var jsonBody = JsonConvert.SerializeObject(body);

            requestBuilder.WithBody(new JsonMatcher(jsonBody, true));
        }

        return method.Method switch
        {
            "GET" => requestBuilder.UsingGet(),
            "POST" => requestBuilder.UsingPost(),
            "PUT" => requestBuilder.UsingPut(),
            "DELETE" => requestBuilder.UsingDelete(),
            _ => requestBuilder.UsingAnyMethod()
        };
    }

    public static IResponseBuilder CreateWiremockResponse(HttpStatusCode code)
    {
        IResponseBuilder responseBuilder = Response.Create();
        responseBuilder.WithStatusCode(code);
        responseBuilder.WithHeader("Content-Type", MediaTypeNames.Application.Json);

        return responseBuilder;
    }

    public static IResponseBuilder CreateWiremockResponse(HttpStatusCode code, object body)
    {
        IResponseBuilder responseBuilder = CreateWiremockResponse(code);

        responseBuilder.WithBodyAsJson(body);

        return responseBuilder;
    }

    public static void RegisterWiremockResponse(IRequestBuilder request, IResponseBuilder response)
    {
        WireMockServer?
            .Given(request)
            .RespondWith(response);
    }

    public static void MockFeatureFlag(RequestDto requestDto, string ret)
    {
        var response = new DefaultResponse<string>(ret);

        RegisterWiremockResponse(
            CreateWiremockRequest(HttpMethod.Post, "/infra/featureflag/v1/flags", requestDto),
            CreateWiremockResponse(HttpStatusCode.OK, response));
    }

    public static void CreateDatabase()
    {
        DbContextOptions = new DbContextOptionsBuilder<PrincipalContext>()
            .UseInMemoryDatabase("IntegrationTestDatabase")
            .Options;
    }
}