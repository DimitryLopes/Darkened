using System;
using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    private static Random random = new Random();

    public static KeyValuePair<TKey, TValue> GetRandom<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.Count == 0)
        {
            throw new InvalidOperationException("The dictionary is empty.");
        }

        int index = random.Next(dictionary.Count);
        return dictionary.ElementAt(index);
    }
}