using BlazorApp.Data.Models;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services.Collection;



public class AppDbContextService(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    public LoginModel? Login => this._context.Login.AsNoTracking().FirstOrDefault();
    public IQueryable<AnswerModel> Entity => this._context.AnswersTable
        .Include(x => x.Survey)
        .Include(y => y.Survey.Questions)
        .AsNoTracking();

    public bool UpdateLogin(string username, string password)
    {
        var loginEntry = _context.Login.Find(1);
        if (loginEntry == null) return false;
        if (loginEntry != null)
        {
            loginEntry.Username = username;
            loginEntry.Password = password;
        }
        else _context.Login.Add(new() { Username = username, Password = password });
        return _context.SaveChanges() > 0;
    }
    public async Task AddAnswerAsync(AnswerModel answer)
    {
        _context.AnswersTable.Add(answer);
        await _context.SaveChangesAsync();
#if DEBUG
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DB SAVE");
        Console.ResetColor();
        foreach (var item in answer.Survey.Questions.SelectMany(x => x.Options)) Console.WriteLine(item);
#endif
    }
    public async Task RemoveAnswerAsync(AnswerModel answer)
    {
        _context.AnswersTable.Remove(answer);
        await _context.SaveChangesAsync();
#if DEBUG
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DB SAVE");
        Console.ResetColor();
#endif
    }
}