/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using ScourgeBloom.Managers;
using ScourgeBloom.Settings;
using Styx;
using Styx.CommonBot.Routines;

namespace ScourgeBloom.Lists
{
    internal static class Capabilities
    {
        //private static ThrottleCoroutineTask _canUseCapability;

        //public static bool CanUse(CapabilityFlags flag)
        //{
        //    if (RoutineManager.GetCapabilityState(flag) != CapabilityState.Disallowed)
        //    {
        //        var x = _canUseCapability ??
        //                (_canUseCapability =
        //                    new ThrottleCoroutineTask(TimeSpan.FromMinutes(3),
        //                        async () =>

        //                            Log.WriteLog(
        //                                "{0} Capability is disabled. Abiding the state and ignoring all related spells. ",
        //                                flag)));
        //        return true;
        //    }
        //    return false;
        //}

        public static bool IsPetUsageAllowed
        {
            get { return GeneralSettings.Instance.PetUsage && !RoutineManager.IsAnyDisallowed(CapabilityFlags.PetUse); }
        }

        public static bool IsPetSummonAllowed
        {
            get
            {
                return IsPetUsageAllowed && !StyxWoW.Me.GotAlivePet &&
                       !RoutineManager.IsAnyDisallowed(CapabilityFlags.PetSummoning) &&
                       PetManager.PetSummonAfterDismountTimer.IsFinished;
            }
        }

        public static bool IsDeathGripAllowed
        {
            get
            {
                return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Taunting | CapabilityFlags.GapCloser) &&
                       DeathKnightSettings.Instance.DeathGrip;
            }
        }

        public static bool IsFacingAllowed
        {
            get { return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && GeneralSettings.Instance.Facing; }
        }

        public static bool IsCooldownUsageAllowed
        {
            get
            {
                return
                    !RoutineManager.IsAnyDisallowed(CapabilityFlags.OffensiveCooldowns |
                                                    CapabilityFlags.DefensiveCooldowns) &&
                    GeneralSettings.Instance.Cooldowns;
            }
        }

        public static bool IsInterruptingAllowed
        {
            get
            {
                return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Interrupting) &&
                       GeneralSettings.Instance.Interrupts;
            }
        }

        public static bool IsMovingAllowed
        {
            get
            {
                return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement) &&
                       !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && GeneralSettings.Instance.Movement;
            }
        }

        public static bool IsRacialUsageAllowed
        {
            get { return GeneralSettings.Instance.UseRacials; }
        }

        public static bool IsTrinketUsageAllowed
        {
            get { return GeneralSettings.Instance.UseTrinket1 | GeneralSettings.Instance.UseTrinket2; }
        }

        public static bool IsTargetingAllowed
        {
            get
            {
                return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Targeting) && GeneralSettings.Instance.Targeting;
            }
        }

        public static bool IsTauntingAllowed
        {
            get
            {
                return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Taunting) &&
                       !RoutineManager.IsAnyDisallowed(CapabilityFlags.GapCloser) && DeathKnightSettings.Instance.DeathGrip;
            }
        }

        public static bool IsAoeAllowed
        {
            get { return !RoutineManager.IsAnyDisallowed(CapabilityFlags.Aoe) && GeneralSettings.Instance.AoE; }
        }
    }
}