namespace Lokallagsportalen.Models;

public class Event : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime RegisterDeadline { get; set; }
    public int? Capacity { get; set; }
    public int? AgeLimit { get; set; }

    public int LokallagId { get; set; }
    public Lokallag Lokallag { get; set; } = null!;

    public List<Registration> Registrations { get; set; } = [];
}
