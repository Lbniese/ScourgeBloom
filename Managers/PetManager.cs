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
using Bots.BGBuddy.Helpers;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.TreeSharp;
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

        private static WoWGuid _lastPetAttack = WoWGuid.Empty;
        private static readonly WaitTimer WaitNextPetAttack = new WaitTimer(TimeSpan.FromSeconds(3));

        static PetManager()
        {
            // NOTE: This is a bit hackish. This fires VERY OFTEN in major cities. But should prevent us from summoning right after dismounting.
            // Lua.Events.AttachEvent("COMPANION_UPDATE", (s, e) => CallPetTimer.Reset());
            // Note: To be changed to OnDismount with new release
            Mount.OnDismount += (s, e) =>
            {
                if (StyxWoW.Me.Class == WoWClass.Hunter || StyxWoW.Me.Class == WoWClass.Warlock ||
                    StyxWoW.Me.PetNumber > 0)
                {
                    PetSummonAfterDismountTimer.Reset();
                }
            };

            // force us to check initially upon load
            NeedToCheckPetTauntAutoCast = true;
            // defaulting to scalar since this is a static ctor (and we dont want to reference settings here)

            // Lets hook this event so we can disable growl
            ScourgeBloom.OnWoWContextChanged += PetManager_OnWoWContextChanged;
        }

        public static bool NeedsPetSupport { get; set; }

        public static bool HavePet => StyxWoW.Me.GotAlivePet;

        public static string WantedPet { get; set; }

        #region INIT

        public static Composite CreatePetManagerInitializeBehaviour()
        {
            NeedToCheckPetTauntAutoCast = GeneralSettings.Instance.PetAutoControlTaunt;
            return null;
        }

        #endregion

        public static string GetPetTalentTree()
        {
            return Lua.GetReturnVal<string>("return GetPetTalentTree()", 0);
        }

        internal static void Pulse()
        {
            if (!NeedsPetSupport)
                return;

            if (!Capabilities.IsPetSummonAllowed || !Capabilities.IsPetUsageAllowed)
                return;

            if (StyxWoW.Me.Mounted)
            {
                _wasMounted = true;
            }

            if (_wasMounted && !StyxWoW.Me.Mounted)
            {
                _wasMounted = false;
                PetSummonAfterDismountTimer.Reset();
            }

            if (StyxWoW.Me.GotAlivePet && StyxWoW.Me.Pet != null)
            {
                if (_petGuid != StyxWoW.Me.Pet.Guid)
                {
                    // clear any existing spells
                    PetSpells.Clear();

                    // only load spells if we have one that is non-null
                    // .. as initial load happens before Me.PetSpells is initialized and we were saving 'null' spells
                    if (StyxWoW.Me.PetSpells.Any(s => s.Spell != null || s.Action != WoWPetSpell.PetAction.None))
                    {
                        // save Pet's Guid so we don't have to repeat
                        _petGuid = StyxWoW.Me.Pet.Guid;

                        NeedToCheckPetTauntAutoCast = GeneralSettings.Instance.PetAutoControlTaunt;

                        // Cache the list. yea yea, we should just copy it, but I'd rather have shallow copies of each object, rather than a copy of the list.
                        PetSpells.AddRange(StyxWoW.Me.PetSpells);
                        PetSummonAfterDismountTimer.Reset();
                        Logging.WriteDiagnostic("---PetSpells Loaded for: {0} Pet ---", GetPetTalentTree());
                        foreach (var sp in PetSpells)
                        {
                            if (sp.Spell == null)
                                Logging.WriteDiagnostic("   {0} spell={1}  Action={0}", sp.ActionBarIndex, sp);
                            else
                                Logging.WriteDiagnostic("   {0} spell={1} #{2}", sp.ActionBarIndex, sp, sp.Spell.Id);
                        }
                        Logging.WriteDiagnostic(" ");
                    }
                }

                HandleAutoCast();
            }
            else if (_petGuid != WoWGuid.Empty)
            {
                PetSpells.Clear();
                _petGuid = WoWGuid.Empty;
            }
        }

        public static bool Attack(WoWUnit unit)
        {
            if (unit == null || StyxWoW.Me.Pet == null || !Capabilities.IsPetUsageAllowed)
                return false;

            if (StyxWoW.Me.Pet.CurrentTargetGuid != unit.Guid &&
                (_lastPetAttack != unit.Guid || WaitNextPetAttack.IsFinished))
            {
                _lastPetAttack = unit.Guid;
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteDiagnostic("PetAttack: on {0} @ {1:F1} yds", unit.SafeName(), unit.SpellDistance());
                CastPetAction("Attack", unit);
                WaitNextPetAttack.Reset();
                return true;
            }

            return false;
        }

        public static bool Passive()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_lastPetAttack != WoWGuid.Empty)
                _lastPetAttack = WoWGuid.Empty;

            if (StyxWoW.Me.Pet == null || StyxWoW.Me.Pet.CurrentTargetGuid == WoWGuid.Empty)
                return false;

            if (StyxWoW.Me.Pet.CurrentTarget == null)
                Logger.Write(Colors.DarkOrchid, "/petpassive");
            else
                Logger.Write(Colors.DarkOrchid, "/petpassive (stop attacking {0})",
                    StyxWoW.Me.Pet.CurrentTarget.SafeName());

            CastPetAction("Passive");
            return true;
        }

        public static bool CanCastPetAction(string action)
        {
            if (StyxWoW.Me.Level < 10)
                return false;

            var petAction = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (petAction == null)
                return false;

            if (petAction.Cooldown)
                return false;

            if (petAction.Spell != null)
                return petAction.Spell.CanCast;

            return
                Lua.GetReturnVal<bool>(
                    string.Format("return GetPetActionSlotUsable({0})", petAction.ActionBarIndex + 1), 0);
        }

        public static void CastPetAction(string action)
        {
            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.Write(Colors.DeepSkyBlue, "*Pet:{0}", action);
            Lua.DoString("CastPetAction({0})", spell.ActionBarIndex + 1);
        }

        public static void CastPetAction(string action, WoWUnit on)
        {
            var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.Write(Colors.DeepSkyBlue, "*Pet:{0} on {1} @ {2:F1} yds", action, on.SafeName(), on.SpellDistance());
            if (on.Guid == StyxWoW.Me.CurrentTargetGuid)
            {
                Logging.WriteDiagnostic("CastPetAction: cast [{0}] specifying CurrentTarget", action);
                Lua.DoString("CastPetAction({0}, 'target')", spell.ActionBarIndex + 1);
            }
            else
            {
                var save = StyxWoW.Me.FocusedUnit;
                StyxWoW.Me.SetFocus(on);
                Logging.WriteDiagnostic("CastPetAction: cast [{0}] specifying FocusTarget {1}", action, on.SafeName());
                Lua.DoString("CastPetAction({0}, 'focus')", spell.ActionBarIndex + 1);
                StyxWoW.Me.SetFocus(save == null ? WoWGuid.Empty : save.Guid);
            }
        }


        //public static void EnableActionAutocast(string action)
        //{
        //    var spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
        //    if (spell == null)
        //        return;

        //    var index = spell.ActionBarIndex + 1;
        //    Logger.Write("[Pet] Enabling autocast for {0}", action, index);
        //    Lua.DoString("local index = " + index + " if not select(6, GetPetActionInfo(index)) then TogglePetAutocast(index) end");
        //}

        /// <summary>
        ///     Calls a pet by name, if applicable.
        /// </summary>
        /// <remarks>
        ///     Created 2/7/2011.
        /// </remarks>
        /// <param name="petName">
        ///     Name of the pet. This parameter is ignored for mages. Warlocks should pass only the name of the
        ///     pet. Hunters should pass which pet (1, 2, etc)
        /// </param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool CallPet(string petName)
        {
            if (!CallPetTimer.IsFinished)
            {
                return false;
            }

            if (!Capabilities.IsPetSummonAllowed)
            {
                return false;
            }

            switch (StyxWoW.Me.Class)
            {
                case WoWClass.Warlock:
                    if (Spell.CanCastHack("Summon " + petName))
                    {
                        Logger.Write(Colors.DeepSkyBlue, "[Pet] Summon {0}", petName);
                        var result = Spell.CastPrimative("Summon " + petName);
                        return result;
                    }
                    break;

                case WoWClass.Mage:
                    if (Spell.CanCastHack("Summon Water Elemental"))
                    {
                        Logger.Write(Colors.DeepSkyBlue, "[Pet] Summon Water Elemental");
                        var result = Spell.CastPrimative("Summon Water Elemental");
                        return result;
                    }
                    break;

                case WoWClass.Hunter:
                    if (Spell.CanCastHack("Call Pet " + petName))
                    {
                        if (!StyxWoW.Me.GotAlivePet)
                        {
                            Logger.Write(Colors.DeepSkyBlue, "[Pet] Call Pet #{0}", petName);
                            var result = Spell.CastPrimative("Call Pet " + petName);
                            return result;
                        }
                    }
                    break;
            }
            return false;
        }

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
            if (ps != null)
            {
                // action bar index base 0 in HB but base 1 in LUA, so adjust
                var svals = Lua.GetReturnValues("return GetPetActionInfo(" + (ps.ActionBarIndex + 1) + ");");
                if (svals != null && svals.Count >= 7)
                {
                    allowed = "1" == svals[5];
                    var active = "1" == svals[6];
                    return active;
                }
            }

            return false;
        }

        #region Manage Growl for Instances

        // flag used to indicate need to check; set anywhere but handled within Pulse()
        private static bool NeedToCheckPetTauntAutoCast { get; set; }

        // set needtocheck flag anytime context changes
        private static void PetManager_OnWoWContextChanged(object sender, WoWContextEventArg e)
        {
            NeedToCheckPetTauntAutoCast = GeneralSettings.Instance.PetAutoControlTaunt;
        }

        public static void HandleAutoCast()
        {
            if (NeedToCheckPetTauntAutoCast)
            {
                if (StyxWoW.Me.GotAlivePet)
                {
                    if (!IsAnySpellOnPetToolbar("Growl", "Taunt", "Thunderstomp", "Suffering", "Threatening Presence"))
                    {
                        NeedToCheckPetTauntAutoCast = false;
                    }
                }
            }
        }

        public static bool IsAnySpellOnPetToolbar(params string[] spells)
        {
            if (!StyxWoW.Me.GotAlivePet)
                return false;

            if (spells == null || !spells.Any())
                return false;

            if (StyxWoW.Me.PetSpells == null)
                return false;

            var taunt = new HashSet<string>(spells);
            var ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.Spell != null && taunt.Contains(s.Spell.Name));

            return ps != null;
        }

        public static bool CanWeCheckAutoCastForAnyOfThese(params string[] spells)
        {
            if (!StyxWoW.Me.GotAlivePet)
                return false;

            if (spells == null || !spells.Any())
                return false;

            if (StyxWoW.Me.PetSpells == null)
                return false;

            var taunt = new HashSet<string>(spells);
            var ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.Spell != null && taunt.Contains(s.Spell.Name));

            if (ps == null)
                return false;

            bool allowed;
            IsAutoCast(ps, out allowed);
            if (!allowed)
                return false;

            return true;
        }

        public static bool HandleAutoCastForSpell(string spellName)
        {
            var ps = StyxWoW.Me.PetSpells.FirstOrDefault(s => s.ToString() == spellName);

            // Disable pet growl in instances but enable it outside.
            if (ps == null)
                Logging.WriteDiagnostic("PetManager: '{0}' is NOT an ability known by this Pet", spellName);
            else
            {
                bool allowed;
                var active = IsAutoCast(ps, out allowed);
                if (!allowed)
                    Logger.Write(Colors.DarkOrchid, "PetManager: '{0}' is NOT an auto-cast ability for this Pet",
                        spellName);
                else
                {
                    if (ScourgeBloom.CurrentWoWContext == WoWContext.Instances)
                    {
                        if (!active)
                            Logger.Write(Colors.DarkOrchid, "PetManager: '{0}' Auto-Cast Already Disabled", spellName);
                        else
                        {
                            Logger.Write(Colors.DarkOrchid, "PetManager: Disabling '{0}' Auto-Cast", spellName);
                            Lua.DoString("DisableSpellAutocast(GetSpellInfo(" + ps.Spell.Id + "))");
                        }
                    }
                    else
                    {
                        if (active)
                            Logger.Write(Colors.DarkOrchid, "PetManager: '{0}' Auto-Cast Already Enabled", spellName);
                        else
                        {
                            Logger.Write(Colors.DarkOrchid, "PetManager: Enabling '{0}' Auto-Cast", spellName);
                            Lua.DoString("EnableSpellAutocast(GetSpellInfo(" + ps.Spell.Id + "))");
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}