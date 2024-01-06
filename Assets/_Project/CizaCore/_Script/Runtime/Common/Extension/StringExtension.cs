using System;
using System.Collections.Generic;
using System.Linq;

namespace CizaCore
{
    public static class StringExtension
    {
        public const char CommaTag = ',';
        public const char SemicolonTag = ';';

        public const char VerticalBarTag = '|';

        public static bool IsContains(this string[] strs, string[] targetStrs)
        {
            foreach (var targetStr in targetStrs)
            {
                if (!strs.Contains(targetStr))
                    return false;
            }

            return true;
        }

        public static bool HasValue(this string str) =>
            !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

        public static string WithoutSpace(this string str) =>
            str.Replace(" ", "");

        public static string[] ToArray(this string str, bool isIgnoreEmpty = true) =>
            str.ToList(isIgnoreEmpty).ToArray();

        public static List<string> ToList(this string str, bool isIgnoreEmpty = true)
        {
            var list = new List<string>();
            if (!str.HasValue())
                return list;

            var strWithoutSpace = str.WithoutSpace();
            if (!strWithoutSpace.Contains(CommaTag))
            {
                list.Add(strWithoutSpace);
                return list;
            }

            var splitStrs = strWithoutSpace.Split(CommaTag);
            for (var i = 0; i < splitStrs.Length; i++)
            {
                var splitStr = splitStrs[i];
                if (!splitStr.HasValue())
                {
                    if (!isIgnoreEmpty && i != splitStrs.Length - 1)
                        list.AddEmptyItem(1);
                    continue;
                }

                list.Add(splitStr);
            }

            return list;
        }

        public static string[] ToArray(this string str, int length, bool isIgnoreEmpty = true) =>
            str.ToList(length, isIgnoreEmpty).ToArray();

        public static List<string> ToList(this string str, int count, bool isIgnoreEmpty = true)
        {
            var list = new List<string>();
            if (!str.HasValue())
                return list.AddEmptyItem(count);

            var strWithoutSpace = str.WithoutSpace();
            if (!strWithoutSpace.Contains(CommaTag))
            {
                list.Add(strWithoutSpace);
                return list.AddEmptyItem(count - 1);
            }

            var splitStrs = strWithoutSpace.Split(CommaTag);
            for (var i = 0; i < splitStrs.Length; i++)
            {
                var splitStr = splitStrs[i];
                if (!splitStr.HasValue())
                {
                    if (!isIgnoreEmpty && i != splitStrs.Length - 1)
                        list.AddEmptyItem(1);
                    continue;
                }

                list.Add(splitStr);
            }

            return list.AddEmptyItem(count - list.Count);
        }

        public static List<string> AddEmptyItem(this List<string> strings, int count)
        {
            for (int i = 0; i < count; i++)
                strings.Add(string.Empty);

            return strings;
        }

        public static bool TryGetIndex(this string[] strs, string checkedStr, out int index)
        {
            for (var i = 0; i < strs.Length; i++)
            {
                if (strs[i] == checkedStr)
                {
                    index = i;
                    return true;
                }
            }

            index = 0;
            return false;
        }

        public static Dictionary<string, string>[] ToStringMapByStringArray(this string str, int count)
        {
            var stringMapByStrings = new List<Dictionary<string, string>>(count);
            var values = str.Split(VerticalBarTag);
            for (var i = 0; i < count; i++)
            {
                if (i < values.Length)
                    stringMapByStrings.Add(values[i].ToStringMapByString());
                else
                    stringMapByStrings.Add(new Dictionary<string, string>());
            }

            return stringMapByStrings.ToArray();
        }

        public static Dictionary<string, int>[] ToIntMapByStringArray(this string str, int count)
        {
            var intMapByStrings = new List<Dictionary<string, int>>(count);
            var values = str.Split(VerticalBarTag);
            for (var i = 0; i < count; i++)
            {
                if (i < values.Length)
                    intMapByStrings.Add(values[i].ToIntMapByString());
                else
                    intMapByStrings.Add(new Dictionary<string, int>());
            }

            return intMapByStrings.ToArray();
        }

        public static Dictionary<string, float>[] ToFloatMapByStringArray(this string str, int count)
        {
            var floatMapByStrings = new List<Dictionary<string, float>>(count);
            var values = str.Split(VerticalBarTag);
            for (var i = 0; i < count; i++)
            {
                if (i < values.Length)
                    floatMapByStrings.Add(values[i].ToFloatMapByString());
                else
                    floatMapByStrings.Add(new Dictionary<string, float>());
            }

            return floatMapByStrings.ToArray();
        }

        public static Dictionary<string, string>[] ToStringMapByStringArray(this string str)
        {
            if (!str.HasValue())
                return Array.Empty<Dictionary<string, string>>();

            var stringMapByStrings = new List<Dictionary<string, string>>();
            var values = str.Split(VerticalBarTag);

            foreach (var value in values)
            {
                if (!value.HasValue())
                    continue;

                stringMapByStrings.Add(value.ToStringMapByString());
            }

            return stringMapByStrings.ToArray();
        }

        public static Dictionary<string, int>[] ToIntMapByStringArray(this string str)
        {
            if (!str.HasValue())
                return Array.Empty<Dictionary<string, int>>();

            var intMapByStrings = new List<Dictionary<string, int>>();
            var values = str.Split(VerticalBarTag);

            foreach (var value in values)
            {
                if (!value.HasValue())
                    continue;

                intMapByStrings.Add(value.ToIntMapByString());
            }

            return intMapByStrings.ToArray();
        }

        public static Dictionary<string, float>[] ToFloatMapByStringArray(this string str)
        {
            if (!str.HasValue())
                return Array.Empty<Dictionary<string, float>>();

            var floatMapByStrings = new List<Dictionary<string, float>>();
            var values = str.Split(VerticalBarTag);

            foreach (var value in values)
            {
                if (!value.HasValue())
                    continue;

                floatMapByStrings.Add(value.ToFloatMapByString());
            }

            return floatMapByStrings.ToArray();
        }

        public static Dictionary<string, string> ToStringMapByString(this string str)
        {
            return str.ToMap(m_GetValue);

            string m_GetValue(string value) =>
                value;
        }

        public static Dictionary<string, int> ToIntMapByString(this string str)
        {
            return str.ToMap(m_GetValue);

            int m_GetValue(string value) =>
                int.Parse(value);
        }

        public static Dictionary<string, float> ToFloatMapByString(this string str)
        {
            return str.ToMap(m_GetValue);

            float m_GetValue(string value) =>
                float.Parse(value);
        }

        private static Dictionary<string, TValue> ToMap<TValue>(this string str, Func<string, TValue> func)
        {
            var dictionary = new Dictionary<string, TValue>();
            if (str == null)
                return dictionary;

            foreach (var text in str.Split(SemicolonTag))
            {
                if (!text.HasValue())
                    continue;

                var keyAndValue = text.Split(CommaTag);
                if (keyAndValue.Length != 2)
                    continue;

                dictionary.Add(keyAndValue[0].Trim(), func.Invoke(keyAndValue[1]));
            }

            return dictionary;
        }
    }
}