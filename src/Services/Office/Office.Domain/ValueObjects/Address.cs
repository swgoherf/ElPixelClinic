namespace Office.Domain.ValueObjects;

public record Address(
    string City,
    string Street,
    string HouseNumber)
{
    public override string ToString() => $"{City}, {Street}, д. {HouseNumber}";
}
