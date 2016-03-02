/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using Bots.DungeonBuddy.Helpers;
using ScourgeBloom.Managers;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace ScourgeBloom.Helpers
{
    public static class Units
    {
        private static readonly LocalPlayer Me = StyxWoW.Me;

        // this one optimized for single applytype lookup
        public static bool HasAuraWithEffect(this WoWUnit unit, WoWApplyAuraType applyType)
        {
            return
                unit.Auras.Values.Any(a => a.Spell != null && a.Spell.SpellEffects.Any(se => applyType == se.AuraType));
        }

        public static bool HasAuraWithEffect(this WoWUnit unit, params WoWApplyAuraType[] applyType)
        {
            var hashes = new HashSet<WoWApplyAuraType>(applyType);
            return
                unit.Auras.Values.Any(
                    a => a.Spell != null && a.Spell.SpellEffects.Any(se => hashes.Contains(se.AuraType)));
        }

        #region UnfriendlyUnits

        public static IEnumerable<WoWUnit> UnfriendlyUnits
        {
            get
            {
                using (StyxWoW.Memory.AcquireFrame(true))
                {
                    return ObjectManager.GetObjectsOfTypeFast<WoWUnit>().Where(u => u.ValidAttackUnit()).ToList();
                }
            }
        }

        #endregion UnfriendlyUnits

        #region GroupMemberInfos

        /// <summary>
        ///     List of WoWPartyMember in your Group. Deals with Party / Raid in a list independent manner and does not restrict
        ///     distance
        /// </summary>
        public static IEnumerable<WoWPartyMember> GroupMemberInfos
        {
            get { return StyxWoW.Me.GroupInfo.RaidMembers.Union(StyxWoW.Me.GroupInfo.PartyMembers).Distinct(); }
        }

        #endregion GroupMemberInfos

        #region Enemies

        #region EnemiesInRange

        public static int EnemiesInRange(int range)
        {
            return UnfriendlyUnits.Count(u => u.Distance2DSqr <= range * range);
        }

        #endregion EnemiesInRange

        public static int EnemiesAroundTarget(WoWUnit target, int range)
        {
            return UnfriendlyUnits.Count(u => u.Location.Distance(target.Location) <= range);
        }

        public static bool EnemyAdd
        {
            get { return UnfriendlyUnits.Any(u => u.IsTargetingMeOrPet && StyxWoW.Me.CurrentTarget != u); }
        }

        #region EnemyUnitsSub40

        public static IEnumerable<WoWUnit> EnemyUnitsSub40
        {
            get { return EnemyUnits(40); }
        }

        #endregion EnemyUnitsSub40

        #region EnemyUnitsSub10

        public static IEnumerable<WoWUnit> EnemyUnitsSub10
        {
            get { return EnemyUnits(10); }
        }

        #endregion EnemyUnitsSub10

        #region EnemyUnitsSub8

        public static IEnumerable<WoWUnit> EnemyUnitsSub8
        {
            get { return EnemyUnits(8); }
        }

        #endregion EnemyUnitsSub8

        #region EnemyUnitsMelee

        public static IEnumerable<WoWUnit> EnemyUnitsMelee
        {
            get { return EnemyUnits(Me.MeleeRange().ToString(CultureInfo.InvariantCulture).ToInt32()); }
        }

        #endregion EnemyUnitsMelee

        #region EnemyUnitsNearTarget

        public static IEnumerable<WoWUnit> EnemyUnitsNearTarget(float distance)
        {
            var dist = distance * distance;
            var curTarLocation = StyxWoW.Me.CurrentTarget.Location;
            return ObjectManager.GetObjectsOfType<WoWUnit>().Where(
                p => IsValid(p) && p.IsHostile && p.Location.DistanceSqr(curTarLocation) <= dist).ToList();
        }

        #endregion EnemyUnitsNearTarget

        #endregion Enemies

        #region IsInGroup

        public static bool IsInGroup(this LocalPlayer me)
        {
            return me.GroupInfo.IsInParty || me.GroupInfo.IsInRaid;
        }

        #endregion IsInGroup

        #region ValidAttackUnit

        public static bool ValidAttackUnit(this WoWUnit p)
        {
            if (p == null || !p.IsValid)
                return false;

            if (!p.Attackable)
                return false;

            if (p.IsFriendly)
                return false;

            if (p.IsDead)
                return false;

            if (p.IsTotem)
                return false;

            if (p.IsNonCombatPet)
                return false;

            if (!p.CanSelect)
                return false;

            if (p.IsCritter)
                return false;

            if (p.IsPlayer && !p.IsHostile)
                return false;

            if (p.IsPet && !p.IsHostile)
                return false;

            return true;
        }

        #endregion ValidAttackUnit

        #region EnemyUnits

        public static IEnumerable<WoWUnit> EnemyUnits(int maxSpellDist)
        {
            var typeWoWUnit = typeof(WoWUnit);
            var typeWoWPlayer = typeof(WoWPlayer);
            var objectList = ObjectManager.ObjectList;
            return (from t1 in objectList
                    let type = t1.GetType()
                    where type == typeWoWUnit ||
                          type == typeWoWPlayer
                    select t1 as WoWUnit
                into t
                    where t != null && IsValid(t) && t.InRange()
                    select t).ToList();
        }

        #endregion EnemyUnits

        #region EnemyUnitsCone

        public static IEnumerable<WoWUnit> EnemyUnitsCone(WoWUnit target, IEnumerable<WoWUnit> otherUnits,
            float distance)
        {
            var targetLoc = target.Location;
            // most (if not all) player cone spells are 90 degrees.
            return otherUnits.Where(u => target.IsSafelyFacing(u, 90) && u.Location.Distance(targetLoc) <= distance);
        }

        #endregion EnemyUnitsCone

        #region FriendlyUnitsNearTarget

        public static IEnumerable<WoWUnit> FriendlyUnitsNearTarget(float distance)
        {
            var dist = distance * distance;
            var curTarLocation = StyxWoW.Me.CurrentTarget.Location;
            return ObjectManager.GetObjectsOfType<WoWUnit>().Where(
                p => IsValid(p) && p.IsFriendly && p.Location.DistanceSqr(curTarLocation) <= dist)
                .ToList();
        }

        #endregion FriendlyUnitsNearTarget

        #region GetPathUnits

        public static IEnumerable<WoWUnit> GetPathUnits(WoWUnit target, IEnumerable<WoWUnit> otherUnits, float distance)
        {
            var myLoc = StyxWoW.Me.Location;
            var targetLoc = target.Location;
            return
                otherUnits.Where(
                    u => u.Location.GetNearestPointOnSegment(myLoc, targetLoc).Distance(u.Location) <= distance);
        }

        #endregion GetPathUnits

        #region InRange

        public static bool InRange(this WoWUnit unit)
        {
            if (!IsValid(unit))
                return false;
            if (unit.Guid == Me.Guid)
                return true;
            if (unit.Distance <= Math.Max(5f, Me.CombatReach + 1.3333334f + unit.CombatReach))
                return true;
            if (unit.IsWithinMeleeRange)
                return true;
            return false;
        }

        #endregion InRange

        #region IsFriendly

        public static bool IsFriendly(this WoWUnit target)
        {
            if (!HealManager.IsValid(target))
                return false;
            if (target.Guid == Me.Guid)
                return true;
            if (HealManager.InitialList.Contains(target.ToPlayer()))
                return true;
            if (Me.CurrentMap.IsArena && !Me.GroupInfo.IsInCurrentParty(target.Guid))
                return false;
            if (target.IsFriendly)
                return true;
            if (target.IsPlayer && target.ToPlayer().FactionGroup == Me.FactionGroup)
                return true;
            return false;
        }

        #endregion IsFriendly

        #region Status

        public static string Status(this WoWUnit unit)
        {
            if (!IsValid(unit))
                return "Unknown";
            if (Spell.Me.Role == WoWPartyMember.GroupRole.Tank && unit.IsHostile)
                return unit.ThreatInfo.RawPercent + "%Threat";
            if (Spell.Me.Role == WoWPartyMember.GroupRole.Damage && !unit.IsFriendly)
                return Math.Round(Spell.Me.EnergyPercent) + "%Energy" + Math.Round(unit.HealthPercent) + "%HP's" +
                       Spell.Me.ComboPoints + "CP's";
            if (Spell.Me.Role == WoWPartyMember.GroupRole.Healer)
                return Math.Round(unit.HealthPercent()) + "%HP's " + Math.Round(Spell.Me.PowerPercent) + "%" +
                       Spell.Me.PowerType + " " + Spell.Me.CurrentChi + "CP's";
            return Math.Round(unit.HealthPercent) + "%HP's";
        }

        #endregion Status

        #region HealthPercent (Predicted and Current)

        public static double HealthPercent(this WoWUnit unit)
        {
            if (unit == null || !unit.IsValid)
                return double.MinValue;
            return Math.Max(unit.GetPredictedHealthPercent(), unit.HealthPercent);
        }

        #endregion HealthPercent (Predicted and Current)

        #region DebuffCC

        public static bool DebuffCc(this WoWUnit target)
        {
            {
                if (!target.IsPlayer)
                {
                    return false;
                }
                if (target.Stunned)
                {
                    Log.WriteLog("Stunned!", Colors.Red);
                    return true;
                }
                if (target.Silenced)
                {
                    Log.WriteLog("Silenced", Colors.Red);
                    return true;
                }
                if (target.Dazed)
                {
                    Log.WriteLog("Dazed", Colors.Red);
                    return true;
                }

                var auras = target.GetAllAuras();

                return auras.Any(a => a.Spell != null && a.Spell.SpellEffects.Any(
                    se => se.AuraType == WoWApplyAuraType.ModConfuse
                          || se.AuraType == WoWApplyAuraType.ModCharm
                          || se.AuraType == WoWApplyAuraType.ModFear
                          || se.AuraType == WoWApplyAuraType.ModPacify
                          || se.AuraType == WoWApplyAuraType.ModPacifySilence
                          || se.AuraType == WoWApplyAuraType.ModPossess
                          || se.AuraType == WoWApplyAuraType.ModStun
                    ));
            }
        }

        #endregion DebuffCC

        #region IsValid

        public static bool IsValid(WoWUnit unit)
        {
            if (unit == null ||
                !unit.IsValid)
                return false;
            return unit.CanSelect &&
                   unit.Attackable &&
                   !unit.IsFriendly &&
                   !unit.IsPet &&
                   unit.CreatedByUnit == null &&
                   !unit.IsCritter &&
                   !unit.IsPetBattleCritter &&
                   !Blacklist.Contains(unit, BlacklistFlags.Combat);
        }

        public static bool IsValid(WoWObject o)
        {
            return o != null && o.IsValid;
        }

        #endregion IsValid

        #region Auras

        public static bool HasAura(this WoWUnit unit, int auraid, int msLeft = 0, int stacks = 0)
        {
            if (unit == null)
                return false;
            var result =
                unit.GetAllAuras()
                    .FirstOrDefault(a => a.CreatorGuid == StyxWoW.Me.Guid && a.SpellId == auraid && !a.IsPassive);
            if (result == null)
                return false;

            if (result.TimeLeft.TotalMilliseconds < msLeft && msLeft != 0)
                return false;

            return result.StackCount >= stacks || stacks == 0;
        }

        public static bool HasAura(this WoWUnit unit, string aura, int stacks)
        {
            return HasAura(unit, aura, stacks, null);
        }

        public static bool HasMyAura(this WoWUnit unit, int aura)
        {
            return unit.GetAllAuras().Any(a => a.SpellId == aura && a.CreatorGuid == Me.Guid);
        }

        private static bool HasAura(this WoWUnit unit, string aura, int stacks, WoWUnit creator)
        {
            return
                unit.GetAllAuras()
                    .Any(
                        a =>
                            a.Name == aura && a.StackCount >= stacks &&
                            (creator == null || a.CreatorGuid == creator.Guid));
        }

        public static bool HasAura(this WoWUnit unit, string name, bool myAurasOnly)
        {
            if (unit == null)
                return false;
            var result =
                unit.GetAllAuras()
                    .FirstOrDefault(a => a.CreatorGuid == StyxWoW.Me.Guid && a.Name == name && !a.IsPassive);
            if (result == null)
                return false;
            return true;
        }

        public static bool HasAnyAura(this WoWUnit unit, params string[] auraNames)
        {
            var auras = unit.GetAllAuras();
            var hashes = new HashSet<string>(auraNames);
            return auras.Any(a => hashes.Contains(a.Name));
        }

        public static uint AuraTimeLeft(this WoWUnit unit, int aura)
        {
            if (!unit.IsValid)
                return 0;

            var result =
                unit.GetAllAuras()
                    .FirstOrDefault(a => a.CreatorGuid == StyxWoW.Me.Guid && a.SpellId == aura && !a.IsPassive);

            return result == null ? 0 : result.Duration;
        }

        public static TimeSpan GetAuraTimeLeft(this WoWUnit onUnit, string auraName, bool fromMyAura = true)
        {
            var wantedAura =
                onUnit.GetAllAuras()
                    .FirstOrDefault(
                        a =>
                            a != null && a.Name == auraName && a.TimeLeft > TimeSpan.Zero &&
                            (!fromMyAura || a.CreatorGuid == StyxWoW.Me.Guid));

            return wantedAura != null ? wantedAura.TimeLeft : TimeSpan.Zero;
        }

        public static uint GetAuraStackCount(this WoWUnit unit, string aura, bool fromMyAura = true)
        {
            if (unit != null && unit.IsValid)
            {
                var s = unit.Auras.Values.FirstOrDefault(a => a.Name == aura && a.CreatorGuid == Me.Guid);
                if (s != null)
                {
                    Log.WritetoFile(LogLevel.Diagnostic,
                        string.Format("{0} has {1} stacks of {2}", unit.SafeName(), unit.Auras[aura].StackCount, aura));
                    return s.StackCount;
                }
            }
            return uint.MinValue;
        }

        public static bool HasAuraExpired(this WoWUnit u, string aura, int secs = 3, bool myAura = true)
        {
            return u.HasAuraExpired(aura, aura, secs, myAura);
        }

        public static bool HasAuraExpired(this WoWUnit u, string spell, string aura, int secs = 3, bool myAura = true)
        {
            // need to compare millisecs even though seconds are provided.  otherwise see it as expired 999 ms early because
            // .. of loss of precision
            return SpellManager.HasSpell(spell) && u.GetAuraTimeLeft(aura, myAura).TotalSeconds <= secs;
        }

        #endregion Auras

        #region PartyBuffs

        public static bool HasPartyBuff(this WoWUnit onunit, Stat stat)
        {
            if (stat == Stat.AttackPower)
                return onunit.HasAnyAura("Horn of Winter", "Trueshot Aura", "Battle Shout");

            if (stat == Stat.BurstHaste)
                return onunit.HasAnyAura("Time Warp", "Ancient Hysteria", "Heroism", "Bloodlust", "Netherwinds",
                    "Drums of Fury");

            if (stat == Stat.CriticalStrike)
                return onunit.HasAnyAura("Leader of the Pack", "Arcane Brilliance", "Dalaran Brilliance",
                    "Legacy of the White Tiger",
                    "Lone Wolf: Ferocity of the Raptor", "Terrifying Roar", "Fearless Roar", "Strength of the Pack",
                    "Embrace of the Shale Spider",
                    "Still Water", "Furious Howl");

            if (stat == Stat.Haste)
                return onunit.HasAnyAura("Unholy Aura", "Mind Quickening", "Swiftblade's Cunning", "Grace of Air",
                    "Lone Wolf: Haste of the Hyena",
                    "Cackling Howl", "Savage Vigor", "Energizing Spores", "Speed of the Swarm");

            if (stat == Stat.Mastery)
                return onunit.HasAnyAura("Power of the Grave", "Moonkin Aura", "Blessing of Might", "Grace of Air",
                    "Lone Wolf: Grace of the Cat",
                    "Roar of Courage", "Keen Senses", "Spirit Beast Blessing", "Plainswalking");

            if (stat == Stat.MortalWounds)
                return onunit.HasAnyAura("Mortal Strike", "Wild Strike", "Wound Poison", "Rising Sun Kick",
                    "Mortal Cleave", "Legion Strike",
                    "Bloody Screech", "Deadly Bite", "Monstrous Bite", "Gruesome Bite", "Deadly Sting");

            if (stat == Stat.Multistrike)
                return onunit.HasAnyAura("Windflurry", "Mind Quickening", "Swiftblade's Cunning", "Dark Intent",
                    "Lone Wolf: Quickness of the Dragonhawk",
                    "Sonic Focus", "Wild Strength", "Double Bite", "Spry Attacks", "Breath of the Winds");

            if (stat == Stat.SpellPower)
                return onunit.HasAnyAura("Arcane Brilliance", "Dalaran Brilliance", "Dark Intent",
                    "Lone Wolf: Wisdom of the Serpent", "Still Water",
                    "Qiraji Fortitude", "Serpent's Cunning");

            if (stat == Stat.Stamina)
                return onunit.HasAnyAura("Power Word: Fortitude", "Blood Pact", "Commanding Shout",
                    "Lone Wolf: Fortitude of the Bear",
                    "Fortitude", "Invigorating Roar", "Sturdiness", "Savage Vigor", "Qiraji Fortitude");

            if (stat == Stat.Stats)
                return onunit.HasAnyAura("Mark of the Wild", "Legacy of the Emperor", "Legacy of the White Tiger",
                    "Blessing of Kings",
                    "Lone Wolf: Power of the Primates", "Blessing of Forgotten Kings", "Bark of the Wild",
                    "Blessing of Kongs",
                    "Embrace of the Shale Spider", "Strength of the Earth");

            if (stat == Stat.Versatility)
                return onunit.HasAnyAura("Unholy Aura", "Mark of the Wild", "Sanctity Aura", "Inspiring Presence",
                    "Lone Wolf: Versatility of the Ravager",
                    "Tenacity", "Indomitable", "Wild Strength", "Defensive Quills", "Chitinous Armor", "Grace",
                    "Strength of the Earth");

            return false;
        }

        public enum Stat
        {
            AttackPower,
            BurstHaste,
            CriticalStrike,
            Haste,
            Mastery,
            MortalWounds,
            Multistrike,
            SpellPower,
            Stamina,
            Stats,
            Versatility
        }

        #endregion PartyBuffs

        #region mechanic

        public static bool HasAuraWithMechanic(this WoWUnit unit, params WoWSpellMechanic[] mechanics)
        {
            var auras = unit.GetAllAuras();
            return auras.Any(a => mechanics.Contains(a.Spell.Mechanic));
        }

        public static bool IsStunned(this WoWUnit unit)
        {
            return unit.HasAuraWithMechanic(WoWSpellMechanic.Stunned, WoWSpellMechanic.Incapacitated);
        }

        public static bool IsCrowdControlled(this WoWUnit unit)
        {
#if AURAS_HAVE_MECHANICS
            return auras.Any(
                a => a.Spell.Mechanic == WoWSpellMechanic.Banished ||
                     a.Spell.Mechanic == WoWSpellMechanic.Charmed ||
                     a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                     a.Spell.Mechanic == WoWSpellMechanic.Incapacitated ||
                     a.Spell.Mechanic == WoWSpellMechanic.Polymorphed ||
                     a.Spell.Mechanic == WoWSpellMechanic.Sapped ||
                     a.Spell.Mechanic == WoWSpellMechanic.Shackled ||
                     a.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                     a.Spell.Mechanic == WoWSpellMechanic.Frozen ||
                     a.Spell.Mechanic == WoWSpellMechanic.Invulnerable ||
                     a.Spell.Mechanic == WoWSpellMechanic.Invulnerable2 ||
                     a.Spell.Mechanic == WoWSpellMechanic.Turned ||

                     // Really want to ignore hexed mobs.
                     a.Spell.Name == "Hex"

                     );
#else
            return unit.Stunned
                   || unit.Rooted
                   || unit.Fleeing
                   || unit.HasAuraWithEffectsing(
                       WoWApplyAuraType.ModConfuse,
                       WoWApplyAuraType.ModCharm,
                       WoWApplyAuraType.ModFear,
                       WoWApplyAuraType.ModDecreaseSpeed,
                       WoWApplyAuraType.ModPacify,
                       WoWApplyAuraType.ModPacifySilence,
                       WoWApplyAuraType.ModPossess,
                       WoWApplyAuraType.ModRoot,
                       WoWApplyAuraType.ModStun);
#endif
        }

        public static bool HasAuraWithEffectsing(this WoWUnit unit, params WoWApplyAuraType[] applyType)
        {
            var hashes = new HashSet<WoWApplyAuraType>(applyType);
            return
                unit.Auras.Values.Any(
                    a => a.Spell != null && a.Spell.SpellEffects.Any(se => hashes.Contains(se.AuraType)));
        }

        public static bool IsSlowed(this WoWUnit unit)
        {
            return
                unit.GetAllAuras()
                    .Any(a => a.Spell.SpellEffects.Any(e => e.AuraType == WoWApplyAuraType.ModDecreaseSpeed));
        }

        #endregion mechanic
    }
}