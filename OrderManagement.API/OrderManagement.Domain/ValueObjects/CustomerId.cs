namespace OrderManagement.Domain.ValueObjects;

public sealed class CustomerId : IEquatable<CustomerId>
{
    public Guid Value { get; }

    // Construtor privado — só cria via factory methods
    private CustomerId(Guid value)
    {
        Value = value;
    }

    // Cria um novo ID (para novos clientes)
    public static CustomerId New() => new(Guid.NewGuid());

    // Reconstrói a partir de um Guid existente (vindo do banco, por ex.)
    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty) 
            throw new ArgumentException("CustomerId can't be empty!", nameof(value));

        return new CustomerId(value);
    }

    // Reconstrói a partir de uma string (vindo de uma requisição HTTP, por ex.)
    public static CustomerId From(string value)
    {
        if (!Guid.TryParse(value, out var customerId))
            throw new ArgumentException("Invalid CustomerId format!", nameof(value));

        return From(customerId);
    }

    // Igualdade por valor
    public bool Equals(CustomerId? other)
    {
        if (other is null) 
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as CustomerId);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator == (CustomerId? left, CustomerId? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator != (CustomerId? left, CustomerId? right) => 
        !(left == right);

    // Conversões implícitas (opcional, mas conveniente)
    public static implicit operator Guid(CustomerId id) => id.Value;

    public static implicit operator string(CustomerId id) => id.Value.ToString();

    public override string ToString() => Value.ToString();
}
