using Lokallagsportalen.Models;
using Microsoft.EntityFrameworkCore;

namespace Lokallagsportalen.Data;

// Run from backend/:  dotnet run -- seed   |   dotnet run -- reset
public static class Seed
{
    // 3 cities × 3 parishes × 2 lokallag (Nord/Sør) × 1 event each = 18 events.
    private static readonly Dictionary<string, string[]> CityParishes = new()
    {
        ["Bergen"] = ["Fana", "Åsane", "Årstad"],
        ["Oslo"] = ["Frogner", "Grünerløkka", "Majorstuen"],
        ["Trondheim"] = ["Lade", "Byåsen", "Heimdal"],
    };

    private static readonly (string Name, string Description, string Place)[] EventTemplates =
    [
        ("Dugnad", "Vi rydder nærområdet sammen. Ta med hansker.", "Nærmiljøparken"),
        ("Kaffeprat", "En-til-en samtale over en kopp kaffe.", "Kafeen"),
        ("Vinsmaking", "Smaking av norske og franske viner.", "Kulturhuset"),
        ("Quizkveld", "Lagquiz med premier til vinnerne.", "Grendehuset"),
        ("Turdag", "Felles tur i nærområdet. Ta med niste.", "Parkeringsplassen"),
        ("Strikkekafé", "Strikking og prat. Garn finnes på stedet.", "Biblioteket"),
    ];

    // Applies migrations, then inserts test data if the database is empty.
    public static void Run(AppDbContext db)
    {
        db.Database.Migrate();

        if (db.Cities.Any())
        {
            Console.WriteLine("Database already has data, skipping seed. Use 'reset' to start over.");
            return;
        }

        var firstStart = new DateTime(2027, 3, 4, 18, 0, 0, DateTimeKind.Utc);
        var eventNo = 0;

        foreach (var (cityName, parishNames) in CityParishes)
        {
            var city = new City { Name = cityName };
            foreach (var parishName in parishNames)
            {
                var parish = new Parish { Name = parishName };
                foreach (var side in new[] { "Nord", "Sør" })
                {
                    var template = EventTemplates[eventNo % EventTemplates.Length];
                    var start = firstStart.AddDays(7 * eventNo); // one event per week, Thursdays 18:00
                    parish.Lokallags.Add(new Lokallag
                    {
                        Name = $"{parishName} {side} Lokallag",
                        Events =
                        [
                            new Event
                            {
                                Name = $"{template.Name} i {parishName}",
                                Description = template.Description,
                                Location = $"{template.Place}, {parishName}",
                                StartTime = start,
                                EndTime = start.AddHours(2),
                                RegisterDeadline = start.AddDays(-3),
                                // Test cases: every Kaffeprat is full after one registration,
                                // every Vinsmaking is 18+.
                                Capacity = template.Name == "Kaffeprat" ? 1 : null,
                                AgeLimit = template.Name == "Vinsmaking" ? 18 : null,
                            },
                        ],
                    });
                    eventNo++;
                }
                city.Parishes.Add(parish);
            }
            db.Cities.Add(city);
        }

        db.SaveChanges();
        Console.WriteLine($"Seeded {CityParishes.Count} cities and {eventNo} events.");
    }

    // Drops the database, recreates it from migrations and seeds it again.
    public static void Reset(AppDbContext db)
    {
        db.Database.EnsureDeleted();
        Run(db);
    }
}
