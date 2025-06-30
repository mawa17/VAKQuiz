using System.Collections.Concurrent;
namespace BlazorApp.Services.Collection;

public interface IDataService
{
    void Set<T>(string key, T? value);
    bool TryGet<T>(string key, out T? value);
    T? Get<T>(string key, T? defaultValue = default);
    bool Remove(string key);
    bool Contains(string key);
}

public sealed class DataService : IDataService
{
    private readonly ConcurrentDictionary<string, object?> _data = new();
    public void Set<T>(string key, T? value) => _ = _data.AddOrUpdate(key, value, (_, _) => value);
    public bool TryGet<T>(string key, out T? value)
    {
        if (_data.TryGetValue(key, out object? storedValue) && storedValue is T castValue)
        {
            value = castValue;
            return true;
        }

        value = default;
        return false;
    }
    public T? Get<T>(string key, T? defaultValue = default) => TryGet(key, out T? value) ? value : defaultValue;
    public bool Remove(string key) => _data.TryRemove(key, out _);
    public bool Contains(string key) => _data.ContainsKey(key);
}