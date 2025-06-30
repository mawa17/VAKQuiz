using System.Text.Json;
using BlazorApp.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace BlazorApp.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AnswerModel> AnswersTable { get; set; } = null!;
    public DbSet<SurveyAnswerView> AnswerView { get; set; } = null!;
    public DbSet<LoginModel> Login { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default login
        modelBuilder.Entity<LoginModel>().HasData(
            new LoginModel { Id = 1, Username = "Admin", Password = "Pa$$w0rd!" }
        );

        // Define the JsonSerializerOptions separately before using them in HasConversion
        var jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Configure the QuestionModel to serialize List<string> into a JSON string
        modelBuilder.Entity<QuestionModel>()
            .Property(q => q.Options)
            .HasConversion(
                // Serialize the List<string> to JSON with the defined options
                v => JsonSerializer.Serialize(v, jsonOptions),
                // Deserialize the JSON back into List<string> with the defined options
                v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>()
            );

        modelBuilder.Entity<SurveyAnswerView>(entity =>
        {
            // Telling EF Core that this is a view and not a table
            entity.ToView("SurveyAnswerView"); // Name of the view in the database
            entity.HasNoKey(); // Views don't have primary keys
        });
    }
}