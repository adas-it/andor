using System.Diagnostics.Metrics;
using System.Reflection;

namespace Family.Budget.Api.Common.Metrics
{
    public class CustomMetrics
    {
        public static readonly Meter Default = new("Andor",
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

        public static readonly Counter<long> Pings =
        Default.CreateCounter<long>("thorstenhans_serviceb_pings",
            description: "Total number of pings");
    }
}
