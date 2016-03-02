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
using ScourgeBloom.Helpers;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Managers
{
    internal class PetManager
    {
        public static readonly WaitTimer CallPetTimer = WaitTimer.OneSecond;

        private static WoWGuid _petGuid;
        private static readonly List<WoWPetSpell> PetSpells = new List<WoWPetSpell>();
        public static readonly WaitTimer PetSummonAfterDismountTimer = new WaitTimer(TimeSpan.FromSeconds(2));

        private static bool _wasMounted;

        static PetManager()
        {
            // NOTE: This is a bit hackish. This fires VERY OFTEN in major cities. But should prevent us from summoning right after dismounting.
            // Lua.Events.AttachEvent("COMPANION_UPDATE", (s, e) => CallPetTimer.Reset());
            // Note: To be changed to OnDismount with new release
            Mount.OnDismount += (s, e) =>
            {
                if (StyxWoW.Me.PetNumber > 0)
                {
                    PetSummonAfterDismountTimer.Reset();
                }
            };

            // force us to check initially upon load
        }

        public static bool HavePet
        {
            get { return StyxWoW.Me.GotAlivePet; }
        }

        public static string WantedPet { get; set; }

        internal static void Pulse()
        {
            if (StyxWoW.Me.Mounted)
            {
                _wasMounted = true;
            }

            if (_wasMounted && !StyxWoW.Me.Mounted)
            {
                _wasMounted = false;
                PetSummonAfterDismountTimer.Reset();
            }

            if (StyxWoW.Me.Pet != null)
            {
                if (_petGuid != StyxWoW.Me.Pet.Guid)
                {
                    PetSpells.Clear();

                    // only load spells if we have one that is non-null
                    // .. as initial load happens before Me.PetSpells is initialized and we were saving 'null' spells
                    if (StyxWoW.Me.PetSpells.Any(s => s.Spell != null))
                    {
                        // Cache the list. yea yea, we should just copy it, but I'd rather have shallow copies of each object, rather than a copy of the list.
                        PetSpells.AddRange(StyxWoW.Me.PetSpells);
                        PetSummonAfterDismountTimer.Reset();
                        _petGuid = StyxWoW.Me.Pet.Guid;

                        Log.WriteLog(LogLevel.Diagnostic, "[ScourgeBloom] ---PetSpells Loaded---");
                        foreach (var sp in PetSpells)
                        {
                            Log.WriteLog(LogLevel.Diagnostic,
                                sp.Spell == null
                                    ? string.Format("[ScourgeBloom]    {0} spell={1}  Action={0}", sp.ActionBarIndex, sp)
                                    : string.Format("[ScourgeBloom]    {0} spell={1} #{2}", sp.ActionBarIndex, sp,
                                        sp.Spell.Id));
                        }
                        Log.WriteLog(LogLevel.Diagnostic, " ");
                    }
                }
            }

            if (!StyxWoW.Me.GotAlivePet)
            {
                PetSpells.Clear();
            }
        }

        public static bool CanCastPetAction(string action)
        {
            var petAction = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (petAction == null || petAction.Spell == null)
            {
                return false;
            }

            return !petAction.Spell.Cooldown;
        }

        public static void CastPetAction(string action)
        {
            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Log.WriteLog(string.Format("[ScourgeBloom] [Pet] Casting {0}", action));
            Lua.DoString("CastPetAction({0})", spell.ActionBarIndex + 1);
        }

        public static void CastPetAction(string action, WoWUnit on)
        {
            // target is currenttarget, then use simplified version (to avoid setfocus/setfocus
            if (on == StyxWoW.Me.CurrentTarget)
            {
                CastPetAction(action);
                return;
            }

            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Log.WriteLog(string.Format("[ScourgeBloom] [Pet] Casting {0} on {1}", action, on.SafeName()));
            var save = StyxWoW.Me.FocusedUnit;
            StyxWoW.Me.SetFocus(on);
            Lua.DoString("CastPetAction({0}, 'focus')", spell.ActionBarIndex + 1);
            StyxWoW.Me.SetFocus(save == null ? WoWGuid.Empty : save.Guid);
        }

        /// <summary>
        ///     behavior form of CastPetAction().  note that this Composite will return RunStatus.Success
        ///     if it appears the ability was cast.  this is to trip the Throttle wrapping it internally
        ///     -and- to allow cascaded sequences of Pet Abilities.  Note: Pet Abilities are not on the
        ///     GCD, so you can safely allow execution to continue even on Success
        /// </summary>
        /// <param name="action">pet ability</param>
        /// <returns></returns>
        public static void EnableActionAutocast(string action)
        {
            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            var index = spell.ActionBarIndex + 1;
            Log.WriteLog(string.Format("[ScourgeBloom] [Pet] Enabling autocast for {0}", action));
            Lua.DoString("local index = " + index +
                         " if not select(6, GetPetActionInfo(index)) then TogglePetAutocast(index) end");
        }

        /// <summary>
        ///     Calls a pet by name, if applicable.
        /// </summary>
        /// <remarks>
        ///     Created 2/7/2011.
        /// </remarks>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool IsAutoCast(int id, out bool allowed)
        {
            var ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.Spell != null && s.Spell.Id == id);
            return IsAutoCast(ps, out allowed);
        }

        public static bool IsAutoCast(string action, out bool allowed)
        {
            var ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.ToString() == action);
            return IsAutoCast(ps, out allowed);
        }

        public static bool IsAutoCast(WoWPetSpell ps, out bool allowed)
        {
            allowed = false;
            if (ps == null) return false;
            // action bar index base 0 in HB but base 1 in LUA, so adjust
            var svals = Lua.GetReturnValues("return GetPetActionInfo(" + (ps.ActionBarIndex + 1) + ");");
            if (svals == null || svals.Count < 7) return false;
            allowed = "1" == svals[5];
            var active = "1" == svals[6];
            return active;
        }
    }
}