namespace BlazorApp.Data.Models;

public sealed class LoginModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}