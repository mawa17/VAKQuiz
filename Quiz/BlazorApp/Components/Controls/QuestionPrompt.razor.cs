using BlazorApp.Data.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Controls;

public partial class QuestionPrompt : ComponentBase
{
    [Parameter, EditorRequired]
    public QuestionModel Question { get; set; } = default!;

    [Parameter]
    public QuestionState State { get; set; }

    [Parameter]
    public EventCallback<QuestionState> OnStateChange { get; set; }

    [Parameter]
    public EventCallback<QuestionModel> OnSubmit { get; set; }

    private bool CanSubmit => State.answers.Count == Question.Options.Count;

    private readonly string guid = Guid.NewGuid().ToString();
    private int x, y; /*For Visualization*/

    protected override void OnInitialized()
    {
        if (Question == null) throw new NullReferenceException($"{nameof(Question)} cannot be null!");
        if (EqualityComparer<QuestionState>.Default.Equals(State, default)) State = new(Question.Options.Count);
        if (EqualityComparer<QuestionState>.Default.Equals(State, default)) throw new NullReferenceException($"{nameof(State)} cannot be null!");
    }

    private async Task OnSubmitEventAsync()
    {
        if (!CanSubmit) return;
        if(OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(new(Question.Text, [.. State.answers.Values]));
#if DEBUG
            Console.WriteLine("INVOKE OnSubmit");
#endif
        }
    }

    private void ToggleColumnSelection(int x, int y)
    {
        string selection = $"SET{x}{y}";
        string value = Question.Options[y];
#if DEBUG
        Console.WriteLine($"{value} ({x})");
#endif
        if (State.grid[y] == selection)
        {
            State.grid[y] = null;
            _ = UpdateAnswersAsync(value, x, true);
        }
        else
        {
            State.grid[y] = selection;
            _ = UpdateAnswersAsync(value, x);
        }
        this.x = x;
        this.y = y;
        StateHasChanged();
    }

    private async Task UpdateAnswersAsync(string value, int index, bool onlyRemove = false)
    {
        var keys = State.answers.Where(x => x.Value == value);
        bool hasKey = keys.Any();
        if (hasKey)
        {
            State.answers.Remove(keys.ElementAt(0).Key);
            State.answers.Remove(index);
        }
        if (!onlyRemove) State.answers[index] = value;
        OnStateChange.InvokeAsync(State).RunSynchronously();
        if (OnStateChange.HasDelegate)
        {
            await OnStateChange.InvokeAsync(State);
#if DEBUG
            Console.WriteLine("INVOKE OnStateChange");
#endif
        }


    }
}

public readonly struct QuestionState(int gridSize)
{
    public readonly SortedDictionary<int, string> answers = [];
    public readonly string?[] grid = new string?[gridSize];
}

