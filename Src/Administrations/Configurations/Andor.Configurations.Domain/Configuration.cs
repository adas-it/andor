using Andor.Configurations.Domain.Errors;
using Andor.Configurations.Domain.Events;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Domain;

/// <summary>
/// Represents a configuration aggregate root entity that manages system configuration settings.
/// </summary>
public class Configuration : AggregateRoot<ConfigurationId>, ISoftDeletableEntity
{
    /// <summary>
    /// Gets the name of the configuration.
    /// </summary>
    public Name Name { get; private set; }

    /// <summary>
    /// Gets the value of the configuration.
    /// </summary>
    public Value Value { get; private set; }

    /// <summary>
    /// Gets the description of the configuration.
    /// </summary>
    public Description Description { get; private set; }

    /// <summary>
    /// Gets the date and time when the configuration becomes active.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the date and time when the configuration expires. Null indicates no expiration.
    /// </summary>
    public DateTime? ExpireDate { get; private set; }

    /// <summary>
    /// Gets the type of the configuration.
    /// </summary>
    public ConfigurationType Type { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the configuration has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Gets the date and time when the configuration was deleted. Null if not deleted.
    /// </summary>
    public DateTime? DeletedDate { get; private set; }

    /// <summary>
    /// Gets the current state of the configuration based on deletion status, start date, and expiration date.
    /// </summary>
    public ConfigurationState State => CalculateState(IsDeleted, StartDate, ExpireDate);

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// Default parameterless constructor for ORM usage.
    /// </summary>
    public Configuration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class with specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the configuration.</param>
    /// <param name="name">The name of the configuration.</param>
    /// <param name="value">The value of the configuration.</param>
    /// <param name="description">The description of the configuration.</param>
    /// <param name="startDate">The date when the configuration becomes active.</param>
    /// <param name="expireDate">The optional date when the configuration expires.</param>
    /// <param name="type">The type of the configuration.</param>
    private Configuration(
            ConfigurationId id,
            Name name,
            Value value,
            Description description,
            DateTime startDate,
            DateTime? expireDate,
            ConfigurationType type)
    {
        Id = id;
        Name = name.ToString().ToLower();
        Value = value;
        Description = description;
        StartDate = startDate;
        ExpireDate = expireDate;
        Type = type;
    }

    /// <summary>
    /// Creates a new configuration instance asynchronously with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the configuration.</param>
    /// <param name="name">The name of the configuration.</param>
    /// <param name="value">The value of the configuration.</param>
    /// <param name="description">The description of the configuration.</param>
    /// <param name="startDate">The date when the configuration becomes active.</param>
    /// <param name="expireDate">The optional date when the configuration expires.</param>
    /// <param name="type">The type of the configuration.</param>
    /// <param name="skipValidations">If true, skips validation rules.</param>
    /// <param name="userId">The identifier of the user creating the configuration.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A tuple containing the domain result and the created configuration if successful.</returns>
    public static async Task<(DomainResult, Configuration?)> NewAsync(
        ConfigurationId id,
        Name name,
        Value value,
        Description description,
        DateTime startDate,
        DateTime? expireDate,
        ConfigurationType type,
        bool skipValidations,
        Guid userId,
        IConfigurationValidator validator,
        CancellationToken cancellationToken)
    {

        DomainResult result;

        var entity = new Configuration(
            id,
            name,
            value,
            description,
            startDate,
            expireDate,
            type);

        if (skipValidations)
        {
            result = DomainResult.Success(infos: new List<Notification>()
                {
                    new Notification("","All Validations has been skipped.", ConfigurationsErrorCodes.SkippedValidations)
                });
        }
        else
        {
            result = await entity.ValidateAsync(validator, cancellationToken);
        }

        if (result.IsSuccess && entity != null)
        {
            entity.RaiseDomainEvent(ConfigurationCreated.FromConfiguration(entity, userId));
        }

        return (result, entity);
    }

    /// <summary>
    /// Calculates the current state of a configuration based on its deletion status and date range.
    /// </summary>
    /// <param name="isDeleted">Indicates whether the configuration is deleted.</param>
    /// <param name="startDate">The date when the configuration becomes active.</param>
    /// <param name="expireDate">The optional date when the configuration expires.</param>
    /// <returns>The calculated <see cref="ConfigurationState"/>.</returns>
    public static ConfigurationState CalculateState(bool isDeleted, DateTime startDate, DateTime? expireDate)
    {
        if (isDeleted)
            return ConfigurationState.Deleted;
        if (startDate > DateTime.UtcNow)
            return ConfigurationState.Awaiting;
        if (startDate < DateTime.UtcNow && (!expireDate.HasValue || expireDate.Value > DateTime.UtcNow))
            return ConfigurationState.Active;
        if (expireDate.HasValue && expireDate.Value < DateTime.UtcNow)
            return ConfigurationState.Expired;

        return ConfigurationState.Undefined;
    }

    #region Update

    /// <summary>
    /// Updates the configuration asynchronously with new values.
    /// </summary>
    /// <param name="newValue">The new value for the configuration.</param>
    /// <param name="newDescription">The new description for the configuration.</param>
    /// <param name="newStartDate">The new start date for the configuration.</param>
    /// <param name="newExpireDate">The new optional expiration date for the configuration.</param>
    /// <param name="userId">The identifier of the user performing the update.</param>
    /// <param name="configurationValidator">The validator to use for validation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="DomainResult"/> indicating success or failure.</returns>
    public async Task<DomainResult> UpdateAsync(Value newValue, Description newDescription,
        DateTime newStartDate, DateTime? newExpireDate, Guid userId, IConfigurationValidator configurationValidator,
        CancellationToken cancellationToken)
    {
        var notifications = await configurationValidator.ValidateUpdateAsync(this, newValue, newDescription,
            newStartDate, newExpireDate, cancellationToken);

        AddNotification(notifications);

        var result = Validate();

        if (result.IsFailure)
        {
            return result;
        }

        Value = newValue;
        Description = newDescription;
        StartDate = newStartDate;
        ExpireDate = newExpireDate;

        RaiseDomainEvent(ConfigurationUpdated.FromConfiguration(this, userId));

        return result;
    }

    #endregion

    /// <summary>
    /// Deactivates the configuration by setting its expiration date to the current date and time.
    /// Only active configurations can be deactivated.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the deactivation.</param>
    /// <param name="configurationValidator">The validator to use for validation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="DomainResult"/> indicating success or failure.</returns>
    public async Task<DomainResult> DeactivateAsync(Guid userId, IConfigurationValidator configurationValidator,
        CancellationToken cancellationToken)
    {
        if (State == ConfigurationState.Deleted)
        {
            AddNotification(nameof(ExpireDate),
                $"Cannot deactivate a {State} configuration.",
                ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedDeleted);
        }

        if (State == ConfigurationState.Expired)
        {
            AddNotification(nameof(State),
                $"Cannot deactivate a configuration in {State} state.",
                ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedExpired);
            return Validate();
        }

        if (State == ConfigurationState.Awaiting)
        {
            AddNotification(nameof(ExpireDate),
                $"Cannot deactivate a configuration in {State} state.",
                ConfigurationsErrorCodes.ErrorOnDeactivationConfigurationNotAllowedAwaiting);
            return Validate();
        }

        if (State == ConfigurationState.Active)
        {
            await UpdateAsync(Value, Description, StartDate, DateTime.UtcNow, userId, configurationValidator,
                cancellationToken);

            AddInformation(nameof(ExpireDate),
                "expire date set to now",
                ConfigurationsErrorCodes.SetExpireDateToToday);

            RaiseDomainEvent(ConfigurationDeactivated.FromConfiguration(this, userId));
        }

        return Validate();
    }

    /// <summary>
    /// Soft deletes the configuration. Only awaiting configurations can be deleted.
    /// Active configurations must be deactivated first, and expired configurations cannot be deleted.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the deletion.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="DomainResult"/> indicating success or failure.</returns>
    public Task<DomainResult> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (State == ConfigurationState.Deleted)
        {
            AddInformation(nameof(ExpireDate),
                "this configuration is already deleted.",
                ConfigurationsErrorCodes.ErrorOnDeleteConfiguration);
        }

        if (State == ConfigurationState.Expired)
        {
            AddWarning(nameof(ExpireDate),
                "not allowed to delete expired configurations.",
                ConfigurationsErrorCodes.ErrorOnDeleteConfigurationNotAllowedDeleteExpired);
        }

        if (State == ConfigurationState.Active)
        {
            AddNotification(nameof(ExpireDate),
                "not allowed to delete active configurations, try Deactivating.",
                ConfigurationsErrorCodes.ErrorOnDeleteConfigurationNotAllowedDeleteActive);
        }

        if (State == ConfigurationState.Awaiting)
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;

            RaiseDomainEvent(ConfigurationDeleted.FromConfiguration(this, userId));
        }

        return Task.FromResult(Validate());
    }
}
