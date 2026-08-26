using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseOrderPlatform.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Money amount cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException(
                    "Currency is required.",
                    nameof(currency));
            }

            Currency = currency.ToUpperInvariant();
            Amount = amount;
        }

        public Money Add(Money other)
        {
            ArgumentNullException.ThrowIfNull(other);

            EnsureSameCurrency(other);

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            ArgumentNullException.ThrowIfNull(other);

            EnsureSameCurrency(other);

            if (other.Amount > Amount)
            {
                throw new InvalidOperationException(
                    "Money subtraction cannot result in a negative amount.");
            }

            return new Money(Amount - other.Amount, Currency);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Cannot perform a monetary operation between {Currency} and {other.Currency}.");
            }
        }
    }
}
