using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.Tests.ValuesObjects;

public class NameTests
{
    #region Valid Name Tests

    [Theory]
    [InlineData("John")]
    [InlineData("John Doe")]
    [InlineData("ABC")]
    [InlineData("A B C")]
    [InlineData("ValidNameWithExactly70CharactersIncludingSpacesAndOtherCharacters123")]
    public void Name_WithValidValue_ShouldCreateSuccessfully(string validName)
    {
        // Act
        Name name = validName;

        // Assert
        Assert.NotNull(name);
        Assert.Equal(validName.Trim(), name.Value);
    }

    [Fact]
    public void Name_WithMinimumLength_ShouldCreateSuccessfully()
    {
        // Arrange
        var validName = "ABC"; // Exactly 3 characters

        // Act
        Name name = validName;

        // Assert
        Assert.NotNull(name);
        Assert.Equal("ABC", name.Value);
    }

    [Fact]
    public void Name_WithMaximumLength_ShouldCreateSuccessfully()
    {
        // Arrange
        var validName = new string('A', 70); // Exactly 70 characters

        // Act
        Name name = validName;

        // Assert
        Assert.NotNull(name);
        Assert.Equal(validName, name.Value);
    }

    [Fact]
    public void Name_WithLeadingAndTrailingSpaces_ShouldTrimSpaces()
    {
        // Arrange
        var nameWithSpaces = "   John Doe   ";

        // Act
        Name name = nameWithSpaces;

        // Assert
        Assert.Equal("John Doe", name.Value);
    }

    #endregion

    #region Invalid Name Tests - Too Short

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("  ")]
    [InlineData(" A ")]
    public void Name_WithTooShortValue_ShouldThrowArgumentOutOfRangeException(string shortName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Name name = shortName;
        });

        Assert.Contains("must be between 3 and 70 characters", exception.Message);
    }

    [Fact]
    public void Name_WithTwoCharacters_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var shortName = "AB";

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Name name = shortName;
        });

        Assert.Contains("Name must be between 3 and 70 characters", exception.Message);
    }

    #endregion

    #region Invalid Name Tests - Too Long

    [Fact]
    public void Name_WithTooLongValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var longName = new string('A', 71); // 71 characters

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Name name = longName;
        });

        Assert.Contains("must be between 3 and 70 characters", exception.Message);
    }

    [Fact]
    public void Name_With100Characters_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var longName = new string('A', 100);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Name name = longName;
        });

        Assert.Contains("Name must be between 3 and 70 characters", exception.Message);
    }

    #endregion

    #region Null Handling Tests

    [Fact]
    public void Name_WithNullValue_ShouldCreateWithNullValue()
    {
        // Arrange
        string? nullName = null;

        // Act
        Name name = nullName!;

        // Assert
        Assert.NotNull(name);
        Assert.Null(name.Value);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidValue_ShouldCreateSuccessfully()
    {
        // Act
        var name = new Name("John Doe");

        // Assert
        Assert.NotNull(name);
        Assert.Equal("John Doe", name.Value);
    }

    [Fact]
    public void Constructor_WithMinimumLength_ShouldCreateSuccessfully()
    {
        // Act
        var name = new Name("ABC");

        // Assert
        Assert.Equal("ABC", name.Value);
    }

    [Fact]
    public void Constructor_WithMaximumLength_ShouldCreateSuccessfully()
    {
        // Arrange
        var validName = new string('A', 70);

        // Act
        var name = new Name(validName);

        // Assert
        Assert.Equal(validName, name.Value);
    }

    [Fact]
    public void Constructor_WithTooShortValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Name("AB"));
        Assert.Contains("must be between 3 and 70 characters", exception.Message);
    }

    [Fact]
    public void Constructor_WithTooLongValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var longName = new string('A', 71);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Name(longName));
        Assert.Contains("must be between 3 and 70 characters", exception.Message);
    }

    [Fact]
    public void Constructor_WithLeadingAndTrailingSpaces_ShouldTrimSpaces()
    {
        // Act
        var name = new Name("   John Doe   ");

        // Assert
        Assert.Equal("John Doe", name.Value);
    }

    [Fact]
    public void Constructor_WithNullValue_ShouldCreateWithNullValue()
    {
        // Act
        var name = new Name(null!);

        // Assert
        Assert.NotNull(name);
        Assert.Null(name.Value);
    }

    #endregion

    #region Implicit Conversion Tests

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        string value = "John Doe";

        // Act
        Name name = value;

        // Assert
        Assert.Equal("John Doe", name.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        Name name = "John Doe";

        // Act
        string? result = name;

        // Assert
        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void ImplicitConversion_NullNameToString_ShouldReturnNull()
    {
        // Arrange
        Name? name = null;

        // Act
        string? result = name;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithValidName_ShouldReturnValue()
    {
        // Arrange
        Name name = "John Doe";

        // Act
        var result = name.ToString();

        // Assert
        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void ToString_WithNullValue_ShouldReturnEmptyString()
    {
        // Arrange
        string? nullValue = null;
        Name name = nullValue!;

        // Act
        var result = name.ToString();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    #endregion

    #region Equality Tests (Record behavior with Case-Insensitive comparison)

    [Fact]
    public void Name_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        Name name1 = "John Doe";
        Name name2 = "John Doe";

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.True(name1 == name2);
    }

    [Fact]
    public void Name_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        Name name1 = "John Doe";
        Name name2 = "Jane Doe";

        // Act & Assert
        Assert.NotEqual(name1, name2);
        Assert.True(name1 != name2);
    }

    [Fact]
    public void Name_WithSameValueDifferentSpaces_ShouldBeEqual()
    {
        // Arrange
        Name name1 = "  John Doe  ";
        Name name2 = "John Doe";

        // Act & Assert
        Assert.Equal(name1, name2);
    }

    [Theory]
    [InlineData("John Doe", "john doe")]
    [InlineData("JOHN DOE", "john doe")]
    [InlineData("JoHn DoE", "john doe")]
    [InlineData("Mary Smith", "MARY SMITH")]
    public void Name_WithDifferentCase_ShouldBeEqual(string name1Value, string name2Value)
    {
        // Arrange
        Name name1 = name1Value;
        Name name2 = name2Value;

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.True(name1 == name2);
        Assert.True(name1.Equals(name2));
    }

    [Fact]
    public void Name_CaseInsensitiveComparison_ShouldHaveSameHashCode()
    {
        // Arrange
        Name name1 = "John Doe";
        Name name2 = "john doe";

        // Act
        var hash1 = name1.GetHashCode();
        var hash2 = name2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Name_WithNull_ComparingToNonNull_ShouldNotBeEqual()
    {
        // Arrange
        string? nullValue = null;
        Name? nullName = nullValue!;
        Name validName = "John Doe";

        // Act & Assert
        Assert.NotEqual(nullName, validName);
        Assert.False(nullName.Equals(validName));
    }

    [Fact]
    public void Name_CompareNullToNull_ShouldBeEqual()
    {
        // Arrange
        string? nullValue1 = null;
        string? nullValue2 = null;
        Name? name1 = nullValue1!;
        Name? name2 = nullValue2!;

        // Act & Assert
        Assert.Equal(name1, name2);
    }

    #endregion

    #region HashCode Tests

    [Fact]
    public void GetHashCode_WithSameValueSameCase_ShouldReturnSameHash()
    {
        // Arrange
        Name name1 = "John Doe";
        Name name2 = "John Doe";

        // Act & Assert
        Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
    }

    [Theory]
    [InlineData("John Doe", "john doe")]
    [InlineData("MARY", "mary")]
    [InlineData("Test Name", "TEST NAME")]
    public void GetHashCode_WithSameValueDifferentCase_ShouldReturnSameHash(string value1, string value2)
    {
        // Arrange
        Name name1 = value1;
        Name name2 = value2;

        // Act
        var hash1 = name1.GetHashCode();
        var hash2 = name2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashes()
    {
        // Arrange
        Name name1 = "John Doe";
        Name name2 = "Jane Doe";

        // Act & Assert
        Assert.NotEqual(name1.GetHashCode(), name2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithNullValue_ShouldReturnZero()
    {
        // Arrange
        string? nullValue = null;
        Name name = nullValue!;

        // Act
        var hash = name.GetHashCode();

        // Assert
        Assert.Equal(0, hash);
    }

    [Fact]
    public void GetHashCode_ConsistentWithEquals()
    {
        // Arrange
        Name name1 = "Test Name";
        Name name2 = "test name";

        // Act & Assert
        // If two objects are equal, their hash codes must be equal
        if (name1.Equals(name2))
        {
            Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
        }
    }

    #endregion

    #region Constants Tests

    [Fact]
    public void MinLength_ShouldBe3()
    {
        // Assert
        Assert.Equal(3, Name.MinLength);
    }

    [Fact]
    public void MaxLength_ShouldBe70()
    {
        // Assert
        Assert.Equal(70, Name.MaxLength);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Name_WithSpecialCharacters_ShouldCreateSuccessfully()
    {
        // Arrange
        var nameWithSpecialChars = "José María O'Brien-Smith";

        // Act
        Name name = nameWithSpecialChars;

        // Assert
        Assert.Equal(nameWithSpecialChars, name.Value);
    }

    [Fact]
    public void Name_WithNumbers_ShouldCreateSuccessfully()
    {
        // Arrange
        var nameWithNumbers = "John123";

        // Act
        Name name = nameWithNumbers;

        // Assert
        Assert.Equal(nameWithNumbers, name.Value);
    }

    [Fact]
    public void Name_WithUnicodeCharacters_ShouldCreateSuccessfully()
    {
        // Arrange
        var nameWithUnicode = "João 你好";

        // Act
        Name name = nameWithUnicode;

        // Assert
        Assert.Equal(nameWithUnicode, name.Value);
    }

    [Theory]
    [InlineData("   ABC   ", "ABC")]
    [InlineData("\t\tJohn\t\t", "John")]
    [InlineData("  Test Name  ", "Test Name")]
    public void Name_WithVariousWhitespace_ShouldTrimCorrectly(string input, string expected)
    {
        // Act
        Name name = input;

        // Assert
        Assert.Equal(expected, name.Value);
    }

    #endregion

    #region Collection Tests (HashSet/Dictionary)

    [Fact]
    public void Name_InHashSet_CaseInsensitiveDuplicatesShouldNotBeAdded()
    {
        // Arrange
        var hashSet = new HashSet<Name>();
        Name name1 = "John Doe";
        Name name2 = "john doe";
        Name name3 = "JOHN DOE";

        // Act
        hashSet.Add(name1);
        hashSet.Add(name2);
        hashSet.Add(name3);

        // Assert
        Assert.Single(hashSet);
    }

    [Fact]
    public void Name_AsDictionaryKey_CaseInsensitiveKeysShouldBeConsideredSame()
    {
        // Arrange
        var dictionary = new Dictionary<Name, string>();
        Name key1 = "John Doe";
        Name key2 = "john doe";

        // Act
        dictionary[key1] = "First";
        dictionary[key2] = "Second";

        // Assert
        Assert.Single(dictionary);
        Assert.Equal("Second", dictionary[key1]);
    }

    [Fact]
    public void Name_InHashSet_DifferentValuesShouldBeAdded()
    {
        // Arrange
        var hashSet = new HashSet<Name>();
        Name name1 = "John Doe";
        Name name2 = "Jane Doe";

        // Act
        hashSet.Add(name1);
        hashSet.Add(name2);

        // Assert
        Assert.Equal(2, hashSet.Count);
    }

    [Fact]
    public void Name_InList_Contains_ShouldWorkWithCaseInsensitivity()
    {
        // Arrange
        var list = new List<Name> { "John Doe", "Jane Smith" };

        // Act & Assert
        Assert.Contains(list, n => n == (Name)"john doe");
        Assert.Contains(list, n => n == (Name)"JANE SMITH");
    }

    #endregion
}
