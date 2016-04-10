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
            return false;
        }

        #endregion PullBuffs

        #region CombatBuffs

        private static async Task<bool> CombatBuffs()
        {
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

            //await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Pull

        public static async Task<bool> PullRoutine()
        {
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
            if (await NecroBlightOpener(TalentManager.IsSelected(19) && IsDualWielding))
            {
                return false;
            }

            if (await DefileOpener(TalentManager.IsSelected(20) && IsDualWielding))
            {
                return false;
            }

            if (await ThOpener(IsDualWielding))
            {
                return false;
            }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Pull

        #region CombatRoutine

        private static async Task<bool> CombatRoutine(WoWUnit onunit)
        {
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
            if (await Spell.CoCast(S.ArmyoftheDead, Me, Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD))
                return true;

            //actions+=/pillar_of_frost
            await Spell.CoCast(S.PillarofFrost, Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd); //Add PoF on Cooldown check
            
            //actions+=/empower_rune_weapon,if=target.time_to_die<=60
            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                TTD.TimeToDeath(onunit) <= 60 && Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            //actions+=/plague_leech,if=disease.min_remains<1
            if (await Spell.CoCast(S.PlagueLeech, onunit, CanPlagueLeech() && DiseaseRemainsLessThanOne()))
                return true;

            //actions+=/soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<=35
            if (await Spell.CoCast(S.SoulReaperFrost, onunit, Me.CurrentTarget.HealthPercent <= 37))
                return true;

            //actions+=/blood_tap,if=(target.health.pct-3*(target.health.pct%target.time_to_die)<=35&cooldown.soul_reaper.remains=0)
            await Spell.CoCast(S.BloodTap, Me,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.SoulReaperFrost) && Me.CurrentTarget.HealthPercent < 45 &&
                Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.DeathRuneCount == 0);

            if (await single_target_2h(onunit, Units.EnemyUnitsSub10.Count() < 4 && !IsDualWielding))
            {
                return true;
            }

            if (await single_target_1h(onunit, Units.EnemyUnitsSub10.Count() < 3 && IsDualWielding))
            {
                return true;
            }

            if (await multi_target(onunit, Units.EnemyUnitsSub10.Count() >= 4 && !IsDualWielding ||
                                           (Units.EnemyUnitsSub10.Count() >= 3 && IsDualWielding)))
            {
                return true;
            }

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
            //actions.single_target_2h=defile
            if (await Spell.CastOnGround(S.Defile, Me,
                Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && DefileSelected()))
                return true;

            //actions.single_target_2h+=/blood_tap,if=talent.defile.enabled&cooldown.defile.remains=0
            await Spell.CoCast(S.BloodTap, Me,
                DefileSelected() && SpellManager.CanCast(S.BloodTap) && Me.HasAura(S.AuraBloodCharge) &&
                Me.Auras["Blood Charge"].StackCount >= 5 && Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 &&
                Me.FrostRuneCount == 0 && Me.DeathRuneCount == 0 && Spell.GetCooldownLeft(S.Defile).TotalSeconds < 1);

            //actions.single_target_2h+=/howling_blast,if=buff.rime.react&disease.min_remains>5&buff.killing_machine.react
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                Me.HasAura("Rime") && DiseaseRemainsMoreThanFive() && Me.HasAura(S.AuraKillingMachine)))
                return true;

            //actions.single_target_2h+=/obliterate,if=buff.killing_machine.react
            if (await Spell.CoCast(S.Obliterate, onunit, Me.HasAura(S.AuraKillingMachine))) return true;

            //actions.single_target_2h+=/blood_tap,if=buff.killing_machine.react
            await Spell.CoCast(S.BloodTap, Me,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                Me.HasAura(S.AuraKillingMachine) && Me.UnholyRuneCount == 0 &&
                Me.BloodRuneCount == 0 && Me.FrostRuneCount == 0 && Me.DeathRuneCount == 0);

            //CUSTOM 
            if (await Spell.CoCast(S.PlagueStrike, onunit, !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;
            
            //actions.single_target_2h+=/howling_blast,if=!talent.necrotic_plague.enabled&!dot.frost_fever.ticking&buff.rime.react
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                !Me.CurrentTarget.HasMyAura(S.AuraFrostFever) && Me.HasAura("Rime") &&
                !NecroticPlagueSelected())) return true;

            //actions.single_target_2h+=/outbreak,if=!disease.max_ticking
            if (await Spell.CoCast(S.Outbreak, onunit,
                ((!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) || !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))
                 && !NecroticPlagueSelected()) || (Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                 Me.CurrentTarget.Auras["Necrotic Plague"].StackCount < 15 && NecroticPlagueSelected()))) return true;

            //actions.single_target_2h+=/unholy_blight,if=!disease.min_ticking
            if (await Spell.CoCast(S.UnholyBlight, onunit,
                !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) ||
                Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                Me.CurrentTarget.Auras["Necrotic Plague"].StackCount <= 5 ||
                !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                !Me.CurrentTarget.HasMyAura(S.AuraFrostFever) && !NecroticPlagueSelected()))
                return true;

            //actions.single_target_2h+=/breath_of_sindragosa,if=runic_power>75
            await Spell.CoCast(S.BreathofSindragosa, onunit, Me.CurrentRunicPower > 75);

            //actions.single_target_2h+=/run_action_list,name=single_target_bos,if=dot.breath_of_sindragosa.ticking
            await single_target_bos(onunit, Me.HasAura("Breath of Sindragosa"));

            //actions.single_target_2h+=/obliterate,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains<7&runic_power<76
            if (await Spell.CoCast(S.Obliterate, onunit,
                BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds < 7 &&
                Me.CurrentRunicPower < 76)) return true;

            //actions.single_target_2h+=/howling_blast,if=talent.breath_of_sindragosa.enabled&cooldown.breath_of_sindragosa.remains<3&runic_power<88
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds < 3 &&
                Me.CurrentRunicPower < 88)) return true;

            //actions.single_target_2h+=/howling_blast,if=!talent.necrotic_plague.enabled&!dot.frost_fever.ticking
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever))) return true;

            //actions.single_target_2h+=/howling_blast,if=talent.necrotic_plague.enabled&!dot.necrotic_plague.ticking
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return true;

            //actions.single_target_2h+=/plague_strike,if=!talent.necrotic_plague.enabled&!dot.blood_plague.ticking
            if (await Spell.CoCast(S.PlagueStrike, onunit, !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            //actions.single_target_2h+=/blood_tap,if=buff.blood_charge.stack>10&runic_power>76
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 10 &&
                SpellManager.CanCast(S.BloodTap) && Me.CurrentRunicPower > 76);

            //actions.single_target_2h+=/frost_strike,if=runic_power>76
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentRunicPower > 76)) return true;

            //actions.single_target_2h+=/howling_blast,if=buff.rime.react&disease.min_remains>5&(blood.frac>=1.8|unholy.frac>=1.8|frost.frac>=1.8)
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.HasAura(S.AuraRime) && DiseaseRemainsMoreThanFive() && (Me.BloodRuneCount >= 2 || Me.UnholyRuneCount >= 2 || Me.FrostRuneCount >= 2)))
            return true; // Should be 1.8

            //actions.single_target_2h+=/obliterate,if=blood.frac>=1.8|unholy.frac>=1.8|frost.frac>=1.8
            if (await Spell.CoCast(S.Obliterate, onunit,
                Me.BloodRuneCount >= 2 || Me.UnholyRuneCount >= 2 || Me.FrostRuneCount >= 2)) return true;

            //actions.single_target_2h+=/plague_leech,if=disease.min_remains<3&((blood.frac<=0.95&unholy.frac<=0.95)|(frost.frac<=0.95&unholy.frac<=0.95)|(frost.frac<=0.95&blood.frac<=0.95))
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                DiseaseRemainsLessThanThree() && (Me.BloodRuneCount <= 1 && Me.UnholyRuneCount <= 1) ||
                (Me.FrostRuneCount <= 1 && Me.UnholyRuneCount <= 1) ||
                (Me.FrostRuneCount <= 1 && Me.BloodRuneCount <= 1))) return true; // Should be 0.95

            //actions.single_target_2h+=/frost_strike,if=talent.runic_empowerment.enabled&(frost=0|unholy=0|blood=0)&(!buff.killing_machine.react|!obliterate.ready_in<=1)
            if (await Spell.CoCast(S.FrostStrike, onunit,
                RunicEmpowermentSelected() &&
                (Me.FrostRuneCount == 0 || Me.UnholyRuneCount == 0 || Me.BloodRuneCount == 0) &&
                (!Me.HasAura(S.AuraKillingMachine) || Spell.GetCooldownLeft(S.Obliterate).TotalSeconds <= 1)))
                return true;
            
            //actions.single_target_2h+=/frost_strike,if=talent.blood_tap.enabled&buff.blood_charge.stack<=10&(!buff.killing_machine.react|!obliterate.ready_in<=1)
            if (await Spell.CoCast(S.FrostStrike, onunit,
                BloodTapSelected() && Me.HasAura(S.AuraBloodCharge)
                && Me.Auras["Blood Charge"].StackCount <= 10 &&
                (Me.HasAura(S.AuraKillingMachine) || Spell.GetCooldownLeft(S.Obliterate).TotalSeconds <= 1)))
                return true;

            //actions.single_target_2h+=/howling_blast,if=buff.rime.react&disease.min_remains>5
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.HasAura("Rime") && DiseaseRemainsMoreThanFive()))
                return true;

            //actions.single_target_2h+=/obliterate,if=blood.frac>=1.5|unholy.frac>=1.6|frost.frac>=1.6|buff.bloodlust.up|cooldown.plague_leech.remains<=4
            if (await Spell.CoCast(S.Obliterate, onunit,
                Me.BloodRuneCount >= 2 || Me.UnholyRuneCount >= 2 || Me.FrostRuneCount >= 2 ||
                Me.HasPartyBuff(Units.Stat.BurstHaste) || Spell.GetCooldownLeft(S.PlagueLeech).TotalSeconds <= 4))
                return true;

            //actions.single_target_2h+=/blood_tap,if=(buff.blood_charge.stack>10&runic_power>=20)|(blood.frac>=1.4|unholy.frac>=1.6|frost.frac>=1.6)
            await Spell.CoCast(S.BloodTap, Me,
                (Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount > 10 &&
                 Me.CurrentRunicPower >= 20) || (Me.BloodRuneCount == 2 ||
                 Me.UnholyRuneCount == 2 || Me.FrostRuneCount == 2));

            //actions.single_target_2h+=/frost_strike,if=!buff.killing_machine.react
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.HasAura(S.AuraKillingMachine))) return true;

            //actions.single_target_2h+=/plague_leech,if=(blood.frac<=0.95&unholy.frac<=0.95)|(frost.frac<=0.95&unholy.frac<=0.95)|(frost.frac<=0.95&blood.frac<=0.95)
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                (Me.BloodRuneCount <= 1 && Me.UnholyRuneCount <= 1) ||
                (Me.FrostRuneCount <= 1 && Me.UnholyRuneCount <= 1) ||
                (Me.FrostRuneCount <= 1 && Me.BloodRuneCount <= 1))) return true;

            //actions.single_target_2h+=/empower_rune_weapon
            await Spell.CoCast(S.EmpowerRuneWeapon, Me, Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> single_target_1h(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            //actions.single_target_1h=breath_of_sindragosa,if=runic_power>75
            await Spell.CoCast(S.BreathofSindragosa, onunit, Me.CurrentRunicPower > 75);

            //actions.single_target_1h+=/run_action_list,name=single_target_bos,if=dot.breath_of_sindragosa.ticking
            await single_target_bos(onunit, Me.HasAura("Breath of Sindragosa"));

            //actions.single_target_1h+=/frost_strike,if=buff.killing_machine.react
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.HasAura(S.AuraKillingMachine))) return true;

            //actions.single_target_1h+=/obliterate,if=unholy>1|buff.killing_machine.react
            if (await Spell.CoCast(S.Obliterate, onunit, Me.UnholyRuneCount > 1)) return true;
            if (await Spell.CoCast(S.Obliterate, onunit, Me.HasAura(S.AuraKillingMachine))) return true;

            //actions.single_target_1h+=/defile
            if (await Spell.CastOnGround(S.Defile, Me, DefileSelected() && Me.Combat && Me.CurrentTarget.Attackable)) return true;

            //actions.single_target_1h+=/blood_tap,if=talent.defile.enabled&cooldown.defile.remains=0
            await Spell.CoCast(S.BloodTap, onunit,
                DefileSelected() && SpellManager.CanCast(S.BloodTap) && Me.HasAura(S.AuraBloodCharge) &&
                Me.Auras["Blood Charge"].StackCount >= 5 && Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 &&
                Me.FrostRuneCount == 0 && Me.DeathRuneCount == 0 && Spell.GetCooldownLeft(S.Defile).TotalSeconds < 1);

            //actions.single_target_1h+=/frost_strike,if=runic_power>88
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentRunicPower > 88)) return true;

            //actions.single_target_1h+=/howling_blast,if=buff.rime.react|death>1|frost>1
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.HasAura(S.AuraRime))) return true;
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.DeathRuneCount > 1)) return true;
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.FrostRuneCount > 1)) return true;

            //actions.single_target_1h+=/blood_tap,if=buff.blood_charge.stack>10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 10 &&
                SpellManager.CanCast(S.BloodTap));

            //actions.single_target_1h+=/frost_strike,if=runic_power>76
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentRunicPower > 76)) return true;

            //actions.single_target_1h+=/unholy_blight,if=!disease.ticking
            if (await Spell.CoCast(S.UnholyBlight, onunit,
                SpellManager.HasSpell(S.UnholyBlight) &&
                (!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                 !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)) && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            if (await Spell.CoCast(S.UnholyBlight, onunit,
                SpellManager.HasSpell(S.UnholyBlight) &&
                (!Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) ||
                 Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                 Me.CurrentTarget.Auras["Necrotic Plague"].StackCount <= 5)
                && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            //actions.single_target_1h+=/outbreak,if=!dot.blood_plague.ticking
            if (await Spell.CoCast(S.Outbreak, onunit,
                !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            //actions.single_target_1h+=/plague_strike,if=!talent.necrotic_plague.enabled&!dot.blood_plague.ticking
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))) return true;

            //actions.single_target_1h+=/howling_blast,if=!(target.health.pct-3*(target.health.pct%target.time_to_die)<=35&cooldown.soul_reaper.remains<3)|death+frost>=2
            if (await Spell.CoCast(S.HowlingBlast, onunit, /*Missing logic*/Me.DeathRuneCount + Me.FrostRuneCount >= 2))
                return true;

            //actions.single_target_1h+=/outbreak,if=talent.necrotic_plague.enabled&debuff.necrotic_plague.stack<=14
            if (await Spell.CoCast(S.Outbreak, onunit,
                NecroticPlagueSelected() && Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                Me.CurrentTarget.Auras["Necrotic Plague"].StackCount <= 14)) return true;

            //actions.single_target_1h+=/blood_tap
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            //actions.single_target_1h+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            //actions.single_target_1h+=/empower_rune_weapon
            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.BloodRuneCount < 1 &&
                Me.DeathRuneCount < 1 && Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> multi_target(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            //actions.multi_target=unholy_blight
            await Spell.CoCast(S.UnholyBlight, onunit, Me.CurrentTarget.IsWithinMeleeRange);

            //actions.multi_target+=/frost_strike,if=buff.killing_machine.react&main_hand.1h
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.HasAura(S.AuraKillingMachine) && IsDualWielding)) return true;

            //actions.multi_target+=/obliterate,if=unholy>1
            if (await Spell.CoCast(S.Obliterate, onunit, Me.UnholyRuneCount > 1)) return true;
            //actions.multi_target+=/blood_boil,if=dot.blood_plague.ticking&(!talent.unholy_blight.enabled|cooldown.unholy_blight.remains<49),line_cd=28
            if (await Spell.CoCast(S.BloodBoil, Me,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) &&
                (!UnholyBlightSelected() || Spell.GetCooldownLeft(S.UnholyBlight).TotalSeconds < 49)))
                return true;

            //actions.multi_target+=/defile
            if (await Spell.CastOnGround(S.Defile, Me, DefileSelected())) return true;

            //actions.multi_target+=/breath_of_sindragosa,if=runic_power>75
            await Spell.CoCast(S.BreathofSindragosa, onunit, Me.CurrentRunicPower > 75);

            //actions.multi_target+=/run_action_list,name=multi_target_bos,if=dot.breath_of_sindragosa.ticking
            await multi_target_bos(onunit, Me.HasAura("Breath of Sindragosa"));

            //actions.multi_target+=/howling_blast
            if (await Spell.CoCast(S.HowlingBlast, onunit, Me.CurrentTarget.Attackable)) return true;

            //actions.multi_target+=/blood_tap,if=buff.blood_charge.stack>10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount > 10 &&
                SpellManager.CanCast(S.BloodTap));

            //actions.multi_target+=/frost_strike,if=runic_power>88
            if (await Spell.CoCast(S.FrostStrike, onunit, Me.CurrentRunicPower > 88)) return true;

            //actions.multi_target+=/death_and_decay,if=unholy=1
            if (await Spell.CastOnGround(S.DeathandDecay, Me, Me.UnholyRuneCount == 1)) return true;

            //actions.multi_target+=/plague_strike,if=unholy=2&!dot.blood_plague.ticking&!talent.necrotic_plague.enabled
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                Me.UnholyRuneCount == 2 && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) &&
                !NecroticPlagueSelected())) return true;

            //actions.multi_target+=/blood_tap
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            //actions.multi_target+=/frost_strike,if=!talent.breath_of_sindragosa.enabled|cooldown.breath_of_sindragosa.remains>=10
            if (await Spell.CoCast(S.FrostStrike, onunit,
                !BoSSelected() ||
                BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds >= 10)) return true;

            //actions.multi_target+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            //actions.multi_target+=/plague_strike,if=unholy=1
            if (await Spell.CoCast(S.PlagueStrike, onunit, Me.UnholyRuneCount == 1)) return true;

            //actions.multi_target+=/empower_rune_weapon
            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.BloodRuneCount < 1 &&
                Me.DeathRuneCount < 1 && Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> single_target_bos(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            // actions.single_target_bos=obliterate,if=buff.killing_machine.react
            if (await Spell.CoCast(S.Obliterate, onunit, Me.HasAura(S.AuraKillingMachine))) return true;

            // actions.single_target_bos+=/blood_tap,if=buff.killing_machine.react&buff.blood_charge.stack>=5
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraKillingMachine) && Me.HasAura(S.AuraBloodCharge) &&
                Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.single_target_bos+=/plague_leech,if=buff.killing_machine.react
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                Me.HasAura(S.AuraKillingMachine) && CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            // actions.single_target_bos+=/blood_tap,if=buff.blood_charge.stack>=5
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.single_target_bos+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            // actions.single_target_bos+=/obliterate,if=runic_power<76
            if (await Spell.CoCast(S.Obliterate, onunit, Me.CurrentRunicPower < 76)) return true;

            // actions.single_target_bos+=/howling_blast,if=((death=1&frost=0&unholy=0)|death=0&frost=1&unholy=0)&runic_power<88
            if (await Spell.CoCast(S.HowlingBlast, onunit,
                (Me.DeathRuneCount == 1 && Me.FrostRuneCount == 0 && Me.UnholyRuneCount == 0
                 || Me.DeathRuneCount == 0 && Me.FrostRuneCount == 1 && Me.UnholyRuneCount == 0) &&
                Me.CurrentRunicPower < 88)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> multi_target_bos(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            // actions.multi_target_bos=howling_blast
            if (await Spell.CoCast(S.HowlingBlast, onunit)) return true;

            // actions.multi_target_bos+=/blood_tap,if=buff.blood_charge.stack>10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount > 10 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.multi_target_bos+=/death_and_decay,if=unholy=1
            if (await Spell.CastOnGround(S.DeathandDecay, Me, Me.UnholyRuneCount == 1)) return true;

            // actions.multi_target_bos+=/plague_strike,if=unholy=2
            if (await Spell.CoCast(S.PlagueStrike, onunit, Me.UnholyRuneCount == 2)) return true;

            // actions.multi_target_bos+=/blood_tap
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.multi_target_bos+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            // actions.multi_target_bos+=/plague_strike,if=unholy=1
            if (await Spell.CoCast(S.PlagueStrike, onunit, Me.UnholyRuneCount == 1)) return true;

            // actions.multi_target_bos+=/empower_rune_weapon
            await Spell.CoCast(S.EmpowerRuneWeapon, Me, Me.CurrentRunicPower < 80 && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion CombatRoutine

        #region RestCoroutine

        private static async Task<bool> RestCoroutine()
        {
            if (!GeneralSettings.Instance.RestingEatFood) return false;

            if (Me.IsDead || SpellManager.GlobalCooldown || !CanBuffEat()) return false;

            if (!(Me.HealthPercent < GeneralSettings.Instance.RestingEatFoodHp) || Me.IsMoving || Me.IsCasting || Me.Combat || Me.HasAura("Food") ||
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
            return auras.Any(a => a.SpellId != S.AuraBloodPlague || a.SpellId != S.AuraFrostFever || a.SpellId != S.AuraNecroticPlague);
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

        public static bool NecroticPlagueSelected()
        {
            return TalentManager.IsSelected(19);
        }

        public static bool DefileSelected()
        {
            return TalentManager.IsSelected(20);
        }

        public static bool BoSSelected()
        {
            return TalentManager.IsSelected(21);
        }

        public static bool UnholyBlightSelected()
        {
            return TalentManager.IsSelected(3);
        }

        public static bool DeathsAdvanceSelected()
        {
            return TalentManager.IsSelected(7);
        }

        public static bool RunicEmpowermentSelected()
        {
            return TalentManager.IsSelected(11);
        }

        public static bool BloodTapSelected()
        {
            return TalentManager.IsSelected(10);
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

        private static async Task<bool> NecroBlightOpener(bool reqs)
        {
            if (!reqs) return false;

            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed)) return true;

            if (await Spell.CoCast(S.DeathsAdvance, TalentManager.IsSelected(7) && Capabilities.IsCooldownUsageAllowed))
                return true;

            await Spell.CoCast(S.PillarofFrost, Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd);

            if (await Spell.CoCast(S.HowlingBlast, !Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.FrostStrike,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 25)) return true;

            if (await Spell.CoCast(S.HowlingBlast, !Me.HasAura(S.AuraKillingMachine))) return true;

            if (await Spell.CoCast(S.PlagueLeech,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                (SpellManager.CanCast(S.Outbreak) || SpellManager.CanCast(S.PlagueStrike)) &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Fever") &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Blood Plague")))
                return true;

            if (await Spell.CoCast(S.Outbreak)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> DefileOpener(bool reqs)
        {
            if (!reqs) return false;

            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed)) return true;

            if (await Spell.CoCast(S.DeathsAdvance, TalentManager.IsSelected(7) && Capabilities.IsCooldownUsageAllowed))
                return true;

            if (await Spell.CoCast(S.Outbreak)) return true;

            if (await Spell.CoCast(S.PillarofFrost, Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd)) return true;

            if (await Spell.CastOnGround(S.Defile, Me, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.FrostStrike,
                Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraKillingMachine) &&
                Me.CurrentRunicPower >= 25)) return true;

            if (await Spell.CoCast(S.Obliterate,
                Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraKillingMachine))) return true;

            if (await Spell.CoCast(S.HowlingBlast, !Me.HasAura(S.AuraKillingMachine))) return true;

            if (await Spell.CoCast(S.PlagueLeech,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                (SpellManager.CanCast(S.Outbreak) || SpellManager.CanCast(S.PlagueStrike)) &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Fever") &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Blood Plague") &&
                (Me.BloodRuneCount < 1 && Me.FrostRuneCount < 1 || Me.BloodRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                 Me.FrostRuneCount < 1 && Me.UnholyRuneCount < 1 || Me.DeathRuneCount < 1 && Me.BloodRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.FrostRuneCount < 1 || Me.DeathRuneCount < 1 && Me.UnholyRuneCount < 1)))
                return true;

            if (await Spell.CoCast(S.Outbreak)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> ThOpener(bool reqs)
        {
            if (!reqs) return false;
            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed)) return true;

            if (await Spell.CoCast(S.DeathsAdvance,
                !Me.CurrentTarget.IsWithinMeleeRange && TalentManager.IsSelected(7) &&
                Capabilities.IsCooldownUsageAllowed)) return true;

            if (await Spell.CoCast(S.PillarofFrost, Capabilities.IsCooldownUsageAllowed && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.PillarofFrostOnCd)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            await Spell.CoCast(S.EmpowerRuneWeapon, Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.Attackable && Me.Combat && Me.GotTarget);

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.FrostStrike,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 25)) return true;

            if (await Spell.CoCast(S.FrostStrike,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 25)) return true;

            if (await Spell.CoCast(S.Obliterate, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Openers
    }
}