using Andor.Foundation.Domain.ValuesObjects;
using Bogus;

namespace Andor.TestsUtil;

public static class GeneralFixture
{
    public static Faker Faker { get; set; } = new Faker();

    public static string GetStringRightSize(int minLength, int maxLength)
    {
        var stringValue = Faker.Lorem.Random.Words(2);

        while (stringValue.Length < minLength)
        {
            stringValue += Faker.Lorem.Random.Words(2);
        }

        if (stringValue.Length > maxLength)
        {
            stringValue = stringValue[..maxLength];
        }

        return stringValue;
    }

    public static Name GetValidName()
        => GetStringRightSize(Name.MinLength, Name.MaxLength);

    public static Description GetValidDescription()
        => GetStringRightSize(Description.MinLength, Description.MaxLength);

}
