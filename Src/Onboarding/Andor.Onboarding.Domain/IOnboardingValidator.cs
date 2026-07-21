using Andor.Foundation.Domain.Validation;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Domain;

public interface IOnboardingValidator : IDefaultValidator<SignupRequest, SignupRequestId>
{
}
