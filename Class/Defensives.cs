/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Linq;
using System.Threading.Tasks;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;

namespace ScourgeBloom.Class
{
    internal class Defensives : ScourgeBloom
    {
        public static async Task<bool> DefensivesMethod()
        {
            if (!Capabilities.IsCooldownUsageAllowed)
                return false;

            await Spell.CoCast(SpellLists.AntiMagicShell, Me,
                Units.EnemyUnitsNearTarget(10)
                    .Any(u => (u.IsCasting || u.ChanneledCastingSpellId != 0) && u.CurrentTargetGuid == Me.Guid) &&
                Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAms);

            await Spell.CoCast(SpellLists.IceboundFortitude, Me,
                Me.HealthPercent < DeathKnightSettings.Instance.UseIceBoundFortitudeHp &&
                DeathKnightSettings.Instance.UseIceBoundFortitude && Capabilities.IsCooldownUsageAllowed);

            if (await Spell.CoCast(SpellLists.DeathStrike,
                Me.GotTarget && Me.CurrentTarget.CanWeAttack() && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HealthPercent < DeathKnightSettings.Instance.UseDeathStrikeHp &&
                DeathKnightSettings.Instance.UseDeathStrike))
                return true;

            return true;
        }
    }
}