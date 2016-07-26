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
        public static bool IsPetUsageAllowed
            => GeneralSettings.Instance.PetUsage && !RoutineManager.IsAnyDisallowed(CapabilityFlags.PetUse);

        public static bool IsPetSummonAllowed
            =>
                IsPetUsageAllowed && !StyxWoW.Me.GotAlivePet &&
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.PetSummoning) &&
                PetManager.PetSummonAfterDismountTimer.IsFinished;

        public static bool IsDeathGripAllowed
            =>
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.Taunting | CapabilityFlags.GapCloser) &&
                DeathKnightSettings.Instance.DeathGrip;

        public static bool IsFacingAllowed
            => !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && GeneralSettings.Instance.Facing;

        public static bool IsCooldownUsageAllowed
            =>
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.OffensiveCooldowns | CapabilityFlags.DefensiveCooldowns) &&
                GeneralSettings.Instance.Cooldowns;

        public static bool IsInterruptingAllowed
            => !RoutineManager.IsAnyDisallowed(CapabilityFlags.Interrupting) && GeneralSettings.Instance.Interrupts;

        public static bool IsMovingAllowed
            =>
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement) &&
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && GeneralSettings.Instance.Movement;

        public static bool IsRacialUsageAllowed => GeneralSettings.Instance.UseRacials;

        public static bool IsTrinketUsageAllowed
            => GeneralSettings.Instance.UseTrinket1 | GeneralSettings.Instance.UseTrinket2;

        public static bool IsTargetingAllowed
            => !RoutineManager.IsAnyDisallowed(CapabilityFlags.Targeting) && GeneralSettings.Instance.Targeting;

        public static bool IsTauntingAllowed
            =>
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.Taunting) &&
                !RoutineManager.IsAnyDisallowed(CapabilityFlags.GapCloser) && DeathKnightSettings.Instance.DeathGrip;

        public static bool IsAoeAllowed
            => !RoutineManager.IsAnyDisallowed(CapabilityFlags.Aoe) && GeneralSettings.Instance.AoE;
    }
}
