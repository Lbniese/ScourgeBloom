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

namespace ScourgeBloom.Class.DeathKnight
{
    [UsedImplicitly]
    public class Unholy : ScourgeBloom
    {
        #region PullRoutine

        private static async Task<bool> PullRoutine()
        {
            if (!Me.Combat || Globals.Mounted || !Me.GotTarget || !Me.CurrentTarget.IsAlive || Me.IsCasting ||
                Me.IsChanneling) return true;

            if (Capabilities.IsMovingAllowed)
                await MovementManager.MoveToTarget();

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (!StyxWoW.Me.GotTarget || !Me.CurrentTarget.Attackable)
                return false;

            // Attack if not attacking
            if (Capabilities.IsPetUsageAllowed && !Me.Pet.IsAutoAttacking)
            {
                await PetAttack();
                return true;
            }

            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
                return true;
            }

            if (!Me.CurrentTarget.IsBoss) return true;

            if (await NecroBlightOpener(NecroticPlagueSelected()))
            {
                return true;
            }

            if (await DefileOpener(DefileSelected()))
            {
                return true;
            }

            if (await BreathofSindragosaOpener(BoSSelected()))
            {
                return true;
            }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion PullRoutine

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

        #region CombatCoroutine

        private static async Task<bool> CombatCoroutine(WoWUnit onunit)
        {
            if (Globals.Mounted || !Me.GotTarget || !Me.CurrentTarget.IsAlive || Me.IsCasting ||
                Me.IsChanneling) return true;

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            if (!Me.Combat) return true;

            // Attack if not attacking
            if (Capabilities.IsPetUsageAllowed && !Me.Pet.IsAutoAttacking && Me.GotAlivePet)
            {
                await PetAttack();
                return true;
            }

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
            if (await BoSActive(onunit, Me.Combat && Me.HasAura("Breath of Sindragosa")))
                return true;

            if (await Spell.CoCast(S.ArmyoftheDead, Me, Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD))
                return true;

            // actions.unholy=plague_leech,if=((cooldown.outbreak.remains<1)|disease.min_remains<1)&((blood<1&frost<1)|(blood<1&unholy<1)|(frost<1&unholy<1))
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && GoodPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                (Spell.GetCooldownLeft(S.Outbreak).TotalSeconds < 1 || DiseaseRemainsLessThanOne())))
                return true;

            // actions.unholy+=/soul_reaper,if=(target.health.pct-3*(target.health.pct%target.time_to_die))<=45
            if (await Spell.CoCast(S.SoulReaperUh, onunit, Me.CurrentTarget.HealthPercent <= 47))
                return true;

            // actions.unholy+=/blood_tap,if=((target.health.pct-3*(target.health.pct%target.time_to_die))<=45)&cooldown.soul_reaper.remains=0
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura("Blood Charge") && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.SoulReaperUh) && Me.CurrentTarget.HealthPercent < 47 &&
                Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.DeathRuneCount == 0);

            // actions.unholy+=/summon_gargoyle
            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Me.GotTarget && Me.CurrentTarget.Attackable && DeathKnightSettings.Instance.SummonGargoyleOnCd &&
                Capabilities.IsCooldownUsageAllowed))
                return true;

            // actions.unholy+=/breath_of_sindragosa,if=runic_power>75
            await Spell.CoCast(S.BreathofSindragosa, onunit,
                Me.CurrentRunicPower > 75 && !Me.HasAura("Breath of Sindragosa") &&
                Capabilities.IsCooldownUsageAllowed && Capabilities.IsAoeAllowed);
            //Eventually add: No runes depleted

            // actions.unholy+=/unholy_blight,if=!disease.min_ticking
            if (await Spell.CoCast(S.UnholyBlight, onunit,
                !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) ||
                Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                Me.CurrentTarget.Auras["Necrotic Plague"].StackCount <= 5))
                return true;

            // actions.unholy+=/outbreak,cycle_targets=1,if=!talent.necrotic_plague.enabled&(!(dot.blood_plague.ticking|dot.frost_fever.ticking))
            if (await Spell.CoCast(S.Outbreak, onunit,
                Units.EnemiesInRange(10) == 1 && !NecroticPlagueSelected() &&
                (!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                 !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))))
                return true;

            // actions.unholy+=/plague_strike,if=(!talent.necrotic_plague.enabled&!(dot.blood_plague.ticking|dot.frost_fever.ticking))|(talent.necrotic_plague.enabled&!dot.necrotic_plague.ticking)
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                (!NecroticPlagueSelected() &&
                 !(Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                   Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))) ||
                NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague)))
                return true;

            // actions.unholy+=/blood_boil,cycle_targets=1,if=(spell_targets.blood_boil>1&!talent.necrotic_plague.enabled)&(!(dot.blood_plague.ticking|dot.frost_fever.ticking))
            var radius = TalentManager.HasGlyph("Blood Boil") ? 15 : 10;
            if (await Spell.CoCast(S.BloodBoil, onunit,
                SpellManager.CanCast(S.BloodBoil) && Units.EnemiesInRange(radius) > 1 && ShouldSpreadDiseases() &&
                Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange &&
                !NecroticPlagueSelected() &&
                (!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) || Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))))
                return true;

            // actions.unholy+=/death_and_decay,if=spell_targets.death_and_decay>1&unholy>1
            if (await Spell.CastOnGround(S.DeathandDecay, Me,
                Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.IsMoving &&
                Capabilities.IsAoeAllowed && Units.EnemiesInRange(10) > 1 && Me.UnholyRuneCount > 1))
                return true;

            // actions.unholy+=/defile,if=unholy=2
            if (await Spell.CastOnGround(S.Defile, Me,
                Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.IsMoving &&
                Me.UnholyRuneCount == 2 && Capabilities.IsAoeAllowed))
                return true;

            // actions.unholy+=/blood_tap,if=talent.defile.enabled&cooldown.defile.remains=0
            await Spell.CoCast(S.BloodTap, onunit,
                DefileSelected() && SpellManager.CanCast(S.BloodTap) && Me.HasAura(S.AuraBloodCharge) &&
                Me.Auras["Blood Charge"].StackCount >= 5 && Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 &&
                Me.FrostRuneCount == 0 && Me.DeathRuneCount == 0 && Spell.GetCooldownLeft(S.Defile).TotalSeconds < 1);

            // actions.unholy+=/scourge_strike,if=unholy=2
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.UnholyRuneCount == 2 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.unholy+=/festering_strike,if=talent.necrotic_plague.enabled&talent.unholy_blight.enabled&dot.necrotic_plague.remains<cooldown.unholy_blight.remains%2
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                NecroticPlagueSelected() && UnholyBlightSelected() &&
                Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                NeedToExtendNecroticPlague()))
                return true;

            // actions.unholy+=/dark_transformation
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Me.GotAlivePet && Me.Pet.ActiveAuras.ContainsKey("Shadow Infusion") &&
                Me.Pet.Auras["Shadow Infusion"].StackCount == 5))
                return true;

            // actions.unholy+=/festering_strike,if=blood=2&frost=2&(((Frost-death)>0)|((Blood-death)>0))
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.BloodRuneCount == 2 && Me.FrostRuneCount == 2 &&
                ((Me.FrostRuneCount - Me.DeathRuneCount > 0) ||
                 (Me.BloodRuneCount - Me.DeathRuneCount > 0))))
                return true;

            // actions.unholy+=/festering_strike,if=(blood=2|frost=2)&(((Frost-death)>0)&((Blood-death)>0))
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.BloodRuneCount == 2 || Me.FrostRuneCount == 2 &&
                (Me.FrostRuneCount - Me.DeathRuneCount > 0) &&
                (Me.BloodRuneCount - Me.DeathRuneCount > 0)))
                return true;

            // actions.unholy+=/blood_boil,cycle_targets=1,if=(talent.necrotic_plague.enabled&!dot.necrotic_plague.ticking)&spell_targets.blood_boil>1
            if (await Spell.CoCast(S.BloodBoil, onunit,
                Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange && NecroticPlagueSelected() &&
                !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) && Units.EnemiesInRange(radius) > 1))
                return true;

            // actions.unholy+=/defile,if=blood=2|frost=2
            if (await Spell.CastOnGround(S.Defile, Me,
                Capabilities.IsAoeAllowed && DefileSelected() && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving && (Me.BloodRuneCount == 2 || Me.FrostRuneCount == 2)))
                return true;

            // actions.unholy+=/death_and_decay,if=spell_targets.death_and_decay>1
            if (await Spell.CastOnGround(S.DeathandDecay, Me,
                Capabilities.IsAoeAllowed && !DefileSelected() && Me.GotTarget && !DefileSelected() &&
                Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.IsMoving &&
                Units.EnemiesInRange(10) > 1)) return true;

            // actions.unholy+=/defile
            if (await Spell.CastOnGround(S.Defile, Me,
                Capabilities.IsAoeAllowed && DefileSelected() && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving))
                return true;

            // actions.unholy+=/blood_boil,if=talent.breath_of_sindragosa.enabled&((spell_targets.blood_boil>=4&(blood=2|(frost=2&death=2)))&(cooldown.breath_of_sindragosa.remains>6|runic_power<75))
            if (await Spell.CoCast(S.BloodBoil, onunit,
                Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange && BoSSelected() && Units.EnemiesInRange(radius) >= 4 &&
                (Me.BloodRuneCount == 2 || (Me.FrostRuneCount == 2 && Me.DeathRuneCount == 2)) &&
                (Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 6 || Me.CurrentRunicPower < 75)))
                return true;

            // actions.unholy+=/blood_boil,if=!talent.breath_of_sindragosa.enabled&(spell_targets.blood_boil>=4&(blood=2|(frost=2&death=2)))
            if (await Spell.CoCast(S.BloodBoil, onunit,
                Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange && !BoSSelected() && Units.EnemiesInRange(radius) >= 4 &&
                (Me.BloodRuneCount == 2 || (Me.FrostRuneCount == 2 && Me.DeathRuneCount == 2))))
                return true;

            // actions.unholy+=/blood_tap,if=buff.blood_charge.stack>10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 10 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.unholy+=/outbreak,if=talent.necrotic_plague.enabled&debuff.necrotic_plague.stack<=14
            if (await Spell.CoCast(S.Outbreak, onunit,
                NecroticPlagueSelected() && Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague) &&
                Me.CurrentTarget.Auras["Necrotic Plague"].StackCount <= 14)) return true;

            // actions.unholy+=/death_coil,if=(buff.sudden_doom.react|runic_power>80)&(buff.blood_charge.stack<=10)
            if (Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount <= 10)
            {
                if (await Spell.CoCast(S.DeathCoil, onunit,
                    Me.HasAura(S.AuraSuddenDoom)))
                    return true;

                //if (Me.HasAura("Breath of Sindragosa")) return true;

                if (await Spell.CoCast(S.DeathCoil, onunit,
                    Me.CurrentRunicPower > 85))
                    return true;
            }

            // actions.unholy+=/blood_boil,if=(spell_targets.blood_boil>=4&(cooldown.breath_of_sindragosa.remains>6|runic_power<75))|(!talent.breath_of_sindragosa.enabled&spell_targets.blood_boil>=4)
            if (await Spell.CoCast(S.BloodBoil, onunit,
                Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange &&
                Units.EnemiesInRange(radius) >= 4 &&
                (BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 6 ||
                 Me.CurrentRunicPower < 75) ||
                !BoSSelected())) return true;

            // actions.unholy+=/scourge_strike,if=(cooldown.breath_of_sindragosa.remains>6|runic_power<75|unholy=2)|!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange &&
                (BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 6 ||
                 Me.CurrentRunicPower < 75 || Me.UnholyRuneCount == 2) ||
                !BoSSelected()))
                return true;

            // actions.unholy+=/festering_strike,if=(cooldown.breath_of_sindragosa.remains>6|runic_power<75)|!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange &&
                (BoSSelected() && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 6 ||
                 Me.CurrentRunicPower < 75) ||
                !BoSSelected())) return true;

            // actions.unholy+=/death_coil,if=(cooldown.breath_of_sindragosa.remains>20)|!talent.breath_of_sindragosa.enabled
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentRunicPower >= 30 && BoSSelected() &&
                Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds > 20 ||
                !BoSSelected())) return true;

            // actions.unholy+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague)))
                return true;

            // actions.unholy+=/empower_rune_weapon,if=!talent.breath_of_sindragosa.enabled (CUSTOM REQUIREMENTS ADDED)
            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                !BoSSelected() && Me.UnholyRuneCount < 1 && Me.FrostRuneCount < 1 && Me.BloodRuneCount < 1 &&
                Me.DeathRuneCount < 1 && Capabilities.IsCooldownUsageAllowed);

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatCoroutine

        #region PreCombatBuffs

        private static async Task<bool> PreCombatBuffs()
        {
            if (!Me.IsAlive)
                return true;

            if (await Spell.CoCast(S.UnholyPresence, Me, !Me.HasAura(S.UnholyPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, Me, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            if (await Spell.CoCast(S.RaiseDead, Me, !Me.GotAlivePet && Capabilities.IsPetSummonAllowed))
                return true;

            if (GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.Attackable &&
                Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight && Me.IsSafelyFacing(Me.CurrentTarget))
            {
                if (Me.CurrentTarget.Distance > 7 && DeathKnightSettings.Instance.DeathGrip)
                    return await Spell.CoCast(S.DeathGrip,
                        SpellManager.CanCast(S.DeathGrip));

                if (Spell.GetCooldownLeft(S.Outbreak).TotalSeconds < 1)
                    return await Spell.CoCast(S.Outbreak,
                        SpellManager.CanCast(S.Outbreak));

                if (Spell.GetCooldownLeft(S.Outbreak).TotalSeconds > 1)
                    return await Spell.CoCast(S.IcyTouch,
                        SpellManager.CanCast(S.IcyTouch));
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

            if (await Spell.CoCast(S.UnholyPresence, Me, !Me.HasAura(S.UnholyPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, Me, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            if (await Spell.CoCast(S.RaiseDead, Me, !Me.GotAlivePet && Capabilities.IsPetSummonAllowed))
                return true;

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Coroutine BoS

        private static async Task<bool> BoSActive(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;
            // Use cooldowns
            // actions.bos=blood_fury,if=dot.breath_of_sindragosa.ticking
            await Spell.CoCast(S.BloodFury, onunit,
                GeneralSettings.BloodFuryUse && DeathKnightSettings.Instance.BosBloodFury && Me.GotTarget &&
                Me.Combat);
            // actions.bos+=/berserking,if=dot.breath_of_sindragosa.ticking
            await Spell.CoCast(S.Berserking, onunit,
                GeneralSettings.BerserkingUse && DeathKnightSettings.Instance.BosBerserking && Me.GotTarget &&
                Me.Combat);

            // CUSTOM
            await  Spell.CoCast(S.EmpowerRuneWeapon, Me, Me.CurrentRunicPower < 60 && Capabilities.IsCooldownUsageAllowed);

            // actions.bos+=/unholy_blight,if=!disease.ticking
            if (await Spell.CoCast(S.UnholyBlight, onunit,
                SpellManager.HasSpell(S.UnholyBlight) &&
                (!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                 !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.CurrentTarget.IsWithinMeleeRange)))
                return true;

            // actions.bos+=/plague_strike,if=!disease.ticking
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                !Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            // actions.bos+=/blood_boil,cycle_targets=1,if=(spell_targets.blood_boil>=2&!(dot.blood_plague.ticking|dot.frost_fever.ticking))|spell_targets.blood_boil>=4&(runic_power<88&runic_power>30)
            var radius = TalentManager.HasGlyph("Blood Boil") ? 15 : 10;
            if (await Spell.CoCast(S.BloodBoil, onunit,
                (Units.EnemiesInRange(radius) >= 2 &&
                 !(!Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                   !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))) ||
                Units.EnemiesInRange(radius) >= 4 && Me.CurrentRunicPower < 88 &&
                Me.CurrentRunicPower > 30 && Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            // actions.bos+=/death_and_decay,if=spell_targets.death_and_decay>=2&(runic_power<88&runic_power>30)
            if (await Spell.CastOnGround(S.DeathandDecay, onunit,
                Units.EnemiesInRange(10) >= 2 && Me.CurrentRunicPower < 88 &&
                Me.CurrentRunicPower > 30 && Capabilities.IsAoeAllowed)) return true;

            // actions.bos+=/festering_strike,if=(blood=2&frost=2&(((Frost-death)>0)|((Blood-death)>0)))&runic_power<80
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.BloodRuneCount == 2 && Me.FrostRuneCount == 2 &&
                ((Me.FrostRuneCount - Me.DeathRuneCount > 0) || (Me.BloodRuneCount - Me.DeathRuneCount > 0)) &&
                Me.CurrentRunicPower < 80 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.bos+=/festering_strike,if=((blood=2|frost=2)&(((Frost-death)>0)&((Blood-death)>0)))&runic_power<80
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                (Me.BloodRuneCount == 2 || Me.FrostRuneCount == 2) &&
                ((Me.FrostRuneCount - Me.DeathRuneCount > 0) || (Me.BloodRuneCount - Me.DeathRuneCount > 0)) &&
                Me.CurrentRunicPower < 80 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.bos+=/arcane_torrent,if=runic_power<70
            await Spell.CoCast(S.ArcaneTorrent, onunit,
                Me.CurrentRunicPower < 70 && Me.Race == WoWRace.BloodElf && Capabilities.IsRacialUsageAllowed &&
                GeneralSettings.ArcaneTorrentUse && DeathKnightSettings.Instance.BosArcaneTorrent &&
                Me.CurrentTarget.IsWithinMeleeRange);

            // CUSTOM
            await Spell.CoCast(S.EmpowerRuneWeapon, Me, Me.CurrentRunicPower < 60 && Capabilities.IsCooldownUsageAllowed);

            // actions.bos+=/scourge_strike,if=spell_targets.blood_boil<=3&(runic_power<88&runic_power>30)
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Units.EnemiesInRange(10) <= 3 && Me.CurrentRunicPower < 88 && Me.CurrentRunicPower > 30 &&
                Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            // actions.bos+=/blood_boil,if=spell_targets.blood_boil>=4&(runic_power<88&runic_power>30)
            if (await Spell.CoCast(S.BloodBoil, onunit,
                SpellManager.CanCast(S.BloodBoil) && Capabilities.IsAoeAllowed &&
                Units.EnemiesInRange(radius) >= 4 &&
                Me.CurrentRunicPower < 88 &&
                Me.CurrentRunicPower > 30 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.bos+=/festering_strike,if=runic_power<77
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentRunicPower < 77 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.bos+=/scourge_strike,if=(spell_targets.blood_boil>=4&(runic_power<88&runic_power>30))|spell_targets.blood_boil<=3
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                (Units.EnemiesInRange(10) >= 4 && Me.CurrentRunicPower < 88 && Me.CurrentRunicPower > 30) ||
                Units.EnemiesInRange(10) <= 3 && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // actions.bos+=/dark_transformation
            if (await Spell.CoCast(S.DarkTransformation,
                Me.GotAlivePet && Me.Pet.ActiveAuras.ContainsKey("Shadow Infusion") &&
                Me.Pet.Auras["Shadow Infusion"].StackCount == 5)) return true;

            // actions.bos+=/blood_tap,if=buff.blood_charge.stack>=5
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // actions.bos+=/plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))) return true;

            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                Me.CurrentRunicPower < 60 && Capabilities.IsCooldownUsageAllowed && BoSSelected());

            if (await Spell.CoCast(S.DeathCoil, onunit, Me.HasAura(S.AuraSuddenDoom)))
                return true;

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Coroutine BoS

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

        #region PetAttack

        private static async Task<bool> PetAttack()
        {
            if (Me.GotAlivePet && Me.GotTarget && Me.CurrentTarget.Attackable)
            {
                Lua.DoString("PetAttack();");
            }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion PetAttack

#endregion Logics

        #region Overrides

        public override WoWClass Class
            => Me.Specialization == WoWSpec.DeathKnightUnholy ? WoWClass.DeathKnight : WoWClass.None;

        protected override Composite CreateCombat()
        {
            return new ActionRunCoroutine(ret => CombatCoroutine(Me.CurrentTarget));
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
            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD)) return true;
            if (await Spell.CoCast(S.DeathsAdvance, DeathsAdvanceSelected() && Capabilities.IsCooldownUsageAllowed))
                return true;
            if (await Spell.CoCast(S.UnholyBlight, Me.CurrentTarget.IsWithinMeleeRange)) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.CurrentTarget.IsWithinMeleeRange)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.CurrentTarget.IsWithinMeleeRange)) return true;
            if (await Spell.CoCast(S.Outbreak, Me.GotTarget)) return true;
            if (
                await
                    Spell.CoCast(S.SummonGargoyle,
                        Me.GotTarget && DeathKnightSettings.Instance.SummonGargoyleOnCd &&
                        Capabilities.IsCooldownUsageAllowed)) return true;

            return true;
        }

        private static async Task<bool> DefileOpener(bool reqs)
        {
            if (!reqs) return false;
            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD)) return true;
            if (await Spell.CoCast(S.DeathsAdvance, DeathsAdvanceSelected() && Capabilities.IsCooldownUsageAllowed))
                return true;
            if (
                await
                    Spell.CoCast(S.SummonGargoyle,
                        Me.GotTarget && DeathKnightSettings.Instance.SummonGargoyleOnCd &&
                        Capabilities.IsCooldownUsageAllowed)) return true;
            if (await Spell.CoCast(S.Outbreak, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.CurrentTarget.IsWithinMeleeRange)) return true;
            if (
                await
                    Spell.CastOnGround(S.Defile, Me,
                        Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && Capabilities.IsAoeAllowed)) return true;
            if (await Spell.CoCast(S.DeathCoil, Me.HasAura(S.AuraSuddenDoom))) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.CurrentTarget.IsWithinMeleeRange)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.CurrentTarget.IsWithinMeleeRange)) return true;

            return true;
        }

        private static async Task<bool> BreathofSindragosaOpener(bool reqs)
        {
            if (!reqs) return false;
            if (await Spell.CoCast(S.ArmyoftheDead, Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD)) return true;
            if (await Spell.CoCast(S.DeathsAdvance, DeathsAdvanceSelected() && Capabilities.IsCooldownUsageAllowed))
                return true;
            if (await Spell.CoCast(S.SummonGargoyle,
                DeathKnightSettings.Instance.SummonGargoyleOnCd && Capabilities.IsCooldownUsageAllowed))
                return true;
            if (await Spell.CoCast(S.Outbreak, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.PlagueLeech,
                CanPlagueLeech() && SpellManager.CanCast(S.PlagueLeech) &&
                (SpellManager.CanCast(S.Outbreak) || SpellManager.CanCast(S.PlagueStrike)) &&
                Me.CurrentTarget.HasMyAura(S.AuraFrostFever) &&
                Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) &&
                (Me.BloodRuneCount < 1 && Me.FrostRuneCount < 1 ||
                 Me.BloodRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                 Me.FrostRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.BloodRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.FrostRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.UnholyRuneCount < 1)))
                return true;
            if (await Spell.CoCast(S.PlagueStrike, Me.GotTarget)) return true;
            await Spell.CoCast(S.BreathofSindragosa, Me.CurrentTarget.IsWithinMeleeRange);
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            await Spell.CoCast(S.EmpowerRuneWeapon, Me, Capabilities.IsCooldownUsageAllowed);
            if (await Spell.CoCast(S.FesteringStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.DarkTransformation,
                Me.GotAlivePet && Me.Pet.ActiveAuras.ContainsKey("Shadow Infusion") &&
                Me.Pet.Auras["Shadow Infusion"].StackCount == 5)) return true;
            if (await Spell.CoCast(S.FesteringStrike, Me.GotTarget)) return true;
            if (await Spell.CoCast(S.ScourgeStrike, Me.GotTarget)) return true;

            return true;
        }

        #endregion Openers
    }
}