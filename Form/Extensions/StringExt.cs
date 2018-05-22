using System;

namespace Sobel.Extensions
{
    public static class StringExt
    {
        public static int ToInt(this string value)
        {
            if (int.TryParse(value, out var intValue))
                return intValue;

            throw new Exception($"can't convert to int value - {value}");
        }
        
        public static double ToDouble(this string value)
        {
            if (double.TryParse(value, out var resultValue))
                return resultValue;

            throw new Exception($"can't convert to double value - {value}");
        }
    }
}