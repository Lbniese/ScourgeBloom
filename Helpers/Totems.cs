/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Linq;
using ScourgeBloom.Managers;
using Styx;
using Styx.WoWInternals;

namespace ScourgeBloom.Helpers
{
    internal class Totems : ScourgeBloom
    {
        #region Totem Existance

        public static bool IsRealTotem(WoWTotem ti)
        {
            return ti != WoWTotem.None
                   && ti != WoWTotem.DummyAir
                   && ti != WoWTotem.DummyEarth
                   && ti != WoWTotem.DummyFire
                   && ti != WoWTotem.DummyWater;
        }

        /// <summary>
        ///     check if a specific totem (ie Mana Tide Totem) exists
        /// </summary>
        /// <param name="wtcheck"></param>
        /// <returns></returns>
        public static bool Exist(WoWTotem wtcheck)
        {
            var tiexist = GetTotem(wtcheck);
            return tiexist != null;
        }

        /// <summary>
        ///     check if a WoWTotemInfo object references a real totem (other than None or Dummies)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public static bool Exist(WoWTotemInfo ti)
        {
            return IsRealTotem(ti.WoWTotem);
        }

        /// <summary>
        ///     check if a type of totem (ie Air Totem) exists
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Exist(WoWTotemType type)
        {
            var wt = GetTotem(type).WoWTotem;
            return IsRealTotem(wt);
        }

        /// <summary>
        ///     check if any of several specific totems exist
        /// </summary>
        /// <param name="wt"></param>
        /// <returns></returns>
        public static bool Exist(params WoWTotem[] wt)
        {
            return wt.Any(t => Exist(t));
        }

        /// <summary>
        ///     check if a specific totem exists within its max range of a given location
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        public static bool ExistInRange(WoWPoint pt, WoWTotem tt)
        {
            if (!Exist(tt))
                return false;

            var ti = GetTotem(tt);
            return ti.Unit != null && ti.Unit.Location.Distance(pt) < GetTotemRange(tt);
        }

        /// <summary>
        ///     check if any of several totems exist in range
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="awt"></param>
        /// <returns></returns>
        public static bool ExistInRange(WoWPoint pt, params WoWTotem[] awt)
        {
            return awt.Any(t => ExistInRange(pt, t));
        }

        /// <summary>
        ///     check if type of totem (ie Air Totem) exists in range
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExistInRange(WoWPoint pt, WoWTotemType type)
        {
            var ti = GetTotem(type);
            return Exist(ti) && ti.Unit != null && ti.Unit.Location.Distance(pt) < GetTotemRange(ti.WoWTotem);
        }

        /// <summary>
        ///     gets reference to array element in Me.Totems[] corresponding to WoWTotemType of wt.  Return is always non-null and
        ///     does not indicate totem existance
        /// </summary>
        /// <param name="wt">WoWTotem of slot to reference</param>
        /// <returns>WoWTotemInfo reference</returns>
        public static WoWTotemInfo GetTotem(WoWTotem wt)
        {
            var ti = GetTotem(ToType(wt));
            if (ti.WoWTotem != wt)
                return null;
            return ti;
        }

        /// <summary>
        ///     gets reference to array element in Me.Totems[] corresponding to type.  Return is always non-null and does not
        ///     indicate totem existance
        /// </summary>
        /// <param name="type">WoWTotemType of slot to reference</param>
        /// <returns>WoWTotemInfo reference</returns>
        public static WoWTotemInfo GetTotem(WoWTotemType type)
        {
            return Me.Totems[(int) type - 1];
        }

        /// <summary>
        ///     Finds the max range of a specific totem, where you'll still receive the buff.
        /// </summary>
        /// <remarks>
        ///     Created 3/26/2011.
        /// </remarks>
        /// <param name="totem">The totem.</param>
        /// <returns>The calculated totem range.</returns>
        public static float GetTotemRange(WoWTotem totem)
        {
            switch (totem)
            {
                case WoWTotem.Tremor:
                    return 30f;

                case WoWTotem.Searing:
                    if (TalentManager.CurrentSpec == WoWSpec.ShamanElemental)
                        return 35f;
                    return 20f;

                case WoWTotem.Earthbind:
                    return 10f;

                case WoWTotem.Grounding:
                case WoWTotem.Magma:
                    return 8f;

                case WoWTotem.EarthElemental:
                case WoWTotem.FireElemental:
                    // Not really sure about these 3.
                    return 20f;

                case WoWTotem.ManaTide:
                    // Again... not sure :S
                    return 40f;


                case WoWTotem.Earthgrab:
                    return 10f;

                case WoWTotem.StoneBulwark:
                    // No idea, unlike former glyphed stoneclaw it has a 5 sec pluse shield component so range is more important
                    return 40f;

                case WoWTotem.HealingStream:
                    return 40f;

                case WoWTotem.HealingTide:
                    return 40f;

                case WoWTotem.Capacitor:
                    return 8f;

                case WoWTotem.Windwalk:
                    return 40f;

                case WoWTotem.SpiritLink:
                    return 10f;
            }

            return 0f;
        }

        public static WoWTotemType ToType(WoWTotem totem)
        {
            return (WoWTotemType) ((long) totem >> 32);
        }

        #endregion
    }
}