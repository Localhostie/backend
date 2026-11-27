namespace Lokallagsportalen.Models;

// Join entity: one registration per user per event (composite key set in AppDbContext).
public class Registration
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }
}
