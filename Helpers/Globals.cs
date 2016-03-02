/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Diagnostics;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    public static class Globals
    {
        public static double MyHp;
        public static bool InParty;
        public static bool HealPulsed;

        public static Stopwatch CombatTime = new Stopwatch();

        // Zone
        public static bool Pvp;

        // Enemies
        public static WoWUnit CurrentTarget;

        public static int EnemiesInRange;
        public static float MeleeRange;

        // PVP
        public static WoWUnit FocusedUnit;

        public static bool HasFocus;

        public static bool Mounted
        {
            get
            {
                var me = StyxWoW.Me;
                return !me.HasAura(164222) &&
                       !me.HasAura(165803) &&
                       !me.HasAura(178807) &&
                       !me.HasAura(157060) &&
                       !me.HasAura(157056) &&
                       (me.Mounted || me.InVehicle);
            }
        }

        public static void Update()
        {
            using (StyxWoW.Memory.TemporaryCacheState(true))
            {
                MyHp = StyxWoW.Me.HealthPercent;
                CurrentTarget = StyxWoW.Me.CurrentTarget;

                EnemiesInRange = StyxWoW.Me.GotTarget
                    ? Units.EnemiesInRange(8 + (int) StyxWoW.Me.CurrentTarget.CombatReach)
                    : EnemiesInRange = Units.EnemiesInRange(8);

                MeleeRange = StyxWoW.Me.GotTarget
                    ? Math.Max(4f, StyxWoW.Me.CombatReach + 1.3333334f + StyxWoW.Me.CurrentTarget.CombatReach)
                    : 4f;

                InParty = StyxWoW.Me.GroupInfo.IsInRaid || StyxWoW.Me.GroupInfo.IsInParty;
                Pvp = StyxWoW.Me.IsInArena || StyxWoW.Me.GroupInfo.IsInBattlegroundParty;

                /*
                // If we're not in Pvp stop
                if (!Pvp)
                    // ReSharper disable once RedundantJumpStatement
                    return;
                    */
            }
        }
    }
}