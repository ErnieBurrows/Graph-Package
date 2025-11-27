using UnityEngine;

public static class AspectRatioUtility
{
    public static Vector2Int GetAspectRatio(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return Vector2Int.zero;

        int gcd = GetGreatestCommonDivisor(width, height);

        return new Vector2Int(width / gcd, height / gcd);
    }

    public static Vector2Int GetAspectRatio(Vector2Int size)
    {
        return GetAspectRatio(size.x, size.y);
    }

    private static int GetGreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
