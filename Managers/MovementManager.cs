/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.POI;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Managers
{
    public static class MovementManager
    {
        private static float Range
            => Math.Max(4f, StyxWoW.Me.CombatReach + 1.3333334f + StyxWoW.Me.CurrentTarget.CombatReach);

        public static async Task<bool> MoveToTarget()
        {
            await MovementFacing();
            return false;
        }

        public static void AutoTarget()
        {
            if (Globals.CurrentTarget != null)
                return;

            var units =
                Units.NearbyUnfriendlyUnits.Where(u => u.Aggro && u.Distance <= 30)
                    .OrderBy(u => u.Distance)
                    .ThenBy(u => u.HealthPercent)
                    .ToList();
            var target = units.FirstOrDefault();

            target?.Target();
        }

        public static void Face(WoWUnit unit)
        {
                unit.Face();
        }

#pragma warning disable 1998
        public static async Task MovementFacing()
#pragma warning restore 1998
        {
            if (!Capabilities.IsMovingAllowed && !Capabilities.IsFacingAllowed)
                return;

            // Only Facing
            if (Capabilities.IsFacingAllowed && !Capabilities.IsMovingAllowed)
            {
                // We need to let the player keep control
                if (StyxWoW.Me.IsMoving)
                    return;

                if (StyxWoW.Me.GotTarget && !StyxWoW.Me.IsSafelyFacing(StyxWoW.Me.CurrentTarget))
                    StyxWoW.Me.CurrentTarget.Face();
            }
            if (Capabilities.IsFacingAllowed)
            EnsureTarget();

            // Move into LOS
            if (Capabilities.IsMovingAllowed && StyxWoW.Me.CurrentTarget != null && StyxWoW.Me.CurrentTarget.Distance > Range)
            {
                Navigator.MoveTo(StyxWoW.Me.CurrentTarget.Location);
            }


            if (Capabilities.IsFacingAllowed && StyxWoW.Me.CurrentTarget != null && !StyxWoW.Me.IsMoving && !StyxWoW.Me.CurrentTarget.IsMe &&
                !StyxWoW.Me.IsSafelyFacing(StyxWoW.Me.CurrentTarget, 70))
                StyxWoW.Me.CurrentTarget.Face();


            if (Capabilities.IsMovingAllowed && !StyxWoW.Me.IsCasting && !StyxWoW.Me.IsChanneling)
            {
                var currentTarget = StyxWoW.Me.CurrentTarget;

                if (currentTarget == null || currentTarget == StyxWoW.Me)
                    return;

                if (StyxWoW.Me.Location.Distance(currentTarget.Location) < Range)
                {
                    if (StyxWoW.Me.IsMoving)
                    {
                        Navigator.PlayerMover.MoveStop();
                    }
                }
                else
                {
                    Navigator.MoveTo(currentTarget.Location);
                    //await CommonCoroutines.SleepForLagDuration();
                }
            }
        }

        private static void EnsureTarget()
        {
            // Get our first target
            var firstTarget = FirstTarget();

            if (firstTarget != null)
            {
                // This target should be our current target
                if (firstTarget.Guid != StyxWoW.Me.CurrentTargetGuid)
                {
                    firstTarget.Target();
                }
                return;
            }

            var secondTarget = SecondTarget();

            if (secondTarget != null && secondTarget.Guid != StyxWoW.Me.CurrentTargetGuid)
            {
                secondTarget.Target();
            }
        }

        private static WoWUnit FirstTarget()
        {
            // Check what the botbase wants to kill
            // If the target returns valid let it do what it wants
            if (BotPoi.Current.Type == PoiType.Kill)
            {
                var botpoi = BotPoi.Current.AsObject.ToUnit();

                if (botpoi.ValidAttackUnit())
                {
                    // Switch targets if we're not hitting what the botbase wants to kill
                    if (StyxWoW.Me.CurrentTargetGuid != botpoi.Guid)
                        return botpoi;
                }

                //BotPoi.Clear();
            }

            // Checks and return null if we need to
            // Let the botpoi select a new target so we can evaluate it
            if (StyxWoW.Me.CurrentTarget == null || StyxWoW.Me.CurrentTarget.IsDead)
                return null;

            if (!StyxWoW.Me.GroupInfo.IsInParty
                && StyxWoW.Me.Combat
                && ((!StyxWoW.Me.CurrentTarget.Combat
                     && !StyxWoW.Me.CurrentTarget.Aggro
                     && !StyxWoW.Me.CurrentTarget.PetAggro) || !StyxWoW.Me.CurrentTarget.InLineOfSpellSight))
            {
                // Look through mobs we have aggro'd
                // Orderby the lowest health and select it
                var target = ObjectManager.GetObjectsOfType<WoWUnit>()
                    .Where(
                        p => p.Distance < 10
                             && p.ValidAttackUnit()
                             && (p.Aggro || p.PetAggro)
                             && p.InLineOfSpellSight
                    )
                    .OrderBy(u => u.HealthPercent)
                    .FirstOrDefault();

                if (target != null && target.Guid != StyxWoW.Me.CurrentTargetGuid)
                {
                    // Return next best target
                    Logging.Write(Colors.YellowGreen,
                        "[ScourgeBloom] Switching to next combat target: " + target.SafeName);
                    return target;
                }
            }

            // Check to see if our current target is blacklisted
            if (Blacklist.Contains(StyxWoW.Me.CurrentTargetGuid, BlacklistFlags.Combat))
            {
                if (StyxWoW.Me.CurrentTarget.Combat && StyxWoW.Me.CurrentTarget.IsTargetingMeOrPet)
                {
                    // No other targets but the blacklisted one, don't have a choice really ...
                    return StyxWoW.Me.CurrentTarget;
                }

                Logging.Write(Colors.YellowGreen,
                    "[ScourgeBloom] Our current target, " + StyxWoW.Me.CurrentTarget.SafeName +
                    ", is blacklisted. Clearing target.");
                StyxWoW.Me.ClearTarget();
                return null;
            }

            // Return our current target
            if (StyxWoW.Me.CurrentTarget.ValidAttackUnit())
                return StyxWoW.Me.CurrentTarget;

            // If we're still unclear and our current target is in the target list, just do it ...
            if (Targeting.Instance.TargetList.Contains(StyxWoW.Me.CurrentTarget))
                return StyxWoW.Me.CurrentTarget;

            // At this point the target is invalid
            // Returning null should force the selection of a new target
            return null;
        }

        private static WoWUnit SecondTarget()
        {
            // Check BotPoi first
            if (BotPoi.Current.Type == PoiType.Kill)
            {
                var unit = BotPoi.Current.AsObject as WoWUnit;
                if (unit == null)
                {
                    Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Current BotPoi is null, clearing BotPoi");
                    BotPoi.Clear();
                }
                else if (!unit.IsAlive)
                {
                    Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Current unit is dead, clearing BotPoi");
                    BotPoi.Clear();
                }
                else if (Blacklist.Contains(unit, BlacklistFlags.Combat))
                {
                    Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Current unit is blacklisted, clearing BotPoi");
                    BotPoi.Clear();
                }
                else
                {
                    Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Current unit invalid, clearing BotPoi");
                    return unit;
                }
            }

            // Look for agrroed mobs next. prioritize by IsPlayer, Relative Distance, then Health
            var target = Targeting.Instance.TargetList
                .Where(
                    p => !Blacklist.Contains(p, BlacklistFlags.Combat)
                         && p.ValidAttackUnit()
                         &&
                         (p.Aggro || p.PetAggro ||
                          (p.Combat && p.GotTarget && (p.IsTargetingMeOrPet || p.IsTargetingMyRaidMember))))

                // Check if there's a player there, maybe we're getting ganked
                .OrderBy(u => u.IsPlayer)
                // Then order then pick the closest and lowest health target 
                .ThenBy(u => u.Distance)
                .ThenBy(u => u.HealthPercent)
                .FirstOrDefault();

            if (target != null)
            {
                // Return the target we selected
                Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Switching to a unit aggro'd to us");
                return target;
            }

            // At this point, look for anything in the target list
            target = Targeting.Instance.TargetList
                .Where(p => !Blacklist.Contains(p, BlacklistFlags.Combat) && p.IsAlive)
                .OrderBy(u => u.IsPlayer)
                .ThenBy(u => u.DistanceSqr)
                .FirstOrDefault();

            if (target != null)
            {
                // Return the closest one to us
                Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Switching to unit in the target list");
                return target;
            }

            return null;
        }
    }
}