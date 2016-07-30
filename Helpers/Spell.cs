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
using System.Threading.Tasks;
using System.Windows.Media;
using Bots.DungeonBuddy.Helpers;
using Buddy.Coroutines;
using JetBrains.Annotations;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    [UsedImplicitly]
    public static class Spell
    {
        public static readonly LocalPlayer Me = StyxWoW.Me;
        public static WoWUnit LastCastTarget;
        public static string LastSpellCast { get; set; }
        public static WoWGuid LastSpellTarget { get; set; }

        public enum LagTolerance
        {
            No = 0,
            Yes
        }

        #region Fix HonorBuddys GCD Handling

#if HONORBUDDY_GCD_IS_WORKING
#else

        private static WoWSpell _gcdCheck;

        public static string FixGlobalCooldownCheckSpell
        {
            get { return _gcdCheck == null ? null : _gcdCheck.Name; }
            set
            {
                SpellFindResults sfr;
                if (!SpellManager.FindSpell(value, out sfr))
                {
                    _gcdCheck = null;
                    Logger.Write("GCD check fix spell {0} not known", value);
                }
                else
                {
                    _gcdCheck = sfr.Original;
                    Logger.Write("GCD check fix spell set to: {0}", value);
                }
            }
        }

#endif

        public static bool GcdActive
        {
            get
            {
#if HONORBUDDY_GCD_IS_WORKING
                return SpellManager.GlobalCooldown;
#else
                if (_gcdCheck == null)
                    return SpellManager.GlobalCooldown;

                return _gcdCheck.Cooldown;
#endif
            }
        }

        public static TimeSpan GcdTimeLeft
        {
            get
            {
#if HONORBUDDY_GCD_IS_WORKING
                return SpellManager.GlobalCooldownLeft;
#else
                try
                {
                    if (_gcdCheck != null)
                        return _gcdCheck.CooldownTimeLeft;
                }
                catch (AccessViolationException)
                {
                    Logging.WriteToFile(LogLevel.Normal,
                        "GcdTimeLeft: handled access exception, reinitializing gcd spell");
                    GcdInitialize();
                }
                catch (InvalidObjectPointerException)
                {
                    Logging.WriteToFile(LogLevel.Normal,
                        "GcdTimeLeft: handled invobj exception, reinitializing gcd spell");
                    GcdInitialize();
                }

                // use default value here (reinit should fix _gcdCheck for next call)
                return SpellManager.GlobalCooldownLeft;
#endif
            }
        }

        public static void GcdInitialize()
        {
#if HONORBUDDY_GCD_IS_WORKING
            Logger.WriteDebug("GcdInitialize: using HonorBuddy GCD");
#else
            Logger.WriteDebug("GcdInitialize: using ScourgeBloom GCD");
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    FixGlobalCooldownCheckSpell = "Frost Presence";
                    break;
            }

            if (FixGlobalCooldownCheckSpell != null)
                return;

            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    // FixGlobalCooldownCheckSpell = "";
                    break;
            }
#endif
        }

        #endregion

        private static Dictionary<string, long> UndefinedSpells { get; [UsedImplicitly] set; }

        public static bool CastPrimative(string spellName)
        {
            LastSpellTarget = WoWGuid.Empty;
            return SpellManager.Cast(spellName);
        }

        public static bool CastPrimative(int id)
        {
            LastSpellTarget = WoWGuid.Empty;
            return SpellManager.Cast(id);
        }

        public static bool CastPrimative(WoWSpell spell)
        {
            LastSpellTarget = WoWGuid.Empty;
            return SpellManager.Cast(spell);
        }

        public static bool CastPrimative(string spellName, WoWUnit unit)
        {
            LastSpellTarget = unit == null ? WoWGuid.Empty : unit.Guid;
            return SpellManager.Cast(spellName, unit);
        }

        public static bool CastPrimative(int id, WoWUnit unit)
        {
            LastSpellTarget = unit == null ? WoWGuid.Empty : unit.Guid;
            return SpellManager.Cast(id, unit);
        }

        public static bool CastPrimative(WoWSpell spell, WoWUnit unit)
        {
            LastSpellTarget = unit == null ? WoWGuid.Empty : unit.Guid;
            return SpellManager.Cast(spell, unit);
        }

        /// <summary>
        ///     get the effective distance between two mobs accounting for their
        ///     combat reaches (hitboxes)
        /// </summary>
        /// <param name="unitOrigin">
        ///     toon originating spell/ability.  If no destination specified then assume 'Me' originates and
        ///     'unit' is the target
        /// </param>
        /// <param name="unitTarget">target of spell.  if null, assume 'unit' is target of spell cast by 'Me'</param>
        /// <returns>normalized attack distance</returns>
        public static float SpellDistance(this WoWUnit unitOrigin, WoWUnit unitTarget = null)
        {
            // abort if mob null
            if (unitOrigin == null)
                return 0;

            // when called as SomeUnit.SpellDistance()
            // .. convert to SomeUnit.SpellDistance(Me)
            if (unitTarget == null)
                unitTarget = StyxWoW.Me;

            // when called as SomeUnit.SpellDistance(Me) then
            // .. convert to Me.SpellDistance(SomeUnit)
            if (unitTarget.IsMe)
            {
                unitTarget = unitOrigin;
                unitOrigin = StyxWoW.Me;
            }

            // only use CombatReach of destination target
            var dist = unitTarget.Location.Distance(unitOrigin.Location) - unitTarget.CombatReach;
            return Math.Max(0, dist);
        }

        #region CastOnGround

        public static async Task<bool> CastOnGround(int spell, WoWUnit unit, bool reqs)
        {
            try
            {
                var sp = WoWSpell.FromId(spell);
                var sname = sp != null ? sp.Name : "#" + spell;

                //Log.WriteLog(LogLevel.Normal, $"Casting {sname}");

                if (!reqs || !SpellManager.CanCast(spell) || unit == null)
                    return false;

                var onLocation = unit.Location;

                if (!SpellManager.Cast(spell))
                    return false;

                if (!await Coroutine.Wait(1000, () => StyxWoW.Me.CurrentPendingCursorSpell != null))
                {
                    Logging.Write(Colors.DarkRed, "[ScourgeBloom] Cursor Spell did not happen");
                    return false;
                }

                SpellManager.ClickRemoteLocation(onLocation);
                Log.WriteLog(LogLevel.Diagnostic, $"Casting {sname}", Colors.DarkGreen);
                await CommonCoroutines.SleepForLagDuration();
                return true;
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic("[ScourgeBloom] CastOnGround: {0} {1}", spell, e);
            }
            return false;
        }

        #endregion CastOnGround

        #region CoCast

        public static async Task<bool> CoCast(int spell, WoWUnit unit, bool reqs, bool cancel)
        {
            try
            {
                var sp = WoWSpell.FromId(spell);
                var sname = sp != null ? sp.Name : "#" + spell;

                //Log.WriteLog(LogLevel.Normal, $"Casting {sname}");

                if (unit == null || !reqs || !SpellManager.CanCast(spell, unit, true))
                    return false;

                if (!SpellManager.Cast(spell, unit))
                    return false;

                // if (!await Coroutine.Wait(GetSpellCastTime(sname), () => cancel) &&
                // GetSpellCastTime(sname).TotalSeconds > 0)
                // {
                // SpellManager.StopCasting();
                // Log.WriteLog("[ScourgeBloom] Canceling " + sname + ".");
                // return false;
                // }

                Log.WriteLog(LogLevel.Diagnostic, $"Casting {sname}", Colors.DarkGreen);
                await CommonCoroutines.SleepForLagDuration();
                return true;
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic("[ScourgeBloom] Cast: {0} {1}", spell, e);
            }
            return false;
        }

        #endregion CoCast

        #region CanCast

        public static CanCastResult CanCast(string strspell, WoWUnit unit, bool ignoregcd)
        {
            if (!Me.IsAlive || Me.IsDead || Me.IsGhost || Me.CurrentHealth == 0)
                return CanCastResult.Dead;

            if (SpellManager.GlobalCooldown && ignoregcd == false)
                return CanCastResult.Gcd;

            SpellFindResults results;
            if (!SpellManager.FindSpell(strspell, out results))
                return CanCastResult.NoSpell;

            var spell = results.Override ?? results.Original;

            if (unit == null || !unit.IsValid)
                return CanCastResult.InvalidTarget;

            if (SpellHistoryContainsKey(strspell, unit.Guid))
                return CanCastResult.Dcp;

            if (Me.IsMoving && !spell.IsMeleeSpell && spell.CastTime != 0 && !IsChanneled(strspell) &&
                spell.Name != "Steady Shot")
                return CanCastResult.Moving;
            if (Me.IsMoving && !spell.IsMeleeSpell && spell.CastTime != 0 && !IsChanneled(strspell) &&
                spell.Name != "Cobra Shot" && !IsChanneled(strspell) && spell.Name != "Aimed Shot" &&
                !IsChanneled(strspell) && spell.Name != "Steady Shot" && !IsChanneled(strspell) &&
                spell.Name != "Barrage" && !IsChanneled(strspell) && spell.Name != "Powershot")
                return CanCastResult.Moving;

            if (Me.IsCasting && !Me.IsChanneling)
                return CanCastResult.Casting;

            if (Me.ChanneledSpell != null)
            {
                //if there is no cooldown to the spell ignore this
                if (spell.BaseCooldown >= 3000)
                    return CanCastResult.Channeling;
            }

            if (spell.Cooldown)
                return CanCastResult.CoolDown;

            if ((!unit.InLineOfSight && unit.IsWithinMeleeRange) ||
                (!unit.InLineOfSpellSight && !unit.IsWithinMeleeRange))
                return CanCastResult.Los;

            if (Me.Mounted && !GeneralSettings.Instance.AutoDismount)
                return CanCastResult.Mounted;

            if (spell.HasRange)
            {
                if (unit.Distance >= spell.MaxRange + unit.CombatReach)
                    return CanCastResult.Range;
                if (unit.Distance <= spell.MinRange - unit.CombatReach)
                    return CanCastResult.Range;
            }
            else
            {
                if (!unit.IsWithinMeleeRange)
                    return CanCastResult.Range;
            }

            if (Me.HasAura("Bladestorm"))
                return CanCastResult.Channeling;

            return Me.HasAura("Drink") ? CanCastResult.Drinking : CanCastResult.Success;
        }

        #endregion CanCast

        public static int GetBaseCooldown(WoWSpell spell)
        {
            var cd = Lua.GetReturnVal<int>("return GetSpellBaseCooldown(" + spell.Id + ")", 0);
            return cd;
        }

        private static void AddUndefinedSpell(string s)
        {
            if (!GeneralSettings.Instance.Debug)
                return;

            if (UndefinedSpells.ContainsKey(s))
                UndefinedSpells[s] = UndefinedSpells[s] + 1;
            else
                UndefinedSpells.Add(s, 1);
        }

        #region Cast Hack - allows casting spells that CanCast returns False

        public static bool CanCastHack(string castName)
        {
            return CanCastHack(castName, Me.CurrentTarget);
        }

        /// <summary>
        ///     CastHack following done because CanCast() wants spell as "Metamorphosis: Doom" while Cast() and aura name are
        ///     "Doom"
        /// </summary>
        /// <param name="castName"></param>
        /// <param name="unit"></param>
        /// <param name="skipWowCheck"></param>
        /// <returns></returns>
        public static bool CanCastHack(string castName, WoWUnit unit, bool skipWowCheck = false)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(castName, out sfr))
            {
                // Logger.WriteDebug("CanCast: spell [{0}] not known", castName);
                AddUndefinedSpell(castName);
                return false;
            }

            return CanCastHack(sfr, unit, skipWowCheck);
        }

        /// <summary>
        ///     CastHack following done because CanCast() wants spell as "Metamorphosis: Doom" while Cast() and aura name are
        ///     "Doom"
        /// </summary>
        /// <returns>true: if spell can be cast, false: if a condition prevents it</returns>
        public static bool CanCastHack(SpellFindResults sfr, WoWUnit unit, bool skipWowCheck = false)
        {
            var spell = sfr.Override ?? sfr.Original;

            // check range
            if (!CanCastHackInRange(spell, unit))
                return false;

            // check if movement prevents cast
            if (CanCastHackWillOurMovementInterrupt(spell, unit))
                return false;

            if (CanCastHackIsCastInProgress(spell, unit))
                return false;

            if (!CanCastHackHaveEnoughPower(spell, unit))
                return false;

            if (GeneralSettings.Instance.DisableSpellsWithCooldown != 0)
            {
                var baseCooldown = GetBaseCooldown(spell);
                if (baseCooldown >= GeneralSettings.Instance.DisableSpellsWithCooldown*1000)
                {
                    if (GeneralSettings.Instance.Debug)
                        Logging.WriteToFile(LogLevel.Normal,
                            "CanCast[{0}]: basecooldown of {0} exceeds max allowed user setting of {1} ms", baseCooldown,
                            GeneralSettings.Instance.DisableSpellsWithCooldown*1000);
                    return false;
                }
            }

            // override spell will sometimes always have cancast=false, so check original also
            if (!skipWowCheck && !spell.CanCast && (sfr.Override == null || !sfr.Original.CanCast))
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: spell specific CanCast failed (#{1})",
                        spell.Name, spell.Id);

                return false;
            }

            return true;
        }

        public static bool CanCastHackHaveEnoughPower(WoWSpell spell, WoWUnit unit)
        {
#if USE_LUA_POWERCHECK
            string usablecmd = string.Format("return IsUsableSpell(\"{0}\")", spell.Name);
            List<string> ret = Lua.GetReturnValues(usablecmd);
            if ( ret == null || !ret.Any())
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: IsUsable check failed with null", spell.Name);
                return false;
            }

            if (ret[0] != "1")
            {
                if (GeneralSettings.Instance.Debug)
                {
                    uint currentPower = Me.CurrentPower;
                    string ptype = Me.PowerType.ToString();

                    if (Me.Class == WoWClass.Druid)
                    {
                        if (Me.Shapeshift == ShapeshiftForm.Cat || Me.Shapeshift == ShapeshiftForm.Bear || Me.Shapeshift == ShapeshiftForm.DireBear)
                        {
                            if (Me.HealingSpellIds.Contains(spell.Id))
                            {
                                ptype = "Mana";
                                currentPower = Me.CurrentMana;
                            }
                            else if (spell.PowerCost >= 100)
                            {
                                ptype = "Mana";
                                currentPower = Me.CurrentMana;
                            }
                        }
                    }
                    
                    if (ret.Count() > 1 && ret[1] == "1")
                        Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: insufficient power ({1} cost={2} have={3})", spell.Name, ptype, spell.PowerCost, currentPower);
                    else
                        Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: not usable atm ({1} cost={2} have={3})", spell.Name, ptype, spell.PowerCost, currentPower);
                }

                return false;
            }

#elif PRE_WOD
            bool formSwitch = false;
            uint currentPower = Me.CurrentPower;

            if (currentPower < (uint)spell.PowerCost)
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: insufficient power (need {1:F0}, have {2:F0} {3})", spell.Name, spell.PowerCost, currentPower, formSwitch ? "Mana in Form" : Me.PowerType.ToString());
                return false;
            }
#else
            if (!spell.CanCast)
            {
                return false;
            }
#endif
            return true;
        }

        public static bool CanCastHackIsCastInProgress(WoWSpell spell, WoWUnit unit)
        {
            var lat = ScourgeBloom.Latency*2u;

            if (Me.ChanneledCastingSpellId == 0)
            {
                if (StyxWoW.Me.IsCasting && Me.CurrentCastTimeLeft.TotalMilliseconds > lat)
                {
                    if (GeneralSettings.Instance.Debug)
                        Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: current cast of {1} has {2:F0} ms left",
                            spell.Name, Me.CurrentCastId, Me.CurrentCastTimeLeft.TotalMilliseconds - lat);
                    return true;
                }
            }

            if (spell.CooldownTimeLeft.TotalMilliseconds > lat)
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: still on cooldown for {1:F0} ms", spell.Name,
                        spell.CooldownTimeLeft.TotalMilliseconds - lat);
                return true;
            }

            return false;
        }

        public static bool CanCastHackInRange(WoWSpell spell, WoWUnit unit)
        {
            if (unit != null && !spell.IsSelfOnlySpell && !unit.IsMe)
            {
#if USE_LUA_RANGECHECK
    // will exercise the IsSpellInRange LUA if it can easily
    // .. derive a UnitID for the target

                string sTarget = null;

                if (unit.Guid == Me.CurrentTargetGuid)
                    sTarget = "target";
                else if (unit.IsPlayer && unit.ToPlayer().IsInMyPartyOrRaid())
                    sTarget = unit.Name;
                else if (unit.IsPet && unit.OwnedByUnit != null && unit.OwnedByUnit.IsPlayer && unit.OwnedByUnit.ToPlayer().IsInMyPartyOrRaid())
                    sTarget = unit.OwnedByUnit.Name + "-pet";
                else if (Me.GotAlivePet)
                {
                    if (unit.Guid == Me.Pet.Guid)
                        sTarget = "pet";
                    else if (unit.Guid == Me.Pet.CurrentTargetGuid)
                        sTarget = "pettarget";
                }

                if (sTarget != null)
                {
                    // 
                    string lua = string.Format("return IsSpellInRange(\"{0}\",\"{1}\")", spell.Name, sTarget);
                    string inRange = Lua.GetReturnVal<string>(lua, 0);
                    if (inRange != "1")
                    {
                        if (SingularSettings.DebugSpellCanCast)
                            Logging.WriteToFile( "CanCast[{0}]: target @ {1:F1} yds failed IsSpellInRange() = {2}", spell.Name, unit.Distance, inRange);
                        return false;
                    }
                }
                else 
#endif
                {
                    if (spell.IsMeleeSpell && !unit.IsWithinMeleeRange)
                    {
                        if (GeneralSettings.Instance.Debug)
                            Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: target @ {1:F1} yds not in melee range",
                                spell.Name, unit.Distance);
                        return false;
                    }
                    if (spell.HasRange)
                    {
                        if (unit.Distance > spell.ActualMaxRange(unit))
                        {
                            if (GeneralSettings.Instance.Debug)
                                Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: out of range - further than {1:F1}",
                                    spell.Name, spell.ActualMaxRange(unit));
                            return false;
                        }
                        if (unit.Distance < spell.ActualMinRange(unit))
                        {
                            if (GeneralSettings.Instance.Debug)
                                Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: out of range - closer than {1:F1}",
                                    spell.Name, spell.ActualMinRange(unit));
                            return false;
                        }
                    }
                }

                if (!unit.InLineOfSpellSight)
                {
                    if (GeneralSettings.Instance.Debug)
                        Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: not in spell line of {1}", spell.Name,
                            unit.SafeName());
                    return false;
                }
            }

            return true;
        }

        public static bool IsInstantCast(this WoWSpell spell)
        {
            return spell.CastTime == 0;
            //return spell.CastTime == 0 && spell.BaseDuration == 0
            //    || spell.Id == 101546   /* Spinning Crane Kick */
            //    || spell.Id == 46924    /* Bladestorm */
            //    ;
        }

        /// <summary>
        ///     check for aura which allows moving without interrupting spell casting
        /// </summary>
        /// <returns></returns>
        public static bool HaveAllowMovingWhileCastingAura(WoWSpell spell = null)
        {
            var found =
                Me.GetAllAuras()
                    .FirstOrDefault(
                        a =>
                            a.ApplyAuraType == (WoWApplyAuraType) 330 &&
                            (spell == null || GetSpellCastTime(spell) < a.TimeLeft));

            if (GeneralSettings.Instance.Debug && found != null)
                Logging.WriteToFile(LogLevel.Normal,
                    "MoveWhileCasting[{0}]: true, since we found move buff {1} #{2}",
                    spell == null ? "(null)" : spell.Name,
                    found.Name,
                    found.SpellId
                    );

            return found != null;
        }


        /// <summary>
        ///     will cast class/specialization specific buff to allow moving without interrupting casting
        /// </summary>
        /// <returns>true if able to cast, false otherwise</returns>
        private static bool CastBuffToAllowCastingWhileMoving()
        {
            string spell = null;
            var allowMovingWhileCasting = false;

            if (GeneralSettings.Instance.UseCastWhileMovingBuffs)
            {
                if (Me.Class == WoWClass.Shaman)
                    spell = "Spiritwalker's Grace";
                else if (Me.Class == WoWClass.Mage)
                    spell = "Ice Floes";

                if (spell != null)
                {
                    if (DoubleCastContains(Me, spell))
                        return false;

                    // DumpDoubleCast();

                    if (CanCastHack(spell, Me)) // Spell.CanCastHack(spell, Me))
                    {
                        LogCast(spell, Me);
                        allowMovingWhileCasting = CastPrimative(spell, Me);
                        if (!allowMovingWhileCasting)
                            Logging.WriteDiagnostic("CastBuffToAllowCastingWhileMoving: spell cast failed - [{0}]",
                                spell);
                        else
                            UpdateDoubleCast(spell, Me, 1);
                    }
                }
            }

            return allowMovingWhileCasting;
        }

        public static void LogCast(string sname, WoWUnit unit, bool isHeal = false)
        {
            LogCast(sname, unit, unit.HealthPercent, unit.SpellDistance(), isHeal);
        }

        public static void LogCast(string sname, WoWUnit unit, double health, double dist, bool isHeal = false)
        {
            Color clr;

            if (isHeal)
                clr = Log.LogColor.SpellHeal;
            else
                clr = Log.LogColor.SpellNonHeal;

            if (unit.IsMe)
                Logger.Write(clr, "*{0} on Me @ {1:F1}%", sname, health);
            else
                Logger.Write(clr, "*{0} on {1} @ {2:F1}% at {3:F1} yds", sname, unit.SafeName(), health, dist);
        }

        /// <summary>
        ///     checked if the spell has an instant cast, the spell is one which can be cast while moving, or we have an aura
        ///     active which allows moving without interrupting casting.
        ///     does not check whether you are presently moving, only whether you could cast if you are moving
        /// </summary>
        /// <param name="spell">spell to cast</param>
        /// <returns>true if spell can be cast while moving, false if it cannot</returns>
        private static bool AllowMovingWhileCasting(WoWSpell spell)
        {
            // quick return for instant spells
            if (spell.IsInstantCast() && !spell.IsChanneled)
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal,
                        "MoveWhileCasting[{0}]: true, since instant cast and not a funnel spell", spell.Name);
                return true;
            }

            // assume we cant move, but check for class specific buffs which allow movement while casting
            var allowMovingWhileCasting = false;
            //            if (!allowMovingWhileCasting && Me.ZoneId == 5723)
            //                allowMovingWhileCasting = Me.HasAura("Molten Feather");

            if (!allowMovingWhileCasting)
            {
                allowMovingWhileCasting = HaveAllowMovingWhileCastingAura(spell);

                // we will atleast check spell cooldown... we may still end up wasting buff, but this reduces the chance
                if (!allowMovingWhileCasting && spell.CooldownTimeLeft == TimeSpan.Zero)
                {
                    var castSuccess = CastBuffToAllowCastingWhileMoving();
                    if (castSuccess)
                        allowMovingWhileCasting = HaveAllowMovingWhileCastingAura();
                }
            }

            return allowMovingWhileCasting;
        }

        public static bool CanCastHackWillOurMovementInterrupt(WoWSpell spell, WoWUnit unit)
        {
            if ((spell.CastTime != 0u || spell.IsChanneled) && Me.IsMoving && !AllowMovingWhileCasting(spell))
            {
                if (GeneralSettings.Instance.Debug)
                    Logging.WriteToFile(LogLevel.Normal, "CanCast[{0}]: cannot cast while moving", spell.Name);
                return true;
            }

            return false;
        }

        #region Buff DoubleCast prevention mechanics

        public static void UpdateDoubleCast(string spellName, WoWUnit unit, int milliSecs = 3000)
        {
            if (unit == null)
                return;

            var expir = DateTime.UtcNow + TimeSpan.FromMilliseconds(milliSecs);
            var key = DoubleCastKey(unit.Guid, spellName);
            if (DoubleCastPreventionDict.ContainsKey(key))
                DoubleCastPreventionDict[key] = expir;
            else
                DoubleCastPreventionDict.Add(key, expir);
        }


        public static void MaintainDoubleCast()
        {
            DoubleCastPreventionDict.RemoveAll(t => DateTime.UtcNow > t);
        }


        public static bool DoubleCastContains(WoWUnit unit, string spellName)
        {
            return DoubleCastPreventionDict.ContainsKey(DoubleCastKey(unit, spellName));
        }

        public static bool DoubleCastContainsAny(WoWUnit unit, params string[] spellNames)
        {
            return spellNames.Any(s => DoubleCastPreventionDict.ContainsKey(DoubleCastKey(unit, s)));
        }

        public static bool DoubleCastContainsAll(WoWUnit unit, params string[] spellNames)
        {
            return spellNames.All(s => DoubleCastPreventionDict.ContainsKey(DoubleCastKey(unit, s)));
        }

        public static void DumpDoubleCast()
        {
            var count = DoubleCastPreventionDict.Count();
            Logger.WriteDebug("DumpDoubleCast: === {0} @ {1:HH:mm:ss.fff}", count, DateTime.UtcNow);
            foreach (var entry in DoubleCastPreventionDict)
            {
                var expires = entry.Value;
                var index = entry.Key.IndexOf('-');
                var guidString = entry.Key.Substring(0, index);
                var spellName = entry.Key.Substring(index + 1);
                WoWGuid guid;
                WoWGuid.TryParseFriendly(guidString, out guid);
                var unit = ObjectManager.GetObjectByGuid<WoWUnit>(guid);
                var safeName = unit == null ? guidString : unit.SafeName();
                Logger.WriteDebug("   {0} {1:HH:mm:ss.fff} {2}", safeName.AlignLeft(25), expires, spellName);
            }
            if (count > 0)
                Logger.WriteDebug("DumpDoubleCast: =======================");
        }

        private static string DoubleCastKey(WoWGuid guid, string spellName)
        {
            return guid + "-" + spellName;
        }

        private static string DoubleCastKey(WoWUnit unit, string spell)
        {
            return DoubleCastKey(unit.Guid, spell);
        }

        private static readonly Dictionary<string, DateTime> DoubleCastPreventionDict =
            new Dictionary<string, DateTime>();

        #endregion

        #endregion

        #region CoCast Wrappers

        public static async Task<bool> CoCast(int spell)
        {
            return await CoCast(spell, Me.CurrentTarget, true, false);
        }

        public static async Task<bool> CoCast(int spell, WoWUnit unit)
        {
            return await CoCast(spell, unit, true, false);
        }

        public static async Task<bool> CoCast(int spell, bool reqs)
        {
            return await CoCast(spell, Me.CurrentTarget, reqs, false);
        }

        public static async Task<bool> CoCast(int spell, WoWUnit unit, bool reqs)
        {
            return await CoCast(spell, unit, reqs, false);
        }

        #endregion CoCast Wrappers

        #region Enums

        public enum CanCastResult
        {
            InvalidTarget,
            Gcd,
            NoSpell,
            Dcp,
            Moving,
            Casting,
            CoolDown,
            Energy,
            Los,
            Inhibited,
            Friendly,
            Range,
            Drinking,
            Dead,
            Success,
            Channeling,
            PointlessHeal,
            Mounted
        }

        public enum SpellFlags
        {
            Buff,
            Heal,
            Normal,
            FreeCast
        }

        #endregion Enums

        #region Properties

        public static TimeSpan GetSpellCastTime(WoWSpell spell)
        {
            if (spell != null)
            {
                var time = (int) spell.CastTime;
                if (time == 0)
                    time = spell.BaseDuration;
                return TimeSpan.FromMilliseconds(time);
            }

            return TimeSpan.Zero;
        }

        public static TimeSpan GetSpellCastTime(string s)
        {
            SpellFindResults sfr;
            if (SpellManager.FindSpell(s, out sfr))
            {
                var spell = sfr.Override ?? sfr.Original;
                return GetSpellCastTime(spell);
            }

            return TimeSpan.Zero;
        }

        public static TimeSpan GetCooldownLeft(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
            }
            return TimeSpan.MaxValue;
        }

        public static bool HasSpell(string spell)
        {
            SpellFindResults results;
            return SpellManager.FindSpell(spell, out results);
        }

        public static bool IsChanneled(string spell)
        {
            SpellFindResults results;
            if (!SpellManager.FindSpell(spell, out results)) return false;
            if (results.Override != null)
                return results.Override.AttributesEx == SpellAttributesEx.Channeled1 ||
                       results.Override.AttributesEx == SpellAttributesEx.Channeled2;
            return results.Original.AttributesEx == SpellAttributesEx.Channeled1 ||
                   results.Original.AttributesEx == SpellAttributesEx.Channeled2;
        }

        public static float SpellRange(string strspell, WoWUnit unit)
        {
            SpellFindResults results;
            if (!SpellManager.FindSpell(strspell, out results))
                return 0;
            var spell = results.Override ?? results.Original;
            return spell.HasRange ? spell.MaxRange : Math.Max(5, Me.CombatReach + 1.3333334f + unit.CombatReach);
        }

        public static int GetCharges(string name)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(name, out sfr)) return 0;
            var spell = sfr.Override ?? sfr.Original;
            return GetCharges(spell);
        }

        public static int GetCharges(int name)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(name, out sfr)) return 0;
            var spell = sfr.Override ?? sfr.Original;
            return GetCharges(spell);
        }

        public static bool StopCasting(Func<bool> reqs)
        {
            if (!reqs()) return false;
            if (!Me.IsChanneling && !Me.IsCasting) return true;
            SpellManager.StopCasting();
            Log.WritetoFile("[ScourgeBloom] Stopping Casting");
            return false;
        }

        public static int GetCharges(WoWSpell spell)
        {
            var charges = Lua.GetReturnVal<int>("return GetSpellCharges(" + spell.Id + ")", 0);
            return charges;
        }

        public static bool SpellHistoryContainsKey(string spell, WoWGuid unitguid)
        {
            var lcs =
                SpellHistory.FirstOrDefault(
                    s =>
                        spell == s.SpellName && s.UnitGuid.IsValid && s.UnitGuid == unitguid &&
                        DateTime.UtcNow.Subtract(s.CurrentTime).TotalMilliseconds <= s.ExpiryTime);
            return lcs.SpellName != null;
        }

        public static void UpdateSpellHistory(string spellName, double expiryTime, WoWUnit unit)
        {
            if (!Units.IsValid(unit))
                return;
            PruneSpellHistory();
            if (unit != null) SpellHistory.Add(new LastCastSpell(spellName, 0, expiryTime, DateTime.UtcNow, unit.Guid));
        }

        private static void PruneSpellHistory()
        {
            SpellHistory.RemoveAll(s => DateTime.UtcNow.Subtract(s.CurrentTime).TotalMilliseconds >= s.ExpiryTime);
        }

        private static readonly List<LastCastSpell> SpellHistory = new List<LastCastSpell>();

        public struct LastCastSpell
        {
            public string SpellName { get; set; }
            public int SpellId { get; set; }
            public double ExpiryTime { get; set; }
            public DateTime CurrentTime { get; set; }
            public WoWGuid UnitGuid { get; set; }

            public LastCastSpell(string spellName, int spellid, double expiryTime, DateTime now, WoWGuid unitguid)
                : this()
            {
                SpellName = spellName;
                SpellId = spellid;
                ExpiryTime = expiryTime;
                CurrentTime = now;
                UnitGuid = unitguid;
            }
        }

        #endregion Properties
    }
}