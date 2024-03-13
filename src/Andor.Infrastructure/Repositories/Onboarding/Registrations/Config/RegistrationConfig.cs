using Andor.Domain.Entities.Onboarding.Registrations;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.Mail;

namespace Andor.Infrastructure.Repositories.Configurations.Config;

public record RegistrationConfig : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> entity)
    {
        entity.ToTable(nameof(Registration), SchemasNames.Onboarding);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.FirstName).HasMaxLength(70);
        entity.Property(k => k.LastName).HasMaxLength(70);

        entity.Ignore(k => k.Events);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => RegistrationId.Load(value)
        );

        entity.Property(k => k.CheckCode)
        .HasConversion(
            CheckCode => CheckCode!.Value,
            value => CheckCode.Load(value)
        );

        entity.Property(k => k.Email)
        .HasConversion(
            Email => Email!.Address,
            value => new MailAddress(value)
        );

        entity.Property(k => k.State)
        .HasConversion(
            State => State,
            value => RegistrationState.GetByKey(value.Key)
        );
    }
}
