namespace Family.Budget.Application.Dto.Configurations.Requests;

public class RegisterConfigurationInput : BaseConfiguration
{
    public RegisterConfigurationInput(string name,
        string value,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset finalDate) : base(name, value, description, startDate, finalDate)
    {

    }

    public RegisterConfigurationInput() : base()
    {
    }
}
