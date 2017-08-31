﻿using System;

namespace GTRevo.Core.Core
{
    public static class StringExtensions
    {
        public static int IndexOfI(this string haystack, string needle)
        {
            return haystack.IndexOf(needle, StringComparison.Ordinal);
        }

        public static int IndexOfI(this string haystack, string needle, int startIndex)
        {
            return haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
        }

        public static int IndexOfI(this string haystack, string needle, int startIndex, int count)
        {
            return haystack.IndexOf(needle, startIndex, count, StringComparison.Ordinal);
        }

        public static int LastIndexOfI(this string haystack, string needle)
        {
            return haystack.LastIndexOf(needle, StringComparison.Ordinal);
        }

        /// <summary>
        /// It the last character of given string is a backslash, result is the string without this backslash. Otherwise result is the input string.
        /// </summary>
        /// <param name="path">String to be modified.</param>
        /// <returns>String without last backslash.</returns>
        public static string RemoveLastBackslash(this string path)
        {
            if (path == null)
                return null;
            if (path[path.Length - 1] == '\\')
            {
                return path.Substring(0, path.Length - 1);
            }
            return path;
        }
    }
}
