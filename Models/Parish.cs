namespace Lokallagsportalen.Models;

public class Parish : BaseEntity
{
    public string Name { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public List<Lokallag> Lokallags { get; set; } = [];
}
