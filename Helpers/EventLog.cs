/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Windows.Media;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;

namespace ScourgeBloom.Helpers
{
    internal class EventLog : ScourgeBloom
    {
        private static bool _combatLogAttached;

        public static void AttachCombatLogEvent()
        {
            if (_combatLogAttached)
                DetachCombatLogEvent();

            // DO NOT EDIT THIS UNLESS YOU KNOW WHAT YOU'RE DOING!
            // This ensures we only capture certain combat log events, not all of them.
            // This saves on performance, and possible memory leaks. (Leaks due to Lua table issues.)
            var myGuid = Lua.GetReturnVal<string>("return UnitGUID('player');", 0);
            Logging.WriteDiagnostic("[ScourgeBloom] CombatLogEvent: setting filter= {0}",
                BuildCombatLogEventFilter("PlayerGUID"));
            Lua.Events.AttachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLog, BuildCombatLogEventFilter(myGuid));
            _combatLogAttached = true;

            Logging.WriteDiagnostic("[ScourgeBloom] Attached combat log");
        }

        public static void DetachCombatLogEvent()
        {
            if (_combatLogAttached)
            {
                Logging.WriteDiagnostic("[ScourgeBloom] Detached combat log");
                Lua.Events.DetachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLog);
                _combatLogAttached = false;
            }
        }

        private static string BuildCombatLogEventFilter(string myGuid)
        {
            var filterCriteria = "return";

            if (CurrentWoWContext == WoWContext.Normal && GeneralSettings.TargetWorldPvpRegardless)
            {
                filterCriteria +=
                    " ("
                    + " args[8] == " + "'" + myGuid + "'"
                    + " and args[4] ~= args[8]"
                    + " and bit.band(args[6], COMBATLOG_OBJECT_CONTROL_PLAYER) > 0"
                    + " and 'Player' == args[4]:sub(1,6)"
                    +
                    " and (args[2] == 'SPELL_DAMAGE' or args[2] == 'SPELL_PERIODIC_DAMAGE' or args[2] == 'RANGE_DAMAGE' or args[2] == 'SWING_DAMAGE')"
                    + ")"
                    + " or";
                // filterCriteria += " (args[8] == UnitGUID('player') and args[8] ~= args[4] and 0x000 == bit.band(tonumber('0x'..strsub(guid, 3,5)),0x00f)) or";
            }

            // standard portion of filter
            filterCriteria +=
                " ("
                + " args[4] == " + "'" + myGuid + "'"
                + " and"
                + " ("
                + " args[2] == 'SPELL_MISSED'"
                + " or args[2] == 'RANGE_MISSED'"
                + " or args[2] == 'SWING_MISSED'"
                + " or args[2] == 'SPELL_CAST_FAILED'"
                + " )"
                + " )";


            return filterCriteria;
        }

        public static void HandleCombatLog(object sender, LuaEventArgs args)
        {
            var e = new CombatLog(args.EventName, args.FireTimeStamp, args.Args);
            switch (e.Event)
            {
                case "SPELL_AURA_APPLIED":
                    if (e.DestName != StyxWoW.Me.Name)
                        Log.WritetoFile(LogLevel.Diagnostic,
                            string.Format("Affected By: {0}({1})", e.SpellName, e.SpellId));
                    break;
                case "SPELL_CAST_FAILED":
                    if (e.Args[14].ToString() == "SPELL_FAILED_LINE_OF_SIGHT" && e.SourceName == StyxWoW.Me.Name &&
                        e.DestName != "[LuaTValue Type: Nil]")
                        Blacklist.Add(e.DestUnit, BlacklistFlags.Combat, TimeSpan.FromSeconds(1));
                    if (e.Args[14].ToString() == "No path available")
                    {
                        //Small hack to blacklist spell for 5 seconds if you have a pathing issue.
                        Spell.UpdateSpellHistory(e.SpellName, 5000, e.DestUnit);
                        Lua.DoString("SpellStopTargeting()");
                    }
                    else
                    {
                        Spell.UpdateSpellHistory(e.SpellName, 1000, e.DestUnit);
                    }
                    Log.WriteLog(
                        string.Format("[ScourgeBloom] {0} missed, reason {3} => {1}@{2}", e.SpellName,
                            e.DestUnit.SafeName(),
                            e.DestUnit.Status(), e.Args[14]), Colors.Red);
                    break;
                case "SPELL_CAST_SUCCESS":
                    if (e.DestName != "[LuaTValue Type: Nil]")
                    {
                        Spell.UpdateSpellHistory(e.SpellName, e.Spell.CooldownTimeLeft.TotalMilliseconds, e.DestUnit);
                        if (e.SourceName == StyxWoW.Me.Name)
                            Log.WriteLog(
                                string.Format("[ScourgeBloom] Landed {0} => {1}@{2}", e.SpellName, e.DestUnit.SafeName(),
                                    e.DestUnit.Status()), Colors.GreenYellow);
                    }
                    if (e.DestName == "[LuaTValue Type: Nil]" && e.SourceName == StyxWoW.Me.Name)
                    {
                        Spell.UpdateSpellHistory(e.SpellName, e.Spell.CooldownTimeLeft.TotalMilliseconds, e.DestUnit);
                        Log.WriteLog(
                            string.Format("[ScourgeBloom] Landed {0} => {1}@{2}", e.SpellName, "Me", e.DestUnit.Status()),
                            Colors.GreenYellow);
                    }
                    break;
                case "RANGE_MISSED":
                    Log.WriteLog(
                        string.Format("{0} missed, reason {3} => {1}@{2}", e.SpellName, e.DestUnit.SafeName(),
                            e.DestUnit.Status(), e.Args[14]), Colors.Red);
                    break;
            }
        }
    }
}