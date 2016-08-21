/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    public static class Extensions
    {
        public static string AddSpaces(this string data)
        {
            var r = new Regex(@"(?!^)(?=[A-Z])");

            return r.Replace(data, " ");
        }

        public static bool Between(this double distance, double min, double max)
        {
            return distance >= min && distance <= max;
        }

        public static bool Between(this float distance, float min, float max)
        {
            return distance >= min && distance <= max;
        }

        public static bool Between(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        public static string AlignLeft(this string s, int width)
        {
            var len = s.Length;
            if (len >= width)
                return s.Left(width);

            return s +
                   "                                                                                                                          "
                       .Left(width - len);
        }

        public static string AlignRight(this string s, int width)
        {
            var len = s.Length;
            if (len >= width)
                return s.Left(width);

            return
                "                                                                                                                          "
                    .Left(width - len) + s;
        }

        public static string Left(this string s, int c)
        {
            return s.Substring(0, c);
        }

        public static string Right(this string s, int c)
        {
            return s.Substring(c > s.Length ? 0 : s.Length - c);
        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }

        public static bool Between(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        ///     A string extension method that turns a Camel-case string into a spaced string. (Example: SomeCamelString -> Some
        ///     Camel String)
        /// </summary>
        /// <remarks>
        ///     Created 2/7/2011.
        /// </remarks>
        /// <param name="str">The string to act on.</param>
        /// <returns>.</returns>
        public static string CamelToSpaced(this string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string SafeName(this WoWUnit unit)
        {
            if (unit != null)
            {
                return unit.Name == StyxWoW.Me.Name ? "Myself" : unit.Name;
            }
            return "No Target";
        }

        public static bool IsNumeric(this string str)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var d = double.Parse(str);
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }
    }
}