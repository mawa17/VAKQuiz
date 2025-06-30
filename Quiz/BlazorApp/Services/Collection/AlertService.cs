using Microsoft.JSInterop;

namespace BlazorApp.Services.Collection;

public interface IAlertService
{
    Task ShowAlertAsync(string message);
}
public sealed class AlertService(IJSRuntime jsRuntime) : IAlertService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    public async Task ShowAlertAsync(string message) => await _jsRuntime.InvokeVoidAsync("alert", message);
}