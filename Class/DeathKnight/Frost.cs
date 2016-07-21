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
using Buddy.Coroutines;
using CommonBehaviors.Actions;
using JetBrains.Annotations;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Managers;
using ScourgeBloom.Settings;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Inventory;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using S = ScourgeBloom.Lists.SpellLists;
using TTD = ScourgeBloom.Helpers.TimeToDeath.TimeToDeathExtension;

namespace ScourgeBloom.Class.DeathKnight
{
    [UsedImplicitly]
    public class Frost : ScourgeBloom
    {
        #region Heals

        private static async Task<bool> HealRoutine()
        {
            if (Paused) return false;

            Globals.HealPulsed = true;

            Globals.Update();

            #region Healing Tonic

            if (GeneralSettings.Instance.HealingTonicUse)
                if (await Item.HealingTonic()) return true;

            #endregion Healing Tonic

            #region Healthstone

            if (GeneralSettings.Instance.HealthstoneUse)
                if (await Item.Healthstone()) return true;

            #endregion Healthstone

            return false;
        }

        #endregion Heals

        #region PreCombatBuffs

        private static async Task<bool> PreCombatBuffs()
        {
            if (Paused) return false;

            if (!Me.IsAlive)
                return false;

            if (await Spell.CoCast(S.FrostPresence, Me, !Me.HasAura(S.FrostPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, Me, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            if (GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.Attackable &&
                Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight && Me.IsSafelyFacing(Me.CurrentTarget))
            {
                if (Me.GotTarget && Me.CurrentTarget.Attackable && Me.IsSafelyFacing(Me.CurrentTarget) &&
                    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.Distance > 7 && Me.CurrentTarget.InLineOfSight &&
                    DeathKnightSettings.Instance.DeathGrip)
                    return await Spell.CoCast(S.DeathGrip, SpellManager.CanCast(S.DeathGrip));

                if (Me.GotTarget && Me.CurrentTarget.Attackable && Me.IsSafelyFacing(Me.CurrentTarget) &&
                    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight)
                    return await Spell.CoCast(S.Outbreak, SpellManager.CanCast(S.Outbreak));

                if (Me.GotTarget && Me.CurrentTarget.Attackable && Me.IsSafelyFacing(Me.CurrentTarget) &&
                    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight &&
                    Spell.GetCooldownLeft(S.Outbreak).TotalSeconds > 1)
                    return await Spell.CoCast(S.IcyTouch, SpellManager.CanCast(S.IcyTouch));
            }

            return false;
        }

        #endregion PreCombatBuffs

        #region PullBuffs

#pragma warning disable 1998

        private static async Task<bool> PullBuffs()
#pragma warning restore 1998
        {
            if (Paused) return false;

            return false;
        }

        #endregion PullBuffs

        #region CombatBuffs

        private static async Task<bool> CombatBuffs()
        {
            if (Paused) return false;

            if (!Globals.HealPulsed)
            {
                await HealRoutine();

                if (Globals.HealPulsed)
                {
                    Globals.HealPulsed = false;
                }
            }

            if (Capabilities.IsRacialUsageAllowed)
            {
                await Racials.RacialsMethod();
            }

            if (GeneralSettings.Instance.UseTrinket1 && Capabilities.IsTrinketUsageAllowed)
            {
                await Trinkets.Trinket1Method();
            }

            if (GeneralSettings.Instance.UseTrinket2 && Capabilities.IsTrinketUsageAllowed)
            {
                await Trinkets.Trinket2Method();
            }

            if (SpellManager.GlobalCooldown)
                return false;

            if (await Spell.CoCast(S.FrostPresence, Me, !Me.HasAura(S.FrostPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, Me, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Pull

        public static async Task<bool> PullRoutine()
        {
            if (Paused) return false;

            if (!Me.Combat || Globals.Mounted || !Me.GotTarget || !Me.CurrentTarget.IsAlive || Me.IsCasting ||
                Me.IsChanneling) return false;

            if (Capabilities.IsMovingAllowed)
                await MovementManager.MoveToTarget();

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (!StyxWoW.Me.GotTarget)
                return false;

            // Attack if not attacking
            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
                return false;
            }

            if (!Me.CurrentTarget.IsBoss && Me.CurrentTarget.IsAlive) return false;

         // if (await ExampleOpener(IsDualWielding))
         // {
         //     return false;
         // }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Pull

        #region CombatRoutine

        private static async Task<bool> CombatRoutine(WoWUnit onunit)
        {
            if (Paused) return false;

            var radius = TalentManager.HasGlyph("Blood Boil") ? 15 : 10;

            if (Globals.Mounted || !Me.GotTarget || !Me.CurrentTarget.IsAlive || Me.IsCasting ||
                Me.IsChanneling) return true;

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            if (!Me.Combat) return true;

            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
                return true;
            }


            if (Capabilities.IsRacialUsageAllowed)
            {
                await Racials.RacialsMethod();
            }

            if (GeneralSettings.Instance.UseTrinket1 && Capabilities.IsTrinketUsageAllowed)
            {
                await Trinkets.Trinket1Method();
            }

            if (GeneralSettings.Instance.UseTrinket2 && Capabilities.IsTrinketUsageAllowed)
            {
                await Trinkets.Trinket2Method();
            }

            if (Me.Combat)
            {
                await Defensives.DefensivesMethod();
            }

            if (Capabilities.IsInterruptingAllowed && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.IsCasting &&
                Me.CurrentTarget.CanInterruptCurrentSpellCast)
                await Interrupts.MindFreezeMethod();

            if (Capabilities.IsInterruptingAllowed && Me.CurrentTarget.InLineOfSight && Me.CurrentTarget.Distance <= 30 &&
                Me.CurrentTarget.IsCasting &&
                Me.CurrentTarget.CanInterruptCurrentSpellCast)
                await Interrupts.StrangulateMethod();

            // Actual Routine

            // pillar_of_frost
            await Spell.CoCast(S.PillarofFrost,
                    Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd);

            // sindragosas_fury
            await Spell.CoCast(S.SindragosasFury,
                    Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.Attackable);

            // obliteration -- Buff???

            // breath_of_sindragosa,if=runic_power>=80
            await Spell.CoCast(S.BreathofSindragosa, onunit, Me.GotTarget && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 80);

            // run_action_list,name=bos,if=dot.breath_of_sindragosa.ticking
            if (await single_target_bos(onunit, Me.HasAura("Breath of Sindragosa"))
            {
              return true;
            }

            if (await single_target_2h(onunit, /*Units.EnemiesInRange(10) < 4 && */!IsDualWielding))
            {
                return true;
            }

            if (await single_target_1h(onunit, /*Units.EnemiesInRange(10) < 3 && */IsDualWielding))
            {
                return true;
            }

        // if (await multi_target(onunit, Units.EnemiesInRange(10) >= 4 && !IsDualWielding ||
        //                                (Units.EnemiesInRange(10) >= 3 && IsDualWielding)))
        // {
        //     return true;
        // }

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        private static async Task<bool> single_target_2h(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            // howling_blast,target_if=!dot.frost_fever.ticking
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever))) return true;

            // howling_blast,if=buff.rime.react
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && Me.HasAura(S.AuraRime))) return true;

            // frost_strike,if=runic_power>=80
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 80)) return true;

            // glacial_advance
            if (await Spell.CoCast(S.GlacialAdvance, Me, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // frostscythe,if=buff.killing_machine.react|spell_targets.frostscythe>=4
            if (await Spell.CoCast(S.Frostscythe, onunit, Me.CurrentTarget.Distance <= 8 && (Me.HasAura(S.AuraKillingMachine) || Units.EnemiesInRange(8) >= 4))) return true;

            // obliterate,if=buff.killing_machine.react
            if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraKillingMachine))) return true;

            // remorseless_winter,if=spell_targets.remorseless_winter>=2
            if (await Spell.CoCast(S.RemorselessWinter, Me, Units.EnemiesInRange(4) >= 2)) return true; // How wide is the range?

            // obliterate
            if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // frostscythe,if=talent.frozen_pulse.enabled
            if (await Spell.CoCast(S.Frostscythe, Me, Me.CurrentTarget.Distance <= 8 && Me.FrozenPulseSelected())) return true;

            // howling_blast,if=talent.frozen_pulse.enabled
            if (await Spell.CoCast(S.HowlingBlast, Me, Me.CurrentTarget.Distance <= 30 && Me.FrozenPulseSelected())) return true;

            // frost_strike,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

            // frost_strike,if=!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && !BoSSelected())) return true;

            // horn_of_winter,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
            if (await Spell.CoCast(S.HornofWinter, Me, BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

            // empower_rune_weapon,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
            if (await Spell.CoCast(S.EmpowerRuneWeapon, Me, BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

            // hungering_rune_weapon,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
            if (await Spell.CoCast(S.HungeringRuneWeapon, Me, HungeringRuneWeaponSelected() && Me.GotTarget && Me.CurrentTarget.Distance <= 8 && BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

            // horn_of_winter,if=!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.HornofWinter, Me, !BoSSelected())) return true;

            // empower_rune_weapon,if=!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.EmpowerRuneWeapon, Me, !BoSSelected())) return true;

            // hungering_rune_weapon,if=!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.HungeringRuneWeapon, Me, HungeringRuneWeaponSelected() && Me.GotTarget && Me.CurrentTarget.Distance <= 8 && !BoSSelected())) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> single_target_1h(WoWUnit onunit, bool reqs)
        {
          if (!reqs) return false;
          // howling_blast,target_if=!dot.frost_fever.ticking
          if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever))) return true;

          // howling_blast,if=buff.rime.react
          if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && Me.HasAura(S.AuraRime))) return true;

          // frost_strike,if=runic_power>=80
          if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 80)) return true;

          // glacial_advance
          if (await Spell.CoCast(S.GlacialAdvance, Me, Me.CurrentTarget.IsWithinMeleeRange)) return true;

          // frostscythe,if=buff.killing_machine.react|spell_targets.frostscythe>=4
          if (await Spell.CoCast(S.Frostscythe, onunit, Me.CurrentTarget.Distance <= 8 && (Me.HasAura(S.AuraKillingMachine) || Units.EnemiesInRange(8) >= 4))) return true;

          // obliterate,if=buff.killing_machine.react
          if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraKillingMachine))) return true;

          // remorseless_winter,if=spell_targets.remorseless_winter>=2
          if (await Spell.CoCast(S.RemorselessWinter, Me, Units.EnemiesInRange(4) >= 2)) return true; // How wide is the range?

          // obliterate
          if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange)) return true;

          // frostscythe,if=talent.frozen_pulse.enabled
          if (await Spell.CoCast(S.Frostscythe, Me, Me.CurrentTarget.Distance <= 8 && Me.FrozenPulseSelected())) return true;

          // howling_blast,if=talent.frozen_pulse.enabled
          if (await Spell.CoCast(S.HowlingBlast, Me, Me.CurrentTarget.Distance <= 30 && Me.FrozenPulseSelected())) return true;

          // frost_strike,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
          if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

          // frost_strike,if=!talent.breath_of_sindragosa.enabled
          if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && !BoSSelected())) return true;

          // horn_of_winter,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
          if (await Spell.CoCast(S.HornofWinter, Me, BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

          // empower_rune_weapon,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
          if (await Spell.CoCast(S.EmpowerRuneWeapon, Me, BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

          // hungering_rune_weapon,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains>15
          if (await Spell.CoCast(S.HungeringRuneWeapon, Me, HungeringRuneWeaponSelected() && Me.GotTarget && Me.CurrentTarget.Distance <= 8 && BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 15)) return true;

          // horn_of_winter,if=!talent.breath_of_sindragosa.enabled
          if (await Spell.CoCast(S.HornofWinter, Me, !BoSSelected())) return true;

          // empower_rune_weapon,if=!talent.breath_of_sindragosa.enabled
          if (await Spell.CoCast(S.EmpowerRuneWeapon, Me, !BoSSelected())) return true;

          // hungering_rune_weapon,if=!talent.breath_of_sindragosa.enabled
          if (await Spell.CoCast(S.HungeringRuneWeapon, Me, HungeringRuneWeaponSelected() && Me.GotTarget && Me.CurrentTarget.Distance <= 8 && !BoSSelected())) return true;

          await CommonCoroutines.SleepForLagDuration();

          return true;
        }

        private static async Task<bool> multi_target(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // Not enough info yet

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> single_target_bos(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            / glacial_advance
            if (await Spell.CoCast(S.GlacialAdvance, Me, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // frostscythe,if=buff.killing_machine.react|spell_targets.frostscythe>=4
            if (await Spell.CoCast(S.Frostscythe, onunit, Me.CurrentTarget.Distance <= 8 && (Me.HasAura(S.AuraKillingMachine) || Units.EnemiesInRange(8) >= 4))) return true;

            // obliterate,if=buff.killing_machine.react
            if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraKillingMachine))) return true;

            // remorseless_winter,if=spell_targets.remorseless_winter>=2
            if (await Spell.CoCast(S.RemorselessWinter, Me, Units.EnemiesInRange(4) >= 2)) return true; // How wide is the range?

            // obliterate
            if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // frostscythe,if=talent.frozen_pulse.enabled
            if (await Spell.CoCast(S.Frostscythe, Me, Me.CurrentTarget.Distance <= 8 && Me.FrozenPulseSelected())) return true;

            // howling_blast,if=talent.frozen_pulse.enabled
            if (await Spell.CoCast(S.HowlingBlast, Me, Me.CurrentTarget.Distance <= 30 && Me.FrozenPulseSelected())) return true;

            // howling_blast,target_if=!dot.frost_fever.ticking
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever))) return true;

            // horn_of_winter
            if (await Spell.CoCast(S.HornofWinter, Me, Me.GotTarget)) return true;

            // empower_rune_weapon
            if (await Spell.CoCast(S.EmpowerRuneWeapon,
                Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget
                && Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.BloodRuneCount < 1 && Me.DeathRuneCount < 1)) return true;

            // hungering_rune_weapon
            if (await Spell.CoCast(S.HungeringRuneWeapon, Me, HungeringRuneWeaponSelected() && Me.GotTarget && Me.CurrentTarget.Distance <= 8)) return true;

            // howling_blast,if=buff.rime.react
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Distance <= 30 && Me.HasAura(S.AuraRime))) return true;


            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> multi_target_bos(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // Not enough info yet

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion CombatRoutine

        #region RestCoroutine

        private static async Task<bool> RestCoroutine()
        {
            if (Paused) return false;

            if (!GeneralSettings.Instance.RestingEatFood) return false;

            if (Me.IsDead || SpellManager.GlobalCooldown || !CanBuffEat()) return false;

            if (!(Me.HealthPercent < GeneralSettings.Instance.RestingEatFoodHp) || Me.IsMoving || Me.IsCasting ||
                Me.Combat || Me.HasAura("Food") ||
                Consumable.GetBestFood(false) == null)
                return false;

            Styx.CommonBot.Rest.FeedImmediate();

            await CommonCoroutines.SleepForLagDuration();

            return await Coroutine.Wait(1000, () => Me.HasAura("Food"));
        }

        public static bool CanBuffEat()
        {
            return !Me.Mounted && !Me.IsDead && !Me.IsGhost && !Me.IsOnTransport && !Me.OnTaxi;
        }

        #endregion RestCoroutine

        #region Logics

        #region IsDualWielding

        private static bool IsDualWielding
            => Me.Inventory.Equipped.MainHand != null && Me.Inventory.Equipped.OffHand != null;

        #endregion IsDualWielding

        #region ShouldSpreadDiseases

        public static bool ShouldSpreadDiseases()
        {
            var radius = TalentManager.HasGlyph("Blood Boil") ? 15 : 10;
            return !Me.CurrentTarget.HasAuraExpired("Blood Plague")
                   && !Me.CurrentTarget.HasAuraExpired("Frost Fever")
                   &&
                   Units.EnemyUnitsNearTarget(10)
                       .Any(
                           u =>
                               Me.CurrentTarget.Distance < radius && u.HasAuraExpired("Blood Plague") &&
                               u.HasAuraExpired("Frost Fever"));
        }

        #endregion ShouldSpreadDiseases

        #region NeedToSpread

        public static bool NeedToSpread()
        {
            if ((!StyxWoW.Me.CurrentTarget.HasAura(S.AuraBloodPlague) ||
                 !StyxWoW.Me.CurrentTarget.HasAura(S.AuraFrostFever)) && !NecroticPlagueSelected() ||
                (!StyxWoW.Me.CurrentTarget.HasAura(S.AuraNecroticPlague) && NecroticPlagueSelected()))
                return false;
            var mobList =
                ObjectManager.GetObjectsOfType<WoWUnit>()
                    .FindAll(
                        unit =>
                            unit.Guid != StyxWoW.Me.Guid && unit.IsAlive && unit.IsHostile && SpreadHelper(unit) &&
                            unit.Attackable && !unit.IsFriendly &&
                            (unit.Location.Distance(StyxWoW.Me.CurrentTarget.Location) <= 10 ||
                             unit.Location.Distance2D(StyxWoW.Me.CurrentTarget.Location) <= 10));

            var playerList =
                ObjectManager.GetObjectsOfType<WoWPlayer>()
                    .FindAll(
                        unit =>
                            unit.Guid != StyxWoW.Me.Guid && unit.IsAlive && unit.IsHostile && SpreadHelper(unit) &&
                            unit.Attackable && !unit.IsFriendly &&
                            (unit.Location.Distance(StyxWoW.Me.CurrentTarget.Location) <= 10 ||
                             unit.Location.Distance2D(StyxWoW.Me.CurrentTarget.Location) <= 10));

            return mobList.Count + playerList.Count > 1;
        }

        private static bool SpreadHelper(WoWUnit p)
        {
            var auras = p.GetAllAuras();
            return
                auras.Any(
                    a =>
                        a.SpellId != S.AuraBloodPlague || a.SpellId != S.AuraFrostFever ||
                        a.SpellId != S.AuraNecroticPlague);
        }

        #endregion NeedToSpread

        #region CanPlagueLeech

        public static bool CanPlagueLeech()
        {
            return (Me.BloodRuneCount < 1 && Me.FrostRuneCount < 1 ||
                    Me.BloodRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                    Me.FrostRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                    Me.DeathRuneCount < 1 && Me.BloodRuneCount < 1 ||
                    Me.DeathRuneCount < 1 && Me.FrostRuneCount < 1 ||
                    Me.DeathRuneCount < 1 && Me.UnholyRuneCount < 1)
                   && Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                   Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && SpellManager.CanCast(S.PlagueLeech);
        }

        #endregion CanPlagueLeech

        #region GoodPlagueLeech

        public static bool GoodPlagueLeech()
        {
            if (!StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return false;

            var ffTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraFrostFever).TimeLeft;
            var bpTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraBloodPlague).TimeLeft;

            return ffTime <= TimeSpan.FromSeconds(3) || bpTime <= TimeSpan.FromSeconds(3);
        }

        #endregion GoodPlagueLeech

        #region DiseaseRemains

        public static bool DiseaseRemainsLessThanOne()
        {
            if (!StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return false;

            var ffTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraFrostFever).TimeLeft;
            var bpTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraBloodPlague).TimeLeft;

            return ffTime < TimeSpan.FromSeconds(1) || bpTime < TimeSpan.FromSeconds(1);
        }

        public static bool DiseaseRemainsLessThanThree()
        {
            if (!StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return false;

            var ffTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraFrostFever).TimeLeft;
            var bpTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraBloodPlague).TimeLeft;

            return ffTime < TimeSpan.FromSeconds(3) || bpTime < TimeSpan.FromSeconds(3);
        }

        public static bool DiseaseRemainsMoreThanFive()
        {
            if (!StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return false;

            var ffTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraFrostFever).TimeLeft;
            var bpTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraBloodPlague).TimeLeft;

            return ffTime > TimeSpan.FromSeconds(5) || bpTime > TimeSpan.FromSeconds(5);
        }

        #endregion DiseaseRemains

        #region NeedToExtendNecroticPlague

        public static bool NeedToExtendNecroticPlague()
        {
            if (BoSSelected() || DefileSelected()) return false;
            if (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague)) return false;

            var npTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraNecroticPlague).TimeLeft;
            var ubTime = Spell.GetCooldownLeft(S.UnholyBlight).TotalSeconds;

            return npTime < TimeSpan.FromSeconds(ubTime);
        }

        #endregion NeedToExtendNecroticPlague

        #region TalentSelected

        public static bool FrozenPulseSelected()
        {
            return TalentManager.IsSelected(5);
        }

        public static bool HungeringRuneWeaponSelected()
        {
            return TalentManager.IsSelected(8);
        }

        public static bool BoSSelected()
        {
            return TalentManager.IsSelected(20);
        }

        public static bool GlacialAdvanceSelected()
        {
            return TalentManager.IsSelected(21);
        }

        #endregion TalentSelected

        #endregion Logics

        #region Overrides

        public override WoWClass Class
            => Me.Specialization == WoWSpec.DeathKnightFrost ? WoWClass.DeathKnight : WoWClass.None;

        protected override Composite CreateCombat()
        {
            return new ActionRunCoroutine(ret => CombatRoutine(Me.CurrentTarget));
        }

        protected override Composite CreatePreCombatBuff()
        {
            return new ActionRunCoroutine(ret => PreCombatBuffs());
        }

        protected override Composite CreatePullBuff()
        {
            return new ActionRunCoroutine(ret => PullBuffs());
        }

        protected override Composite CreateCombatBuff()
        {
            return new ActionRunCoroutine(ret => CombatBuffs());
        }

        protected override Composite CreatePull()
        {
            return new ActionRunCoroutine(ret => PullRoutine());
        }

        protected override Composite CreateRest()
        {
            return new ActionRunCoroutine(ret => RestCoroutine());
        }

        protected override Composite CreateHeal()
        {
            return new ActionRunCoroutine(ret => HealRoutine());
        }

        #endregion Overrides

        #region Openers

        private static async Task<bool> ExampleOpener(bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;
            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed)) return true;


            if (await Spell.CoCast(S.PillarofFrost,
                        Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd)) return true;

            if (await Spell.CoCast(S.Outbreak)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Openers
    }
}
