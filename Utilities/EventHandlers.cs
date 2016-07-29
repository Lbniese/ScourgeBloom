/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ScourgeBloom.Helpers;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Utilities
{
    public static class EventHandlers
    {
        private static bool _combatLogAttached;

        private static string _localizedUnitNotInfrontFailure;
        private static string _localizedNoPathAvailableFailure;

        public static Dictionary<WoWGuid, int> MobsThatEvaded = new Dictionary<WoWGuid, int>();
        public static Queue<Damage> DamageHistory { get; set; }
        public static bool TrackDamage { get; set; }

        /// <summary>
        ///     time of last "Target not in line of sight" spell failure.
        ///     Used by movement functions for situations where the standard
        ///     LoS and LoSS functions are true but still fails in WOW.
        ///     See CreateMoveToLosBehavior() for usage
        /// </summary>
        public static DateTime LastLineOfSightFailure { get; set; }

        public static DateTime LastUnitNotInfrontFailure { get; set; }
        public static DateTime LastNoPathFailure { get; set; }
        public static DateTime SuppressShapeshiftUntil { get; set; }
        public static bool IsShapeshiftSuppressed => SuppressShapeshiftUntil > DateTime.UtcNow;

        public static WoWUnit LastLineOfSightTarget { get; set; }
        public static WoWGuid LastUnitNotInfrontGuid { get; set; }
        public static WoWGuid LastNoPathGuid { get; set; }

        public static WoWUnit AttackingEnemyPlayer { get; set; }
        public static WoWSpellSchool AttackedWithSpellSchool { get; set; }
        private static DateTime TimeLastAttackedByEnemyPlayer { get; set; }
        public static TimeSpan TimeSinceAttackedByEnemyPlayer => DateTime.UtcNow - TimeLastAttackedByEnemyPlayer;

        public static DateTime LastRedErrorMessage { get; set; }

        public static void Init()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                // get locale specific messasge strings we'll check for
                InitializeLocalizedValues();

                // set default values for timed error states
                LastLineOfSightFailure = DateTime.MinValue;
                LastUnitNotInfrontFailure = DateTime.MinValue;
                SuppressShapeshiftUntil = DateTime.MinValue;

                // reset the damage history
                DamageHistory = new Queue<Damage>(50);

                // hook combat log event if we are debugging or not in performance critical circumstance
                if (GeneralSettings.Instance.Debug ||
                    (ScourgeBloom.CurrentWoWContext != WoWContext.Battlegrounds && !StyxWoW.Me.CurrentMap.IsRaid))
                    EventLog.AttachCombatLogEvent();

                // add context handler that reacts to context change with above rules for logging
                ScourgeBloom.OnWoWContextChanged += HandleContextChanged;

                // hook PVP start timer so we can identify end of prep phase
                PvP.AttachStartTimer();

                // also hook wow error messages
                Lua.Events.AttachEvent("UI_ERROR_MESSAGE", HandleErrorMessage);

                // hook LOOT_BIND_CONFIRM to handle popup appearing when applying certain spells to weapon
                // Lua.Events.AttachEvent("AUTOEQUIP_BIND_CONFIRM", HandleLootBindConfirm);
                // Lua.Events.AttachEvent("LOOT_BIND_CONFIRM", HandleLootBindConfirm);
                Lua.Events.AttachEvent("END_BOUND_TRADEABLE", HandleEndBoundTradeable);

                Lua.Events.AttachEvent("PARTY_MEMBER_DISABLE", HandlePartyMemberDisable);
                Lua.Events.AttachEvent("PARTY_MEMBER_ENABLE", HandlePartyMemberEnable);
            }
        }

        private static void InitializeLocalizedValues()
        {
            // get localized copies of spell failure error messages
            GetSymbolicLocalizeValue("SPELL_FAILED_LINE_OF_SIGHT");
            _localizedUnitNotInfrontFailure = GetSymbolicLocalizeValue("SPELL_FAILED_UNIT_NOT_INFRONT");
            _localizedNoPathAvailableFailure = GetSymbolicLocalizeValue("SPELL_FAILED_NOPATH");
        }

        internal static void HandleContextChanged(object sender, WoWContextEventArg e)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current == null ||
                RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            if (GeneralSettings.Instance.Debug ||
                (ScourgeBloom.CurrentWoWContext != WoWContext.Battlegrounds && !StyxWoW.Me.CurrentMap.IsRaid))
                EventLog.AttachCombatLogEvent();
            else
                EventLog.DetachCombatLogEvent();
        }

        public static bool IsPathErrorTarget(this WoWUnit unit)
        {
            if (unit.Guid != LastNoPathGuid)
                return false;
            if (LastNoPathFailure < DateTime.UtcNow - TimeSpan.FromMinutes(15))
                return false;
            return true;
        }

        public static bool IsNotFacingErrorTarget(this WoWUnit unit)
        {
            if (unit.Guid != LastUnitNotInfrontGuid)
                return false;
            if (LastNoPathFailure < DateTime.UtcNow - TimeSpan.FromMilliseconds(750))
                return false;
            return true;
        }


        public static int EvadedAttacksCount(this WoWUnit unit)
        {
            if (!MobsThatEvaded.ContainsKey(unit.Guid))
                return 0;

            return MobsThatEvaded[unit.Guid];
        }

        private static void HandleErrorMessage(object sender, LuaEventArgs args)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            // bool handled = false;
            LastRedErrorMessage = DateTime.UtcNow;

            if (GeneralSettings.Instance.Debug)
            {
                Logging.WriteDiagnostic("[WoWRedError] {0}", args.Args[0]);
            }
        }

        private static void HandlePartyMemberEnable(object sender, LuaEventArgs args)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            var pm =
                Units.GroupMemberInfos.FirstOrDefault(
                    g =>
                        g.ToPlayer() != null &&
                        string.Equals(g.ToPlayer().Name, args.Args[0].ToString(), StringComparison.InvariantCulture));
            var name = "(null)";
            var status = "(unknown)";

            if (pm == null)
            {
                Logging.WriteDiagnostic("Group Member: {0} enabled but could not be found", args.Args[0]);
            }
            else
            {
                var o = ObjectManager.GetObjectByGuid<WoWUnit>(pm.Guid);
                name = o.Name;
                status = "Alive";
                Logging.WriteDiagnostic("Group Member {0}: {1} {2}", pm.RaidRank, name, status);
            }
        }


        private static void HandlePartyMemberDisable(object sender, LuaEventArgs args)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            var pm =
                Units.GroupMemberInfos.FirstOrDefault(
                    g =>
                        g.ToPlayer() != null &&
                        string.Equals(g.ToPlayer().Name, args.Args[0].ToString(), StringComparison.InvariantCulture));
            var name = "(null)";
            var status = "(unknown)";

            if (pm == null)
            {
                Logging.WriteDiagnostic("Group Member: {0} disabled but could not be found", args.Args[0]);
            }
            else
            {
                var o = ObjectManager.GetObjectByGuid<WoWUnit>(pm.Guid);
                name = o.Name;
                if (!o.IsAlive)
                    status = "Died!";
                else if (!pm.IsOnline)
                    status = "went Offline";

                Logging.WriteDiagnostic("Group Member {0}: {1} {2}", pm.RaidRank, name, status);
            }
        }


        private static string GetSymbolicLocalizeValue(string symbolicName)
        {
            var localString = Lua.GetReturnVal<string>("return " + symbolicName, 0);
            return localString;
        }

        private static void AddSymbolicLocalizeValue(this Dictionary<string, string> dict, string symbolicName)
        {
            var localString = GetSymbolicLocalizeValue(symbolicName);
            if (!string.IsNullOrEmpty(localString) && !dict.ContainsKey(localString))
            {
                dict.Add(localString, symbolicName);
            }
        }

        private static void HandleEndBoundTradeable(object sender, LuaEventArgs args)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            var argval = args.Args[0].ToString();
            Logging.Write(Colors.LightGreen, "EndBoundTradeable: confirming '{0}'", argval);
            var cmd = string.Format("EndBoundTradeable('{0}')", argval);
            Logging.WriteDiagnostic("END_BOUND_TRADEABLE: confirm with \"{0}\"", cmd);
            Lua.DoString(cmd);
        }

        /// <summary>
        ///     gets the damage occuring in the last maxage seconds.  removes damage
        ///     entries from queue older than maxage
        /// </summary>
        /// <param name="maxage">seconds to calculate damage received</param>
        /// <returns>damage received</returns>
        public static long GetRecentDamage(float maxage)
        {
            var since = DateTime.UtcNow - TimeSpan.FromSeconds(maxage);
            while (DamageHistory.Any())
            {
                var next = DamageHistory.Peek();
                if (next.Time >= since)
                    break;

                DamageHistory.Dequeue();
            }

            long sum = 0;
            foreach (var q in DamageHistory)
            {
                if (GeneralSettings.Instance.Debug)
                {
                    if (q.Time < since)
                    {
                        Logging.WriteDiagnostic(
                            "GetRecentDamage: Program Error: entry {0} {1:HH:mm:ss.FFFF} older than {2:HH:mm:ss.FFFF}",
                            q.Amount, q.Time, since);
                    }
                }
                sum += q.Amount;
            }
            return DamageHistory.Sum(v => v.Amount);
        }

        /// <summary>
        ///     gets the damage occuring in the last maxage seconds.  removes damage
        ///     entries from queue older than maxage.  additionally calculates damage
        ///     at another time boundary less than maxage (referred to as recent)
        /// </summary>
        /// <param name="maxage">seconds to calculate damage received</param>
        /// <param name="alldmg">damage received since maxage</param>
        /// <param name="recentage">more recent timeframe</param>
        /// <param name="recentdmg">damage since more recent timeframe</param>
        public static void GetRecentDamage(float maxage, out long alldmg, float recentage, out long recentdmg)
        {
            var now = DateTime.UtcNow;
            var sinceoldest = now - TimeSpan.FromSeconds(maxage);
            var sincerecent = now - TimeSpan.FromSeconds(recentage);

            recentdmg = 0;
            alldmg = 0;

            if (DamageHistory == null)
                return;

            while (DamageHistory.Any())
            {
                var next = DamageHistory.Peek();
                if (next.Time >= sinceoldest)
                    break;

                DamageHistory.Dequeue();
            }

            foreach (var q in DamageHistory)
            {
                alldmg += q.Amount;
                if (q.Time < sincerecent)
                    recentage += q.Amount;

                if (GeneralSettings.Instance.Debug)
                {
                    if (q.Time < sinceoldest)
                    {
                        Logging.WriteDiagnostic(
                            "GetRecentDamage: Program Error: entry {0} {1:HH:mm:ss.FFFF} older than {2:HH:mm:ss.FFFF}",
                            q.Amount, q.Time, sinceoldest);
                    }
                }
            }
        }
    }

    public class Damage
    {
        public Damage(DateTime time, long amt)
        {
            Time = time;
            Amount = amt;
        }

        public DateTime Time { get; set; }
        public long Amount { get; set; }
    }
}