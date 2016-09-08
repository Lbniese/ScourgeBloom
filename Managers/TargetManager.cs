using System;
using System.Collections.Generic;
using System.Linq;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Managers
{
    internal class TargetManager : ScourgeBloom
    {
        public static Styx.Common.Helpers.WaitTimer SpamDelay = Styx.Common.Helpers.WaitTimer.OneSecond;
        private static List<WoWUnit> _initialunits = new List<WoWUnit>();

        public static List<WoWUnit> InitialUnits
        {
            get
            {
                _initialunits = ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(IsValid).ToList();
                if (_initialunits.Count != 0 && SpamDelay.IsFinished)
                {
                    Log.WritetoFile(LogLevel.Diagnostic,
                        "TargetManager List: " + _initialunits.Count + " Entries");
                    SpamDelay.Reset();
                }
                return _initialunits;
            }
        }

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
            if (o == null ||
                !o.IsValid)
                return false;
            return true;
        }

        public static WoWUnit NeedTaunt
        {
            get
            {
                return InitialUnits.Where(unit =>
                    HealManager.Tanks.All(tank => tank.GetThreatInfoFor(unit).ThreatStatus != ThreatStatus.SecurelyTanking))
                    .OrderBy(unit => unit.ThreatInfo.ThreatValue)
                    .FirstOrDefault();
            }
        }

        public static WoWUnit InterruptTarget => InterruptTargets.FirstOrDefault();

        public static List<WoWUnit> InterruptTargets
        {
            get
            {
                return InitialUnits
                    .Where(unit =>
                        (unit.CastingSpell != null &&
                         unit.CanInterruptCurrentSpellCast) ||
                        unit.ChannelObject != null &&
                        SpellLists.ChanneledInteruptableSpells.Contains(unit.ChanneledCastingSpellId))
                    .ToList();
            }
        }

        public static void EnsureTarget(WoWUnit onunit)
        {
            if (!IsValid(StyxWoW.Me.CurrentTarget) && onunit != null && onunit.IsValid && onunit.IsAlive)
            {
                Log.WriteLog(LogLevel.Diagnostic, "No Target...Reselecting");
                onunit.Target();
                return;
            }
            if (Me.Combat && Me.CurrentTarget != null && !StyxWoW.Me.CurrentTarget.IsAlive)
            {
                StyxWoW.Me.ClearTarget();
            }
        }

        public static bool BossFight
        {
            get
            {
                return InitialUnits.Count(unit => unit.IsBoss && unit.Classification == WoWUnitClassificationType.Elite && unit.Distance < 40) != 0;
            }
        }

        public static WoWUnit MeleeTarget
        {
            get { return SmartTarget(); }
        }

        public static WoWUnit RangeTarget
        {
            get { return SmartTarget(true); }
        }

        public static WoWUnit SmartTarget(bool range = false, double distance = 40)
        {
            //First Lets Build a complete List of Possible Targets
            var allTargets =
                (from t in InitialUnits
                    where IsValid(t) &&
                          range
                        ? (t.Distance + t.CombatReach) <= distance
                        : t.IsWithinMeleeRange &&
                          //If your in a group and not a tank lets not pull anything that isn't already aggroed on us and isn't withing reach.
                          (!Me.GroupInfo.IsInParty || t.IsWithinMeleeRange || t.IsTargetingMeOrPet ||
                           t.IsTargetingMyPartyMember ||
                           t.IsTargetingMyRaidMember ||
                           t.ThreatInfo.ThreatStatus != ThreatStatus.UnitNotInThreatTable ||
                           HealManager.InitialList.Any(gp => gp.Combat && gp.CurrentTarget == t)) &&
                          t.IsAlive
                    select t).ToList();
            if (Me.CurrentTarget != null && !allTargets.Contains(Me.CurrentTarget)) allTargets.Add(Me.CurrentTarget);
            WoWUnit bestTarget =
                (from t in allTargets
                    orderby distance, t.HealthPercent
                    //percentage = (int * 100 / total)
                    select t).FirstOrDefault();
            if (bestTarget != null && bestTarget.IsValid)
                Log.WritetoFile(LogLevel.Diagnostic,
                    "Hostile Target selected:" + bestTarget.SafeName + "@" + bestTarget.HealthPercent + "HP@" +
                    Math.Round(bestTarget.Distance) + "Distance");
            return bestTarget;
        }

        public static double CountNear(WoWObject unitCenter, float distance)
        {
            if (!IsValid(unitCenter))
                return 0;
            return InitialUnits.Count(unit => unit.ThreatInfo.ThreatStatus != ThreatStatus.UnitNotInThreatTable &&
                                                         unitCenter.Location.Distance(unit.Location) <= distance);

        }

        public static double CountUnitsTargetingMe()
        {
            return InitialUnits.Count(unit => unit.IsTargetingMeOrPet);
        }

        public static IEnumerable<WoWUnit> HasAura(string aura, float range = 40)
        {
            return InitialUnits.Where(unit => unit.HasAura(aura));
        }
    }
}
