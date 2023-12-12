using CSharpFunctionalExtensions;
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
        var whiskeySpec = new Spec<Drink>(d => Result.SuccessIf(d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase), "It is not whiskey"));
        var coldSpec = new Spec<Drink>(d => Result.SuccessIf(d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), "there is no ice"));

        // Act
        var coldWhiskeySpec = whiskeySpec.And(coldSpec);

        // Assert
        coldWhiskeySpec.IsSatisfiedBy(coldWhiskey).Should().BeTrue();
        coldWhiskeySpec.IsSatisfiedBy(appleJuice).Should().BeFalse();
    }

    [Test]
    public void AppleOrOrangeJuice()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();
        var juiceSpec = new Spec<Drink>(d => Result.SuccessIf(d.Name.Contains("juice",   StringComparison.OrdinalIgnoreCase), "not a juice"));
        var appleSpec = new Spec<Drink>(d => Result.SuccessIf(d.Name.Contains("apple",   StringComparison.OrdinalIgnoreCase), "not an apple drink"));
        var orangeSpec = new Spec<Drink>(d => Result.SuccessIf(d.Name.Contains("orange", StringComparison.OrdinalIgnoreCase), "not an orange drink"));

        // Act
        var appleOrOrangeJuiceSpec = juiceSpec.And(appleSpec.Or(orangeSpec));

        // Assert
        appleOrOrangeJuiceSpec.IsSatisfiedBy(appleJuice).Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(orangeJuice).Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(blackberryJuice).Should().BeFalse();
    }
}
