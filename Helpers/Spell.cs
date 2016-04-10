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
using Buddy.Coroutines;
using JetBrains.Annotations;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    [UsedImplicitly]
    public static class Spell
    {
        public static readonly LocalPlayer Me = StyxWoW.Me;
        public static WoWUnit LastCastTarget;
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public static string LastSpellCast { get; private set; }

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

                Log.WriteLog(LogLevel.Normal, $"Casting {sname}");

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
                Log.WritetoFile(LogLevel.Diagnostic, $"Casting {sname}");
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

                Log.WriteLog(LogLevel.Normal, $"Casting {sname}");

                if (unit == null || !reqs || !SpellManager.CanCast(spell, unit, true))
                    return false;

                if (!SpellManager.Cast(spell, unit))
                    return false;

                if (!await Coroutine.Wait(GetSpellCastTime(sname), () => cancel) && GetSpellCastTime(sname).TotalSeconds > 0)
                {
                    SpellManager.StopCasting();
                    Log.WriteLog("[ScourgeBloom] Canceling " + sname + ".");
                    return false;
                }

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

        public static TimeSpan GetSpellCastTime(string s)
        {
            SpellFindResults sfr;
            return SpellManager.FindSpell(s, out sfr)
                ? TimeSpan.FromMilliseconds((sfr.Override ?? sfr.Original).CastTime)
                : TimeSpan.Zero;
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