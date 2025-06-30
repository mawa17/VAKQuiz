using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Components.Controls;

public partial class NameForm
{
    [SupplyParameterFromForm]
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [RegularExpression(@"^[a-zA-ZÆØÅæøå\s]+$", ErrorMessage = "Name can only contain letters, spaces")]
    public string Username { get; set; } = string.Empty;

    [Parameter] public EventCallback<string> OnSubmit { get; set; }

    private async Task HandleValidSubmit()
    {
        await OnSubmit.InvokeAsync(Username);
    }
}
