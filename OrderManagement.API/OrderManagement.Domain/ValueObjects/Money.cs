namespace OrderManagement.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // Factory methods
    public static Money Of(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new ArgumentException("Amount can't be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency can't be empty", nameof(currency));

        return new Money(decimal.Round(amount, 2), currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = "BRL") => Of(0m, currency);

    // ── Operações
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;

        if (result < 0)
            throw new InvalidOperationException("Subtraction would result in a negative value");

        return new Money(result, Currency);
    }

    public Money Multiply(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        return new Money(Amount * quantity, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor <= 0)
            throw new ArgumentException("Factor must be greater than zero", nameof(factor));

        return new Money(decimal.Round(Amount * factor, 2), Currency);
    }

    // ── Comparações 
    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return Amount > other.Amount;
    }

    public bool IsZero() => Amount == 0m;

    // ── Sobrecarga de operadores 
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);

    public static Money operator *(Money left, int quantity) => left.Multiply(quantity);
    public static Money operator *(int quantity, Money right) => right.Multiply(quantity);
    public static Money operator *(Money left, decimal factor) => left.Multiply(factor);
    public static Money operator *(decimal factor, Money right) => right.Multiply(factor);

    public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);
    public static bool operator <(Money left, Money right) => right.IsGreaterThan(left);
    public static bool operator ==(Money? left, Money? right) => left?.Equals(right) ?? right is null;
    public static bool operator !=(Money? left, Money? right) => !(left == right);
    public static bool operator >=(Money left, Money right) => left == right || left > right;
    public static bool operator <=(Money left, Money right) => left == right || left < right;

    // ── Validação interna 
    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Operation between different currencies: {Currency} e {other.Currency}.");
    }

    // ── Igualdade por valor
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Currency} {Amount:F2}";
}