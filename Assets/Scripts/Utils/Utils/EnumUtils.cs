using System;
using System.Collections.Generic;

public class EnumUtils
{
    private static Random random = new Random();

    public static T GetRandomEnumValue<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(random.Next(values.Length));
    }
    public static List<T> GetEnumValues<T>()
    {
        return new List<T>((T[])Enum.GetValues(typeof(T)));
    }
}
