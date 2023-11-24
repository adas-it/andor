namespace Family.Budget.Application.Models.Config;
public class PollyConfigs
{
    public string Repetitions { get; set; }
    public string TimeCircuitBreak { get; set; }
    public string TimeOut { get; set; }

    public const string PollyConfig = "Polly";

    public PollyConfigs()
    {
        Repetitions = String.Empty;
        TimeCircuitBreak = String.Empty;
        TimeOut = String.Empty;
    }

    public PollyConfigs(string repetitions, string timeCircuitBreak, string circuitBreakRepetitions)
    {
        Repetitions = repetitions;
        TimeCircuitBreak = timeCircuitBreak.ToString();
        TimeOut = circuitBreakRepetitions;
    }
}
