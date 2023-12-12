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
        var whiskeySpec = new Spec<Drink>(d => d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase),      new Error(0, "not whiskey"));
        var coldSpec = new Spec<Drink>(d => d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), new Error(0, "not cold"));

        // Act
        var coldWhiskeySpec = whiskeySpec & coldSpec;

        // Assert
        coldWhiskeySpec.IsSatisfiedBy(coldWhiskey).IsSuccess.Should().BeTrue();
        coldWhiskeySpec.IsSatisfiedBy(appleJuice).IsSuccess.Should().BeFalse();
        // And
        coldWhiskey.Is(coldWhiskeySpec).IsSuccess.Should().BeTrue();
        appleJuice.Is(coldWhiskeySpec).IsSuccess.Should().BeFalse();
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
        var appleOrOrangeJuiceSpec = juiceSpec & (appleSpec | orangeSpec);

        // Assert
        appleOrOrangeJuiceSpec.IsSatisfiedBy(appleJuice).IsSuccess.Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(orangeJuice).IsSuccess.Should().BeTrue();
        appleOrOrangeJuiceSpec.IsSatisfiedBy(blackberryJuice).IsSuccess.Should().BeFalse();
        // And
        new[] { appleJuice, orangeJuice }.Are(appleOrOrangeJuiceSpec).IsSuccess.Should().BeTrue();
        blackberryJuice.Is(appleOrOrangeJuiceSpec).IsSuccess.Should().BeFalse();
    }

    [Test]
    public void CastUp()
    {
        // Arrange
        var coldWhiskey = Drink.ColdWhiskey();
        var appleJuice = Drink.AppleJuice();
        var whiskeySpec = new Spec<IDrink>(d => d.Name.Equals("whiskey", StringComparison.OrdinalIgnoreCase),      new Error(0, "not whiskey"));
        var coldSpec = new Spec<IDrink>(d => d.With.Any(w => w.Equals("ice", StringComparison.OrdinalIgnoreCase)), new Error(0, "not cold"));

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
        new[] { blackberryJuice, appleJuice, orangeJuice }.Are(Spec.Any<Drink>()).IsSuccess.Should().BeTrue();
    }

    [Test]
    public void None()
    {
        // Arrange
        var blackberryJuice = Drink.BlackberryJuice();
        var appleJuice = Drink.AppleJuice();
        var orangeJuice = Drink.OrangeJuice();

        // Assert
        new[] { blackberryJuice, appleJuice, orangeJuice }.Are(Spec.None<Drink>()).IsSuccess.Should().BeFalse();
    }
}
