namespace Lokallagsportalen.Models;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }

    public List<Registration> Registrations { get; set; } = [];
}
