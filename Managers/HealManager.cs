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
    internal class HealManager : ScourgeBloom
    {
        public static double GroupCount;
        public static WaitTimer SpamDelay = WaitTimer.OneSecond;

        public static HashSet<int> HealNpcs = new HashSet<int>
        {
            72311, //King Varian
            72302, //Lady Jaina
            73910, //Vanessa Windrunner
            62442, //Tsulong
            71604, //Immerseus Spawn
            71995, //Leven Puri fight
            71996, //Rook puri fight
            72000, //Sun puri fight
            71357, //Wrathion Challenge
            87321, //Healing Dummy
            78884, //Living Mushroom
            78868 //Revujanating Mushroom
        };

        public static List<WoWUnit> InitialList
        {
            get
            {
                // Grab party+raid member + myself GUIDs
                var guids =
                    StyxWoW.Me.GroupInfo.RaidMemberGuids.Union(StyxWoW.Me.GroupInfo.PartyMemberGuids)
                        .Distinct()
                        .ToArray();
                var initiallist = (
                    from p in ObjectManager.GetObjectsOfType<WoWUnit>(true)
                    where p.IsFriendly && (guids.Any(g => g == p.Guid) || HealNpcs.Any(h => h == p.Entry))
                    select p).ToList();
                initiallist.Add(StyxWoW.Me);
                GroupCount = initiallist.Count();
                if (initiallist.Count != 0 && SpamDelay.IsFinished)
                {
                    Log.WritetoFile(LogLevel.Diagnostic, "HealManager List: " + initiallist.Count + " Entries");
                    SpamDelay.Reset();
                }
                return initiallist;
            }
        }

        public static List<WoWUnit> ValidList
        {
            get { return InitialList.Where(IsValid).ToList(); }
        }

        public static WoWUnit CleanseTarget
        {
            get
            {
                return InitialList.FirstOrDefault(unit => unit.IsValid && unit.Distance < 40 && NeedCleanseAsap(unit));
            }
        }

        public static List<WoWUnit> Tanks
        {
            get
            {
                if (!Me.GroupInfo.IsInParty)
                    return new List<WoWUnit> {StyxWoW.Me.ToUnit()};
                var myTanks =
                    StyxWoW.Me.GroupInfo.RaidMembers.Union(StyxWoW.Me.GroupInfo.PartyMembers)
                        .Distinct()
                        .Where(mbr => mbr.IsMainAssist || mbr.IsMainTank)
                        .ToList();
                var tankList = (from pm in myTanks
                    where InitialList.Where(t => IsValid(t) && t.Distance <= 100).Contains(pm.ToPlayer())
                    select pm.ToPlayer()).Cast<WoWUnit>().ToList();
                if (!tankList.Any())
                    tankList.Add(StyxWoW.Me.ToUnit());
                return tankList;
            }
        }

        public static List<WoWGuid> TankGuids
        {
            get
            {
                var myTanks =
                    StyxWoW.Me.GroupInfo.RaidMembers.Union(StyxWoW.Me.GroupInfo.PartyMembers)
                        .Distinct()
                        .Where(mbr => mbr.IsMainAssist || mbr.IsMainTank)
                        .ToList();
                return (from pm in myTanks where InitialList.Contains(pm.ToPlayer()) select pm.Guid).ToList();
            }
        }

        public static bool IsPhased(WoWUnit unit)
        {
            return unit.GetAllAuras().Any(a => a.SpellId == 144850 || a.SpellId == 144849 || a.SpellId == 144851);
        }

        public static IEnumerable<WoWUnit> Targets(double minhealthpct = 100, double maxdistance = 40)
        {
            return InitialList.Where(unit => IsValid(unit) &&
                                             (unit.HealthPercent <= minhealthpct ||
                                              unit.GetPredictedHealthPercent() <= minhealthpct) &&
                                             unit.Distance <= maxdistance).ToList();
        }

        public static bool IsValid(WoWUnit unit)
        {
            if (unit == null ||
                !unit.IsValid)
                return false;
            return unit.IsValid &&
                   unit.CanSelect &&
                   unit.InLineOfSpellSight &&
                   unit.IsAlive &&
                   !IsPhased(unit) &&
                   !Blacklist.Contains(unit, BlacklistFlags.Combat);
        }

        #region HealTargeting

        public static WoWUnit SmartTarget(double minHealthPct)
        {
            //First Lets Build a complete List of Possible Targets
            var allTargets =
                (from t in InitialList
                    where IsValid(t) && t.GetPredictedHealthPercent() <= minHealthPct && t.TotalAbsorbs != t.MaxHealth
                    select t).ToList();
            var bestTarget =
                (from t in allTargets
                    orderby Calcweight(t)
                    //percentage = (int * 100 / total)
                    select t).FirstOrDefault();
            if (bestTarget != null && bestTarget.IsValid)
                Log.WritetoFile(LogLevel.Diagnostic,
                    "Healing Target selected:" + bestTarget.SafeName + "@" + bestTarget.HealthPercent + "HP" + " &" +
                    Math.Round(Calcweight(bestTarget)) + "weight");
            return bestTarget;
        }

        public static IOrderedEnumerable<WoWUnit> SmartTargets(double minHealthPct, double range = 40)
        {
            //First Lets Build a complete List of Possible Targets
            var allTargets =
                (from t in InitialList
                    where
                        IsValid(t) && t.GetPredictedHealthPercent() <= minHealthPct && t.TotalAbsorbs != t.MaxHealth &&
                        t.Distance <= range
                    select t).ToList();
            var bestTargets =
                from t in allTargets
                orderby Calcweight(t)
                select t;
            return bestTargets;
        }

        private static double Calcweight(WoWUnit unit)
        {
            if (unit == null || !unit.IsValid || !unit.IsAlive || !Me.GroupInfo.IsInParty)
                return 0;
            if (!unit.IsPlayer)
                return unit.HealthPercent*2;
            var member =
                Me.GroupInfo.RaidMembers.Union(Me.GroupInfo.PartyMembers)
                    .Distinct()
                    .FirstOrDefault(mbr => mbr.Guid == unit.Guid);
            double multiplier = 2;
            if (member != null && member.Role == WoWPartyMember.GroupRole.Healer)
                multiplier = 1.5;
            if (member != null && member.Role == WoWPartyMember.GroupRole.Tank)
                multiplier = 1;
            return unit.HealthPercent*multiplier;
        }

        public static WoWUnit GetTarget(double healthpct, float range = 40)
        {
            var healTargets = Targets(healthpct, range);
            return healTargets.OrderBy(unit => unit.HealthPercent).FirstOrDefault();
        }

        public static WoWUnit Target
        {
            get { return GetTarget(100); }
        }

        public static IEnumerable<WoWUnit> NeedMyAura(string aura, double healthpct, float range = 40)
        {
            return Targets(healthpct, range).Where(unit => !unit.HasAura(aura)).OrderBy(unit => unit.HealthPercent);
        }

        public static bool NeedCleanseAsap(WoWUnit unit)
        {
            return
                unit.Auras.Any(
                    aura =>
                        aura.Value.IsHarmful &&
                        (aura.Value.Spell.DispelType == WoWDispelType.Magic ||
                         aura.Value.Spell.DispelType == WoWDispelType.Poison ||
                         aura.Value.Spell.DispelType == WoWDispelType.Disease));
        }

        public static double CountNearby(WoWObject unitCenter, float distance, double healthPercent)
        {
            return !Units.IsValid(unitCenter)
                ? 0
                : Targets(healthPercent).Count(unit => unitCenter.Location.Distance(unit.Location) <= distance);
        }

        #endregion HealTargeting
    }
}