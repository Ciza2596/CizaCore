using System;

namespace CizaCore
{
    public static class IntExtension
    {
        public static int ToClamp01(this int value, bool isCircle = false) =>
            MathUtils.Clamp01(value, isCircle);

        public static int ToClamp(this int value, int min, int max, bool isCircle = false) =>
            MathUtils.Clamp(value, min, max, isCircle);

        public static string ToTime(this int value)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            return $"{timeSpan.Minutes}:{timeSpan.Seconds}";
        }
        
        public static bool TryGetIndex(this int[] numbers, int checkedNumber, out int index)
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == checkedNumber)
                {
                    index = i;
                    return true;
                }
            }

            index = 0;
            return false;
        }
    }
}