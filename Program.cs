using CountriesAPI.Data;
using CountryAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CountriesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

            builder.Services.AddControllers();

            var app = builder.Build();


            app.MapGet("/api", () => "server is running");
            app.MapGet("/api/ping", () => "pong");

            app.MapGet("/api/country", async (ApplicationDbContext context) =>
            {
                var countries = await context.Countries.ToListAsync();
                return Results.Json(countries);
            });

            app.MapGet("/api/country/{id:int}", async (int id, ApplicationDbContext context) =>
            {
                var country = await context.Countries.FindAsync(id);
                return Results.Json(country ?? null);
            });

            app.MapGet("/api/country/{code}", async (string code, ApplicationDbContext context) =>
            {
                var country = await context.Countries
                    .FirstOrDefaultAsync(c => c.Alpha2Code.ToLower() == code.ToLower());

                return Results.Json(country ?? null);
            });

            app.MapPost("/api/country", async (Country country, ApplicationDbContext context) =>
            {
                if (string.IsNullOrEmpty(country.FullName) || string.IsNullOrEmpty(country.ShortName) || string.IsNullOrEmpty(country.Alpha2Code))
                {
                    return Results.BadRequest("All fields are required");
                }

                var existingCountry = await context.Countries.FirstOrDefaultAsync(c => c.Alpha2Code.ToLower() == country.Alpha2Code.ToLower());

                if (existingCountry != null)
                {
                    return Results.BadRequest("Country with this Alpha2Code already exists");
                }

                context.Countries.Add(country);
                await context.SaveChangesAsync();

                return Results.Created($"/api/country/{country.Id}", country);
            });

            app.MapDelete("/api/country/{id}", async (int id, ApplicationDbContext context) =>
            {
                var country = await context.Countries.FindAsync(id);
                if (country == null)
                {
                    return Results.NotFound();
                }

                context.Countries.Remove(country);
                await context.SaveChangesAsync();

                return Results.Ok();
            });

            app.Run();
        }
    }
}
