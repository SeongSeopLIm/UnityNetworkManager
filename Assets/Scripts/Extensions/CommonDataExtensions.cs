public static class CommonDataExtensions
{
    public static bool CheckBit(this int baseValue, int checkValue)
    {
        return (baseValue & checkValue) == checkValue;
    }

    public static string Repeat(this string text, int repeatCount)
    {
        var result = string.Empty;
        for (var i = 0; i < repeatCount; i++)
        {
            result += text;
        }

        return result;
    }

    public static int ToInt(this string text)
    {
        var result = 0;
        int.TryParse(text, out result);
        return result;
    }
}
