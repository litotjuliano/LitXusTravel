using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "MYR")
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty.");

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money ApplyMarkup(decimal markupAmount)
        => new(Amount + markupAmount, Currency);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on different currencies: {Currency} vs {other.Currency}.");
    }

    public override string ToString() => $"{Currency} {Amount:F2}";
}
