namespace Lokallagsportalen.Models;

// Timestamps are meant to be set in the DbContext's SaveChanges, not by hand.
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
