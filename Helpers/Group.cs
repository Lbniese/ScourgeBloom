/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    internal static class Group
    {
        public static bool MeIsTank
        {
            get
            {
                return (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) != 0 ||
                       Tanks.All(t => !t.IsAlive) && StyxWoW.Me.HasAura("Bear Form");
            }
        }

        public static List<WoWPlayer> Tanks
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty)
                    return new List<WoWPlayer>();

                return StyxWoW.Me.GroupInfo.RaidMembers.Where(p => p.HasRole(WoWPartyMember.GroupRole.Tank))
                    .Select(p => p.ToPlayer())
                    .Union(new[] {RaFHelper.Leader})
                    .Where(p => p != null && p.IsValid)
                    .Distinct()
                    .ToList();
            }
        }

        public static List<WoWPlayer> Healers
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty)
                    return new List<WoWPlayer>();

                return StyxWoW.Me.GroupInfo.RaidMembers.Where(p => p.HasRole(WoWPartyMember.GroupRole.Healer))
                    .Select(p => p.ToPlayer()).Where(p => p != null).ToList();
            }
        }


        public static List<WoWPlayer> Dps
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty)
                    return new List<WoWPlayer>();

                return
                    StyxWoW.Me.GroupInfo.RaidMembers.Where(
                        p => !p.HasRole(WoWPartyMember.GroupRole.Tank) && !p.HasRole(WoWPartyMember.GroupRole.Healer))
                        .Select(p => p.ToPlayer()).Where(p => p != null).ToList();
            }
        }

        /// <summary>
        ///     True: if a group member with Tank role (not spec) is alive and within 40 yds
        /// </summary>
        public static bool AnyTankNearby
        {
            get { return Tanks.Any(h => h.IsAlive && h.Distance < 40); }
        }

        /// <summary>
        ///     True: if a group member with Healer role (not spec) is alive and within 40 yds
        /// </summary>
        public static bool AnyHealerNearby
        {
            get { return Healers.Any(h => h.IsAlive && h.Distance < 40); }
        }

        /// <summary>Gets a player by class priority. The order of which classes are passed in, is the priority to find them.</summary>
        /// <remarks>Created 9/9/2011.</remarks>
        /// <param name="range"></param>
        /// <param name="includeDead"></param>
        /// <param name="classes">A variable-length parameters list containing classes.</param>
        /// <returns>The player by class prio.</returns>
        public static WoWUnit GetPlayerByClassPrio(float range, bool includeDead, params WoWClass[] classes)
        {
            return (from woWClass in classes
                select
                    StyxWoW.Me.GroupInfo.RaidMembers.FirstOrDefault(
                        p => p.ToPlayer() != null && p.ToPlayer().Distance < range && p.ToPlayer().Class == woWClass)
                into unit
                where unit != null
                where !includeDead && unit.Dead || unit.Ghost
                select unit.ToPlayer()).FirstOrDefault();
        }
    }
}