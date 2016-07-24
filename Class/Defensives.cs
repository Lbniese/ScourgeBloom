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

            await Spell.CoCast(SpellLists.DeathPact, Me.HealthPercent < 45 && Capabilities.IsCooldownUsageAllowed);

            await Spell.CoCast(SpellLists.IceboundFortitude, Me,
                Me.HealthPercent < DeathKnightSettings.Instance.UseIceBoundFortitudeHp &&
                DeathKnightSettings.Instance.UseIceBoundFortitude && Capabilities.IsCooldownUsageAllowed);

            if (await Spell.CoCast(SpellLists.DeathSiphon,
                Me.GotTarget && Me.CurrentTarget.Attackable &&
                Me.HealthPercent < DeathKnightSettings.Instance.UseDeathSiphonHp &&
                DeathKnightSettings.Instance.UseDeathSiphon && Me.CurrentTarget.Distance <= 40)) return true;

            if (await Spell.CoCast(SpellLists.Conversion,
                Me.HealthPercent < 50 && Me.RunicPowerPercent >= 20 && !Me.HasAura(SpellLists.Conversion) &&
                Capabilities.IsCooldownUsageAllowed)) return true;

            if (await Spell.CoCast(SpellLists.Conversion,
                Me.HealthPercent > 65 && Me.HasAura(SpellLists.Conversion) && Capabilities.IsCooldownUsageAllowed))
                return true;

            if (await Spell.CoCast(SpellLists.DeathStrike,
                Me.GotTarget && Me.CurrentTarget.Attackable && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HealthPercent < DeathKnightSettings.Instance.UseDeathStrikeHp &&
                DeathKnightSettings.Instance.UseDeathStrike))
                return true;

            return true;
        }
    }
}
