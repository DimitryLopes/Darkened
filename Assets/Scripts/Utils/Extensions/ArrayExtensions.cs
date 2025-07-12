using System;

public static class ArrayExtensions
{
    public static void Shuffle<T>(this T[,] array)
    {
        System.Random random = new System.Random();
        int lengthRow = array.GetLength(1);

        for (int i = array.Length - 1; i > 0; i--)
        {
            int i0 = i / lengthRow;
            int i1 = i % lengthRow;

            int j = random.Next(i + 1);
            int j0 = j / lengthRow;
            int j1 = j % lengthRow;

            T temp = array[i0, i1];
            array[i0, i1] = array[j0, j1];
            array[j0, j1] = temp;
        }
    }

    public static T GetRandom<T>(this T[,] array)
    {
        System.Random random = new System.Random();
        int randomX = random.Next(array.GetLength(0));
        int randomY = random.Next(array.GetLength(1));
        return array[randomX, randomY];
    }
}
