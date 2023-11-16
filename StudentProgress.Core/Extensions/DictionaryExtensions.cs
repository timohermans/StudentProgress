namespace StudentProgress.Core.Extensions;

public static class DictionaryExtensions
{
    public static bool ContainsPair<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
    {
        return dictionary.TryGetValue(key, out var found) && found != null && found.Equals(value);
    }
}