using EnterpriseOrderPlatform.Domain.ValueObjects;

namespace EnterpriseOrderPlatform.UnitTests;

public class MoneyTests
{

    // Constructor tests
    [Fact]
    public void Constructor_WithValidAmountAndCurrency_CreatesMoney()
    {
        // Arrange
        const decimal amount = 100m;
        const string currency = "CAD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }
    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const decimal amount = -100m;
        const string currency = "CAD";

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Money(amount, currency));
    }
    [Fact]
    public void Constructor_WithEmptyCurrency_ThrowsArgumentException()
    {
        // Arrange
        const decimal amount = 100m;
        const string currency = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Money(amount, currency));
    }
    [Fact]
    public void Constructor_WithLowercaseCurrency_NormalizesCurrencyToUppercase()
    {
        // Arrange
        const decimal amount = 100m;
        const string currency = "cad";

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal("CAD", money.Currency);
    }

    // Add method tests
    [Fact]
    public void Add_WithSameCurrency_ReturnsCombinedAmount()
    {
        // Arrange
        var first = new Money(100m, "CAD");
        var second = new Money(25m, "CAD");

        // Act
        var result = first.Add(second);

        // Assert
        Assert.Equal(125m, result.Amount);
        Assert.Equal("CAD", result.Currency);
    }
    [Fact]
    public void Add_WithDifferentCurrencies_ThrowsInvalidOperationException()
    {
        // Arrange
        var cad = new Money(100m, "CAD");
        var usd = new Money(25m, "USD");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => cad.Add(usd));
    }


    // Subtract method tests
    [Fact]
    public void Subtract_WithSameCurrency_ReturnsDifference()
    {
        // Arrange
        var first = new Money(125m, "CAD");
        var second = new Money(25m, "CAD");

        // Act
        var result = first.Subtract(second);

        // Assert
        Assert.Equal(100m, result.Amount);
        Assert.Equal("CAD", result.Currency);
    }
    [Fact]
    public void Subtract_WhenResultWouldBeNegative_ThrowsInvalidOperationException()
    {
        // Arrange
        var first = new Money(25m, "CAD");
        var second = new Money(100m, "CAD");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => first.Subtract(second));
    }


    // compare two Money objects for equality, as we are using Money as a record type, the equality is based on the values of the properties
    [Fact]
    public void TwoMoneyObjects_WithSameAmountAndCurrency_AreEqual()
    {
        // Arrange
        var first = new Money(100m, "CAD");
        var second = new Money(100m, "CAD");

        // Act & Assert
        Assert.Equal(first, second);
    }


    // HashSet tests to ensure that Money objects with the same values are considered equal and only one instance is stored in the HashSet
    [Fact]
    public void HashSet_WithEquivalentMoneyValues_ContainsOneValue()
    {
        // Arrange
        var money = new HashSet<Money>
        {
            new Money(100m, "CAD"),
            new Money(100m, "CAD")
        };

        // Act & Assert
        Assert.Single(money);
    }


}
