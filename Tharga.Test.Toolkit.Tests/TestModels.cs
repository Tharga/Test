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

internal static class AssignedModels
{
    public static Customer Customer() => new() { Id = 7, Name = "Ada" };

    public static OrderLine Line(string sku = "A-1") => new() { Sku = sku, Quantity = 1 };

    public static Order Order(params OrderLine[] lines) => new() { Customer = Customer(), Lines = [.. lines] };
}
