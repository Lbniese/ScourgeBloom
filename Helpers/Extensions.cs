/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    public static class Extensions
    {
        public static bool IsValidCombatUnit(this WoWUnit unit)
        {
            return unit != null && unit.IsValid && unit.IsAlive && unit.Attackable;
        }

        /// <summary>
        /// determines if a target is off the ground far enough that you can
        /// reach it with melee spells if standing directly under.
        /// </summary>
        /// <param name="u">unit</param>
        /// <returns>true if above melee reach</returns>
        public static bool IsAboveTheGround(this WoWUnit u)
        {
            // temporary change while working out issues with using mesh to check if off ground
            // return !Styx.Pathing.Navigator.CanNavigateFully(StyxWoW.Me.Location, u.Location);

            float height = HeightOffTheGround(u);
            if (height == float.MaxValue)
                return false;   // make this true if better to assume aerial 

            if (height >= StyxWoW.Me.MeleeDistance(u))
                return true;

            return false;
        }

        /// <summary>
        /// calculate a unit's vertical distance (height) above ground level (mesh).  this is the units position
        /// relative to the ground and is independent of any other character.  note: this isn't actually the ground,
        /// it's the height from the mesh and the mesh is not guarranteed to be flush with the terrain (which is why we add the +2f)
        /// </summary>
        /// <param name="u">unit</param>
        /// <returns>float.MinValue if can't determine, otherwise distance off ground</returns>
        public static float HeightOffTheGround(this WoWUnit u)
        {
            var unitLoc = new Vector3(u.Location.X, u.Location.Y, u.Location.Z);
            float zBelow = u.FindGroundBelow();
            if (zBelow == float.MaxValue)
                return float.MaxValue;

            return unitLoc.Z - zBelow;
        }

        /// <summary>
        /// calculate the Z of ground below unit.
        /// </summary>
        /// <param name="unit">unit to query</param>
        /// <returns>float.MaxValue if non-deterministic, otherwise Z of ground</returns>
        public static float FindGroundBelow(this WoWUnit unit)
        {
            var unitLoc = unit.Location;
            return unitLoc.Z;
            // TODO: FindHeights
            // This can use mesh sampling with HighlyConnected or Any
            //            var listMeshZ = Navigator.FindHeights(unitLoc.X, unitLoc.Y);
            //            if (listMeshZ != null)
            //            {
            //                listMeshZ = listMeshZ.Where(h => h <= unitLoc.Z + 2f).ToList();
            //                if (listMeshZ.Any())
            //                    return listMeshZ.Max();
            //            }
            //            return float.MaxValue;
        }

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