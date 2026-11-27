using Lokallagsportalen.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// `dotnet run -- seed` / `dotnet run -- reset`: run once and exit instead of starting the API.
if (args is ["seed" or "reset"])
{
    // Reset drops the database, so never allow this outside local development.
    if (!app.Environment.IsDevelopment())
    {
        Console.Error.WriteLine($"'{args[0]}' is only allowed in the Development environment.");
        return 1;
    }

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (args[0] == "reset") Seed.Reset(db);
    else Seed.Run(db);
    return 0;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
return 0;