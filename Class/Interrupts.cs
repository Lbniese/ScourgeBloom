/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Threading.Tasks;
using System.Windows.Media;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using S = ScourgeBloom.Lists.SpellLists;

namespace ScourgeBloom.Class
{
    public class Interrupts : ScourgeBloom
    {
        public static async Task<bool> MindFreezeMethod()
        {
            if (!Capabilities.IsInterruptingAllowed)
                return false;

            if (WoWSpell.FromId(S.MindFreeze).Cooldown)
                return false;

            if (!StyxWoW.Me.GotTarget)
                return false;

            if (Globals.CurrentTarget.IsPet)
                return false;

            if (!Globals.CurrentTarget.IsCasting)
                return false;

            if (!Globals.CurrentTarget.CanInterruptCurrentSpellCast)
                return false;

            // Get the spell ID our target is casting
            var spell = Globals.CurrentTarget.CastingSpell;

            if (spell == null)
                return false;

            var castTime = Globals.CurrentTarget.CastingSpell.CastTime;
            var timeLeft = Globals.CurrentTarget.CurrentCastTimeLeft.TotalMilliseconds;

            var percentage = 100 - timeLeft*100/castTime;

            if (DeathKnightSettings.Instance.MindFreezeRandomTimerUse)
            {
                if (percentage > DeathKnightSettings.Instance.MindFreezeRandomTimerMax)
                    return false;

                if (percentage < DeathKnightSettings.Instance.MindFreezeRandomTimerMin)
                    return false;


                if (!await Spell.CoCast(S.MindFreeze))
                    return false;

                Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Interrupting {0} with Mind Freeze",
                    Globals.CurrentTarget.SafeName);

                return true;
            }

            if (!await Spell.CoCast(S.MindFreeze))
                return false;

            Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Interrupting {0} with Mind Freeze",
                Globals.CurrentTarget.SafeName);

            return true;
        }

        public static async Task<bool> StrangulateMethod()
        {
            if (!Capabilities.IsInterruptingAllowed)
                return false;

            if (WoWSpell.FromId(S.Strangulate).Cooldown)
                return false;

            if (!StyxWoW.Me.GotTarget)
                return false;

            if (Globals.CurrentTarget.IsPet)
                return false;

            if (!Globals.CurrentTarget.IsCasting)
                return false;

            if (!Globals.CurrentTarget.CanInterruptCurrentSpellCast)
                return false;

            // Get the spell ID our target is casting
            var spell = Globals.CurrentTarget.CastingSpell;

            if (spell == null)
                return false;

            var castTime = Globals.CurrentTarget.CastingSpell.CastTime;
            var timeLeft = Globals.CurrentTarget.CurrentCastTimeLeft.TotalMilliseconds;

            var percentage = 100 - timeLeft*100/castTime;

            if (DeathKnightSettings.Instance.StrangulateRandomTimerUse)
            {
                if (percentage > DeathKnightSettings.Instance.StrangulateRandomTimerMax)
                    return false;

                if (percentage < DeathKnightSettings.Instance.StrangulateRandomTimerMin)
                    return false;


                if (!await Spell.CoCast(S.Strangulate))
                    return false;

                Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Interrupting {0} with Strangulate!",
                    Globals.CurrentTarget.SafeName);

                return true;
            }

            if (!await Spell.CoCast(S.Strangulate))
                return false;

            Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Interrupting {0} with Strangulate!",
                Globals.CurrentTarget.SafeName);

            return true;
        }

        private static bool InterruptCheck(WoWUnit unit, double millisecondsleft, bool includeUninterruptable = true)
        {
            return unit != null && unit.IsValid && unit.Combat && (unit.IsCasting || unit.IsChanneling) &&
                   (includeUninterruptable || unit.CanInterruptCurrentSpellCast) &&
                   (!unit.IsCasting || !(unit.CurrentCastTimeLeft.TotalMilliseconds > millisecondsleft));
        }
    }
}