namespace Family.Budget.ComponentTest.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Verifier;
using System;
using System.IO;

public class PactsGenericValidator : IDisposable
{
    public PactsGenericValidator()
    {
    }

    public void Dispose()
    {
        // make sure you dispose the verifier to stop the internal messaging server
        GC.SuppressFinalize(this);
    }

    public static void EnsureEventApiHonoursPactWithConsumer(string microServiceName, string eventName, string fileName, object eventSended)
    {
        var verifier = new PactVerifier();

        string pactPath = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "Family.Budget.ComponentTest",
                                       "pacts",
                                       fileName);

        var defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        verifier
            .MessagingProvider(microServiceName, defaultSettings)
            .WithProviderMessages(scenarios =>
            {
                scenarios.Add(eventName, builder =>
                {
                    builder
                    .WithContent(() => new[]
                        {
                        eventSended
                    });
                });
            })
            .WithFileSource(new FileInfo(pactPath))
            .Verify();

        verifier.Dispose();
    }
}
