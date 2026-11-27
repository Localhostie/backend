namespace Lokallagsportalen.Models;

public class City : BaseEntity
{
    public string Name { get; set; } = null!;

    public List<Parish> Parishes { get; set; } = [];
}
