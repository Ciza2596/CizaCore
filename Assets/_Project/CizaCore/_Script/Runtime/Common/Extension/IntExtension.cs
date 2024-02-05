using System;

namespace CizaCore
{
    public static class IntExtension
    {
        // Odd: 0, Even: 1
        public static int ToOddOrEvenIndex(this int index) =>
            CheckIsOdd(index + 1) ? 0 : 1;

        // Odd: 1, Even: 2
        public static int ToOddOrEven(this int value) =>
            CheckIsOdd(value) ? 1 : 2;

        public static bool CheckIsOdd(this int value) =>
            value % 2 == 1;


        public static int ToClamp01(this int value, bool isCircle = false) =>
            MathUtils.Clamp01(value, isCircle);

        public static int ToClamp(this int value, int min, int max, bool isCircle = false) =>
            MathUtils.Clamp(value, min, max, isCircle);

        public static string ToTime(this int value)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            var hoursText = timeSpan.Hours > 0 ? $"{timeSpan.Hours:D2}:" : string.Empty;
            return $"{hoursText}{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
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