using System.Collections.Generic;
public static class IEnumerableExtensions
{
    public static List<T> ToList<T>(this IEnumerable<T> source)
    {
        List<T> list = new List<T>();
        foreach(T item in source)
        {
            list.Add(item);
        }
        return new List<T>(source);
    }
}