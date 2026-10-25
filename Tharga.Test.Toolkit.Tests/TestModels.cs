namespace Tharga.Test.Toolkit.Tests;

internal enum Status
{
    None,
    Active
}

internal sealed class Customer
{
    public int Id { get; init; }
    public string Name { get; init; }
}

internal sealed class OrderLine
{
    public string Sku { get; init; }
    public int Quantity { get; init; }
}

internal sealed class Order
{
    public Customer Customer { get; init; }
    public List<OrderLine> Lines { get; init; }
}

internal sealed class Values
{
    public int Number { get; init; }
    public decimal Amount { get; init; }
    public double Ratio { get; init; }
    public bool Flag { get; init; }
    public DateTime When { get; init; }
    public TimeSpan Duration { get; init; }
    public Status Status { get; init; }
    public Uri Link { get; init; }
}

internal sealed class Counter
{
    public int? Count { get; init; }
}

internal struct Point
{
    public int X { get; init; }
    public int Y { get; init; }
}

internal sealed class Shape
{
    public string Name { get; init; }
    public Point Location { get; init; }
}

internal sealed class Scalars
{
    public long Total { get; init; }
    public float Weight { get; init; }
    public char Initial { get; init; }
    public Guid Key { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateOnly Born { get; init; }
    public TimeOnly Opens { get; init; }
    public System.Numerics.BigInteger Large { get; init; }
}

internal sealed class NullableScalars
{
    public long? Total { get; init; }
    public Guid? Key { get; init; }
}

internal struct Measurement
{
    public long Amount { get; init; }
    public Guid Unit { get; init; }
}

internal sealed class Reading
{
    public Measurement Value { get; init; }
}

internal sealed class Pair
{
    public (int Count, string Label) Value { get; init; }
}

internal sealed class FieldHolder
{
    public string Label;
}

internal sealed class Node
{
    public string Name { get; init; }
    public Node Parent { get; init; }
    public Node Child { get; set; }
}

internal sealed class Settings
{
    public string Host { get; init; }
    public bool UseTls { get; init; }
}

internal sealed class SharedState
{
    public static readonly string SharedField = null;

    public static string Shared { get; set; }

    public string Name { get; init; }
}

internal sealed class Defaulted
{
    public static Defaulted Default => new() { Name = "default" };

    public string Name { get; init; }
}

internal struct Spot
{
    public static Spot Origin => default;

    public int X { get; init; }
    public int Y { get; init; }
}

internal sealed class Marker
{
    public Spot Position { get; init; }
}

internal sealed class Restricted
{
    public string Name { get; init; }
    internal string Note { get; init; }
}

internal sealed record Contact(string Name, string Email);

internal sealed class Contacts
{
    public Contact Primary { get; init; }
    public Contact Backup { get; init; }
}

internal sealed class Account
{
    public int Id { get; init; }
    public string Owner { get; init; }

    public override bool Equals(object obj) => obj is Account other && other.Id == Id;

    public override int GetHashCode() => Id;
}

internal sealed class Transfer
{
    public Account From { get; init; }
    public Account To { get; init; }
}

internal sealed class Route
{
    public Point Start { get; init; }
    public Point End { get; init; }
}

internal sealed record Link
{
    public string Name { get; init; }
    public Link Next { get; set; }
}

internal struct Angle
{
    public int Degrees { get; init; }
    public Angle Normalized => this;
}

internal sealed class Bearing
{
    public Angle Heading { get; init; }
}

internal sealed record Money(decimal Amount)
{
    public Money Abs => new(Math.Abs(Amount));
}

internal sealed class Wallet
{
    public Money Balance { get; init; }
}

internal static class AssignedModels
{
    public static Customer Customer() => new() { Id = 7, Name = "Ada" };

    public static OrderLine Line(string sku = "A-1") => new() { Sku = sku, Quantity = 1 };

    public static Order Order(params OrderLine[] lines) => new() { Customer = Customer(), Lines = [.. lines] };
}
