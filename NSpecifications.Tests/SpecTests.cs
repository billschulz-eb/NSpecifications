using FluentAssertions;
using NSpecifications.Tests.Entities;
using NUnit.Framework;

namespace NSpecifications.Tests;

[TestFixture]
public sealed class SpecTests
{
    [Test]
    public void WhiskeyAndCold()
    {
        // Arrange
        var coldWhiskey = Drink.ColdWhiskey();
        var appleJuice = Drink.AppleJuice();
        var whiskeySpec = new Spec<Drink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase), "not whiskey"));
        var coldSpec = new Spec<Drink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), "not a cold drink"));

        // Act
        var coldWhiskeySpec = whiskeySpec & coldSpec;

        // Assert
        coldWhiskeySpec.IsSatisfiedBy(coldWhiskey).Should().BeTrue();
        coldWhiskeySpec.IsSatisfiedBy(appleJuice).Should().BeFalse();
        // And
        coldWhiskey.Is(coldWhiskeySpec).Should().BeTrue();
        appleJuice.Is(coldWhiskeySpec).Should().BeFalse();
    }

    [Test]
    public void AppleOrOrangeJuice()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();
        var juiceSpec = new Spec<Drink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.Name.Contains("juice",   StringComparison.OrdinalIgnoreCase), "not juice"));
        var appleSpec = new Spec<Drink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.Name.Contains("apple",   StringComparison.OrdinalIgnoreCase), "not apple"));
        var orangeSpec = new Spec<Drink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.Name.Contains("orange", StringComparison.OrdinalIgnoreCase), "not orange"));

        // Act
        var appleOrOrangeJuiceSpec = juiceSpec & (appleSpec | orangeSpec);

        // Assert
        appleOrOrangeJuiceSpec.IsSatisfiedBy(appleJuice).Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(orangeJuice).Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(blackberryJuice).Should().BeFalse();
        // And
        new[] { appleJuice, orangeJuice }.Are(appleOrOrangeJuiceSpec).Should().BeTrue();
        blackberryJuice.Is(appleOrOrangeJuiceSpec).Should().BeFalse();
    }

    [Test]
    public void CastUp()
    {
        // Arrange
        var coldWhiskey = Drink.ColdWhiskey();
        var appleJuice = Drink.AppleJuice();
        var whiskeySpec = new Spec<IDrink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase), "not whiskey"));
        var coldSpec = new Spec<IDrink>(d => CSharpFunctionalExtensions.Result.SuccessIf(d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), "no cold drinks"));

        // Act
        var coldWhiskeySpec = whiskeySpec & coldSpec;

        //Assert
        new[] { coldWhiskey, appleJuice }.Where(coldWhiskeySpec.CastUp<Drink>()).Should().NotContain(appleJuice);
    }

    [Test]
    public void Any()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();

        // Assert
        new[] { blackberryJuice, appleJuice, orangeJuice }.Are(Spec.Any<Drink>()).Should().BeTrue();
    }

    [Test]
    public void None()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();

        // Assert
        new[] { blackberryJuice, appleJuice, orangeJuice }.Are(Spec.None<Drink>()).Should().BeFalse();
    }
}
