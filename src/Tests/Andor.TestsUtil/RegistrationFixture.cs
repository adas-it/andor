using Andor.Application.Dto.Onboarding.Registrations.Requests;

namespace Andor.TestsUtil;

public static class RegistrationFixture
{
    public static UserRegistrationInput GetUserRegistrationInput()
    {
        return new UserRegistrationInput(
            GeneralFixture.Faker.Person.FirstName,
            GeneralFixture.Faker.Person.LastName,
            GeneralFixture.Faker.Person.Email
            );
    }
}
