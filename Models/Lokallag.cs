namespace Lokallagsportalen.Models;

public class Lokallag : BaseEntity
{
    public string Name { get; set; } = null!;

    public int ParishId { get; set; }
    public Parish Parish { get; set; } = null!;

    public List<Event> Events { get; set; } = [];
}
