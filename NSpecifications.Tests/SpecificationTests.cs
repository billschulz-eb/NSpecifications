using FluentAssertions;
using NSpecifications.Tests.Entities;
using NUnit.Framework;

namespace NSpecifications.Tests;

[TestFixture]
public sealed class SpecificationTests
{
    [Test]
    public void WhiskeyAndCold()
    {
        // Arrange
        var coldWhiskey = Drink.ColdWhiskey();
        var appleJuice = Drink.AppleJuice();
        var whiskeySpec = new Spec<Drink>(d => d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase),      new Error(0, "not whiskey"));
        var coldSpec = new Spec<Drink>(d => d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), new Error(0, "not cold"));

        // Act
        var coldWhiskeySpec = whiskeySpec.And(coldSpec);

        // Assert
        coldWhiskeySpec.IsSatisfiedBy(coldWhiskey)
                       .IsSuccess.Should()
                       .BeTrue();

        coldWhiskeySpec.IsSatisfiedBy(appleJuice)
                       .IsSuccess.Should()
                       .BeFalse();
    }

    [Test]
    public void AppleOrOrangeJuice()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();
        var juiceSpec = new Spec<Drink>(d => d.Name.Contains("juice",   StringComparison.OrdinalIgnoreCase), new Error(0, "not juice"));
        var appleSpec = new Spec<Drink>(d => d.Name.Contains("apple",   StringComparison.OrdinalIgnoreCase), new Error(0, "not apple"));
        var orangeSpec = new Spec<Drink>(d => d.Name.Contains("orange", StringComparison.OrdinalIgnoreCase), new Error(0, "not orange"));

        // Act
        var appleOrOrangeJuiceSpec = juiceSpec.And(appleSpec.Or(orangeSpec));

        // Assert
        appleOrOrangeJuiceSpec.IsSatisfiedBy(appleJuice)
                              .IsSuccess.Should()
                              .BeTrue();

        appleOrOrangeJuiceSpec.IsSatisfiedBy(orangeJuice)
                              .IsSuccess.Should()
                              .BeTrue();

        var result = appleOrOrangeJuiceSpec.IsSatisfiedBy(blackberryJuice);

        result.IsSuccess.Should()
              .BeFalse();

        result.Error.Should()
              .NotBeNull();
    }
}
