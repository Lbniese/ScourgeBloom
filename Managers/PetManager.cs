using System;
using System.Collections.Generic;
using System.Linq;
using ScourgeBloom.Helpers;
using Styx;
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
                if (StyxWoW.Me.Class == WoWClass.DeathKnight && StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy ||
                    StyxWoW.Me.PetNumber > 0)
                {
                    PetSummonAfterDismountTimer.Reset();
                }
            };

            // force us to check initially upon load

        }

        public static bool HavePet => StyxWoW.Me.GotAlivePet;

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
                    // clear any existing spells
                    PetSpells.Clear();

                    // only load spells if we have one that is non-null
                    // .. as initial load happens before Me.PetSpells is initialized and we were saving 'null' spells
                    if (StyxWoW.Me.PetSpells.Any(s => s.Spell != null))
                    {

                        // Cache the list. yea yea, we should just copy it, but I'd rather have shallow copies of each object, rather than a copy of the list.
                        PetSpells.AddRange(StyxWoW.Me.PetSpells);
                        PetSummonAfterDismountTimer.Reset();
                        _petGuid = StyxWoW.Me.Pet.Guid;

                        Log.WriteLog(Styx.Common.LogLevel.Diagnostic, "---PetSpells Loaded---");
                        foreach (var sp in PetSpells)
                        {
                            if (sp.Spell == null)
                                Log.WriteLog(Styx.Common.LogLevel.Diagnostic, string.Format("   {0} spell={1}  Action={0}", sp.ActionBarIndex, sp.ToString(), sp.Action.ToString()));
                            else
                                Log.WriteLog(Styx.Common.LogLevel.Diagnostic, string.Format("   {0} spell={1} #{2}", sp.ActionBarIndex, sp.ToString(), sp.Spell.Id));
                        }
                        Log.WriteLog(Styx.Common.LogLevel.Diagnostic, " ");
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
            WoWPetSpell petAction = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (petAction == null || petAction.Spell == null)
            {
                return false;
            }

            return !petAction.Spell.Cooldown;
        }

        public static void CastPetAction(string action)
        {
            WoWPetSpell spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Log.WriteLog(string.Format("[Pet] Casting {0}", action));
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

            WoWPetSpell spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Log.WriteLog(string.Format("[Pet] Casting {0} on {1}", action, on.SafeName()));
            WoWUnit save = StyxWoW.Me.FocusedUnit;
            StyxWoW.Me.SetFocus(on);
            Lua.DoString("CastPetAction({0}, 'focus')", spell.ActionBarIndex + 1);
            StyxWoW.Me.SetFocus(save == null ? WoWGuid.Empty : save.Guid);
        }

        /// <summary>
        /// behavior form of CastPetAction().  note that this Composite will return RunStatus.Success
        /// if it appears the ability was cast.  this is to trip the Throttle wrapping it internally
        /// -and- to allow cascaded sequences of Pet Abilities.  Note: Pet Abilities are not on the
        /// GCD, so you can safely allow execution to continue even on Success
        /// </summary>
        /// <param name="action">pet ability</param>
        /// <param name="onUnit">unit deleg to cast on (null if current target)</param>
        /// <returns></returns>

        public static void EnableActionAutocast(string action)
        {
            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            var index = spell.ActionBarIndex + 1;
            Log.WriteLog(string.Format("[Pet] Enabling autocast for {0}", action, index));
            Lua.DoString("local index = " + index + " if not select(6, GetPetActionInfo(index)) then TogglePetAutocast(index) end");
        }

        /// <summary>
        ///   Calls a pet by name, if applicable.
        /// </summary>
        /// <remarks>
        ///   Created 2/7/2011.
        /// </remarks>
        /// <param name = "petName">Name of the pet. This parameter is ignored for mages. Warlocks should pass only the name of the pet. Hunters should pass which pet (1, 2, etc)</param>
        /// <returns>true if it succeeds, false if it fails.</returns>

        public static bool IsAutoCast(int id, out bool allowed)
        {
            WoWPetSpell ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.Spell != null && s.Spell.Id == id);
            return IsAutoCast(ps, out allowed);
        }

        public static bool IsAutoCast(string action, out bool allowed)
        {
            WoWPetSpell ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.ToString() == action);
            return IsAutoCast(ps, out allowed);
        }

        public static bool IsAutoCast(WoWPetSpell ps, out bool allowed)
        {
            allowed = false;
            if (ps != null)
            {
                // action bar index base 0 in HB but base 1 in LUA, so adjust
                List<string> svals = Lua.GetReturnValues("return GetPetActionInfo(" + (ps.ActionBarIndex + 1) + ");");
                if (svals != null && svals.Count >= 7)
                {
                    allowed = ("1" == svals[5]);
                    bool active = ("1" == svals[6]);
                    return active;
                }
            }

            return false;
        }

    }
}