using System.Collections.Generic;
using System.Linq;

namespace CizaCore
{
    public static class StringExtension
    {
        public const char SplitTag = ',';
        
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
            if (!strWithoutSpace.Contains(SplitTag))
            {
                list.Add(strWithoutSpace);
                return list;
            }

            var splitStrs = strWithoutSpace.Split(SplitTag);
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
            if (!strWithoutSpace.Contains(SplitTag))
            {
                list.Add(strWithoutSpace);
                return list.AddEmptyItem(count - 1);
            }

            var splitStrs = strWithoutSpace.Split(SplitTag);
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
    }
}