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
    public class Blood : ScourgeBloom
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

        #region CombatRoutine

        private static async Task<bool> CombatRoutine(WoWUnit onunit)
        {
            if (Paused) return false;

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
            if (
                await
                    Spell.CoCast(S.ArmyoftheDead, Me,
                        Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed &&
                        DeathKnightSettings.Instance.UseAotD))
                return true;

            //9	5.52	dancing_rune_weapon,if=target.time_to_die>90|buff.draenic_armor_potion.remains<=buff.dancing_rune_weapon.duration

            //0.00	lichborne,if=health.pct<30

            //E	0.01	vampiric_blood,if=health.pct<40

            //0.00	icebound_fortitude,if=health.pct<30&buff.army_of_the_dead.down&buff.dancing_rune_weapon.down&buff.bone_shield.down&buff.rune_tap.down

            //0.00	death_pact,if=health.pct<30
            await Spell.CoCast(S.DeathPact, Me, DeathPactSelected() && Me.HealthPercent < 30);

            //run_action_list,name=last,if=target.time_to_die<8|target.time_to_die<13&cooldown.empower_rune_weapon.remains<4
            await Last(onunit, TTD.TimeToDeath(onunit) < 8 ||
                               TTD.TimeToDeath(onunit) < 13 &&
                               Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 4);

            //run_action_list,name=bos,if=dot.breath_of_sindragosa.ticking
            await BoS(onunit, Me.HasAura(S.BreathofSindragosa));

            //run_action_list,name=nbos,if=!dot.breath_of_sindragosa.ticking&cooldown.breath_of_sindragosa.remains<4
            await
                NboS(onunit,
                    !Me.HasAura(S.BreathofSindragosa) && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds < 4);

            //run_action_list,name=cdbos,if=!dot.breath_of_sindragosa.ticking&cooldown.breath_of_sindragosa.remains>=4
            await CdBoS(onunit,
                !Me.HasAura(S.BreathofSindragosa) && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds >= 4);

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatRoutine

        #region Coroutine Last

        private static async Task<bool> Last(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            // antimagic_shell,if=runic_power<90
            //await Spell.Cast(S.AntiMagicShell, () => Me.CurrentRunicPower < 90);

            // blood_tap
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // soul_reaper,if=target.time_to_die>7
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, TTD.TimeToDeath(onunit) > 7))
                return true;

            // death_coil,if=runic_power>80
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 80)) return true;

            // death_strike
            if (await Spell.CoCast(S.DeathStrike, onunit)) return true;

            // blood_boil,if=blood=2|target.time_to_die<=7
            if (await Spell.CoCast(S.BloodBoil, onunit, Me.BloodRuneCount == 2 || TTD.TimeToDeath(onunit) <= 8) &&
                Me.CurrentTarget.IsWithinMeleeRange)
                return true;

            // death_coil,if=runic_power>75|target.time_to_die<4|!dot.breath_of_sindragosa.ticking
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentRunicPower > 75 || TTD.TimeToDeath(onunit) < 4 ||
                !(Me.HasAura("Breath of Sindragosa") || Me.Auras["Breath of Sindragosa"].IsActive)))
                return true;

            // plague_strike,if=target.time_to_die<2|cooldown.empower_rune_weapon.remains<2
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                TTD.TimeToDeath(onunit) < 2 || Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 2))
                return true;

            // icy_touch,if=target.time_to_die<2|cooldown.empower_rune_weapon.remains<2
            if (await Spell.CoCast(S.IcyTouch, onunit,
                TTD.TimeToDeath(onunit) < 2 || Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 2))
                return true;

            // empower_rune_weapon,if=!blood&!unholy&!frost&runic_power<76|target.time_to_die<5
            await Spell.CoCast(S.EmpowerRuneWeapon,
                Me.BloodRuneCount == 0 && Me.UnholyRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.CurrentRunicPower < 76 || TTD.TimeToDeath(onunit) < 5);

            // plague_leech
            if (await Spell.CoCast(S.PlagueLeech, onunit,
                SpellManager.CanCast(S.PlagueLeech) &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Fever") &&
                Me.CurrentTarget.ActiveAuras.ContainsKey("Blood Plague") &&
                (Me.BloodRuneCount < 1 && Me.FrostRuneCount < 1 ||
                 Me.BloodRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                 Me.FrostRuneCount < 1 && Me.UnholyRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.BloodRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.FrostRuneCount < 1 ||
                 Me.DeathRuneCount < 1 && Me.UnholyRuneCount < 1)))
                return true;

            return true;
        }

        #endregion Coroutine Last

        #region Coroutine BoS

        private static async Task<bool> BoS(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            // blood_tap,if=buff.blood_charge.stack>=11
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 11 &&
                SpellManager.CanCast(S.BloodTap));
            // soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<35&runic_power>5

            // blood_tap,if=buff.blood_charge.stack>=9&runic_power>80&(blood.frac>1.8|frost.frac>1.8|unholy.frac>1.8)

            // death_coil,if=runic_power>80&(blood.frac>1.8|frost.frac>1.8|unholy.frac>1.8)

            // outbreak,if=(!dot.blood_plague.ticking|!dot.frost_fever.ticking)&runic_power>21
            if (await Spell.CoCast(S.Outbreak, onunit, (!Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) || !Me.CurrentTarget.HasMyAura(S.AuraFrostFever)) && Me.CurrentRunicPower > 21)) return true;

            // chains_of_ice,if=!dot.frost_fever.ticking&runic_power<90
            if (await Spell.CoCast(S.ChainsOfIce, onunit, !Me.CurrentTarget.HasMyAura(S.AuraFrostFever) && Me.CurrentRunicPower < 90)) return true;

            // plague_strike,if=!dot.blood_plague.ticking&runic_power>5
            if (await Spell.CoCast(S.PlagueStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.CurrentRunicPower > 5)) return true;

            // icy_touch,if=!dot.frost_fever.ticking&runic_power>5
            if (await Spell.CoCast(S.IcyTouch, onunit, Me.CurrentTarget.HasMyAura(S.AuraFrostFever) && Me.CurrentRunicPower > 5)) return true;

            // death_strike,if=runic_power<16
            if (await Spell.CoCast(S.DeathStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower < 16)) return true;

            // blood_tap,if=runic_power<16
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap) && Me.CurrentRunicPower < 16);

            //blood_boil,if=runic_power<16&runic_power>5&buff.crimson_scourge.down&(blood>=1&blood.death=0|blood=2&blood.death<2)

            // arcane_torrent,if=runic_power<16
            await Spell.CoCast(S.ArcaneTorrent, onunit,
                Me.CurrentRunicPower < 16 && Me.Race == WoWRace.BloodElf && Capabilities.IsRacialUsageAllowed &&
                GeneralSettings.ArcaneTorrentUse && DeathKnightSettings.Instance.BosArcaneTorrent &&
                Me.CurrentTarget.IsWithinMeleeRange);

            // chains_of_ice,if=runic_power<16
            if (await Spell.CoCast(S.ChainsOfIce, onunit, Me.CurrentRunicPower < 16)) return true;

            // blood_boil,if=runic_power<16&buff.crimson_scourge.down&(blood>=1&blood.death=0|blood=2&blood.death<2)

            // icy_touch,if=runic_power<16
            if (await Spell.CoCast(S.IcyTouch, onunit, Me.CurrentRunicPower < 16)) return true;

            // plague_strike,if=runic_power<16
            if (await Spell.CoCast(S.PlagueStrike, onunit, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower < 16)) return true;

            // rune_tap,if=runic_power<16&blood>=1&blood.death=0&frost=0&unholy=0&buff.crimson_scourge.up

            // empower_rune_weapon,if=runic_power<16&blood=0&frost=0&unholy=0

            // death_strike,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8|buff.blood_charge.stack>=11)

            // blood_tap,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8)

            // blood_boil,if=(blood>=1&blood.death=0&target.health.pct-3*(target.health.pct%target.time_to_die)>35|blood=2&blood.death<2)&buff.crimson_scourge.down

            // antimagic_shell,if=runic_power<65

            // plague_leech,if=runic_power<65

            // outbreak,if=!dot.blood_plague.ticking

            // outbreak,if=pet.dancing_rune_weapon.active&!pet.dancing_rune_weapon.dot.blood_plague.ticking

            // death_and_decay,if=buff.crimson_scourge.up

            // blood_boil,if=buff.crimson_scourge.up

            return true;
        }

        #endregion Coroutine BoS

        #region Coroutine nBoS

        private static async Task<bool> NboS(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            // breath_of_sindragosa,if=runic_power>=80
            await Spell.CoCast(S.BreathofSindragosa, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 80);

            // soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<=35
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, Me.CurrentTarget.HealthPercent <= 37))
                return true;

            // chains_of_ice,if=!dot.frost_fever.ticking
            if (
                await
                    Spell.CoCast(S.ChainsOfIce, onunit,
                        !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever)))
                return true;

            // icy_touch,if=!dot.frost_fever.ticking
            if (
                await
                    Spell.CoCast(S.IcyTouch, onunit,
                        !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever)))
                return true;

            // plague_strike,if=!dot.blood_plague.ticking
            if (
                await
                    Spell.CoCast(S.PlagueStrike, onunit,
                        !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) &&
                        Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            // death_strike,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8)&runic_power<80
            if (await Spell.CoCast(S.DeathStrike, onunit,
                (Me.BloodRuneCount > 2 || Me.FrostRuneCount > 2 || Me.UnholyRuneCount > 2) &&
                Me.CurrentRunicPower < 80)) return true;

            // death_and_decay,if=buff.crimson_scourge.up
            if (await Spell.CastOnGround(S.DeathandDecay, Me, Me.HasAura("Crimson Scourge"))) return true;

            // blood_boil,if=buff.crimson_scourge.up|(blood=2&runic_power<80&blood.death<2)
            if (
                await
                    Spell.CastOnGround(S.BloodBoil, Me,
                        Me.HasAura("Crimson Scourge") || Me.BloodRuneCount == 2 && Me.CurrentRunicPower < 80))
                return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Coroutine nBoS

        #region Coroutine CdBoS

        private static async Task<bool> CdBoS(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;
            // soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<=35
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, Me.CurrentTarget.HealthPercent <= 37))
                return true;

            // blood_tap,if=buff.blood_charge.stack>=10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 10 &&
                SpellManager.CanCast(S.BloodTap));

            // death_coil,if=runic_power>65
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 65)) return true;

            // plague_strike,if=!dot.blood_plague.ticking&unholy=2
            if (
                await
                    Spell.CoCast(S.PlagueStrike, onunit,
                        !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.UnholyRuneCount == 2 &&
                        Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // icy_touch,if=!dot.frost_fever.ticking&frost=2
            if (
                await
                    Spell.CoCast(S.IcyTouch, onunit,
                        !Me.CurrentTarget.HasMyAura(S.AuraFrostFever) && Me.FrostRuneCount == 2)) return true;

            // death_strike,if=unholy=2|frost=2|blood=2&blood.death>=1
            if (
                await
                    Spell.CoCast(S.DeathStrike, onunit,
                        Me.CurrentTarget.IsWithinMeleeRange &&
                        (Me.UnholyRuneCount == 2 || Me.FrostRuneCount == 2 || Me.BloodRuneCount == 2 ||
                         Me.DeathRuneCount >= 1))) return true; //Skal måske tweakes lidt?

            // blood_boil,if=blood=2&blood.death<2
            if (await Spell.CastOnGround(S.BloodBoil, Me, Me.BloodRuneCount == 2 && Me.DeathRuneCount < 2))
                return true; //Recheck

            // outbreak,if=!dot.blood_plague.ticking
            if (await Spell.CoCast(S.Outbreak, onunit, !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague))) return true;

            // plague_strike,if=!dot.blood_plague.ticking
            if (
                await
                    Spell.CoCast(S.PlagueStrike, onunit,
                        !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            // icy_touch,if=!dot.frost_fever.ticking
            if (await Spell.CoCast(S.IcyTouch, onunit, !Me.CurrentTarget.HasMyAura(S.AuraFrostFever))) return true;

            // outbreak,if=pet.dancing_rune_weapon.active&!pet.dancing_rune_weapon.dot.blood_plague.ticking
            // ØHHH??

            // blood_boil,if=((dot.frost_fever.remains<4&dot.frost_fever.ticking)|(dot.blood_plague.remains<4&dot.blood_plague.ticking))
            if (await Spell.CoCast(S.BloodBoil, Me, DiseaseRemainsLessThanFour())) return true;

            // death_and_decay,if=buff.crimson_scourge.up
            if (await Spell.CastOnGround(S.DeathandDecay, onunit,
                Units.EnemiesInRange(10) >= 1 && Me.HasAura(S.AuraCrimsonScourge) && Capabilities.IsAoeAllowed))
                return true;

            if (DefileSelected())
            {
                if (await Spell.CastOnGround(S.Defile, onunit,
                    Units.EnemiesInRange(10) >= 1 && Me.HasAura(S.AuraCrimsonScourge) && Capabilities.IsAoeAllowed))
                    return true;
            }

            // blood_boil,if=buff.crimson_scourge.up
            var radius = TalentManager.HasGlyph("Blood Boil") ? 15 : 10;
            if (
                await
                    Spell.CoCast(S.BloodBoil, onunit,
                        Units.EnemiesInRange(radius) >= 1 && Capabilities.IsAoeAllowed &&
                        Me.CurrentTarget.IsWithinMeleeRange && Me.HasAura(S.AuraCrimsonScourge)))
                return true;

            // death_coil,if=runic_power>45
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 45))
                return true;

            // blood_tap
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.BloodTap));

            // death_strike
            if (await Spell.CoCast(S.DeathStrike, onunit))
                return true;

            // blood_boil,if=blood>=1&blood.death=0
            if (
                await
                    Spell.CoCast(S.BloodBoil, onunit,
                        Units.EnemiesInRange(radius) >= 1 && Capabilities.IsAoeAllowed &&
                        Me.CurrentTarget.IsWithinMeleeRange && Me.BloodRuneCount >= 1 && Me.DeathRuneCount == 0))
                return true;

            // death_coil
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower >= 30))
                return true;

            // CUSTOM death_coil
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.HasAura(S.AuraSuddenDoom)))
                return true;


            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Coroutine CdBoS

        #region RestCoroutine

        private static async Task<bool> RestCoroutine()
        {
            if (Paused) return false;

            if (Me.IsDead || SpellManager.GlobalCooldown)
                return false;

            if (!(Me.HealthPercent < 60) || Me.IsMoving || Me.IsCasting || Me.Combat || Me.HasAura("Food") ||
                Consumable.GetBestFood(false) == null)
                return false;

            Styx.CommonBot.Rest.FeedImmediate();
            return await Coroutine.Wait(1000, () => Me.HasAura("Food"));
        }

        #endregion RestCoroutine

        #region PreCombatBuffs

        private static async Task<bool> PreCombatBuffs()
        {
            if (Paused) return false;

            if (!Me.IsAlive)
                return false;

            if (await Spell.CoCast(S.BloodPresence, !Me.HasAura(S.BloodPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            await Spell.CoCast(S.BoneShield, SpellManager.CanCast(S.BoneShield) && !Me.HasAura(S.BoneShield));

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

            if (await Spell.CoCast(S.BloodPresence, !Me.HasAura(S.BloodPresence)))
                return true;

            if (await Spell.CoCast(S.HornofWinter, !Me.HasPartyBuff(Units.Stat.AttackPower)))
                return true;

            await Spell.CoCast(S.BoneShield, SpellManager.CanCast(S.BoneShield) && !Me.HasAura(S.BoneShield));

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Pull

        public static async Task<bool> PullRoutine()
        {
            if (Paused) return false;

            if (!Me.Combat || Globals.Mounted || !Me.GotTarget || !Me.CurrentTarget.IsAlive || Me.IsCasting ||
                Me.IsChanneling) return true;

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
                return true;
            }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Pull

        #region Overrides

        public override WoWClass Class
            => Me.Specialization == WoWSpec.DeathKnightBlood ? WoWClass.DeathKnight : WoWClass.None;

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

        #region Logics

        public static bool NeedToSpread()
        {
            if ((!StyxWoW.Me.CurrentTarget.HasAura(S.AuraBloodPlague) ||
                 !StyxWoW.Me.CurrentTarget.HasAura(S.AuraFrostFever)) &&
                (!StyxWoW.Me.CurrentTarget.HasAura(S.AuraNecroticPlague) || !TalentManager.IsSelected(19)))
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
            return auras.Any(a => a.SpellId != 59879 || a.SpellId != 55095 || a.SpellId != 155159);
        }

        public static bool PlagueLeech()
        {
            if (StyxWoW.Me.CurrentTarget.GetAuraById(59879) == null ||
                StyxWoW.Me.CurrentTarget.GetAuraById(55095) == null) return false;
            var frTime = StyxWoW.Me.CurrentTarget.GetAuraById(59879).TimeLeft;
            var blTime = StyxWoW.Me.CurrentTarget.GetAuraById(55095).TimeLeft;

            return frTime <= TimeSpan.FromSeconds(3) || blTime <= TimeSpan.FromSeconds(3);
        }

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

        public static bool DiseaseRemainsLessThanFour()
        {
            if (!StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraFrostFever) ||
                !StyxWoW.Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) ||
                (NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraNecroticPlague))) return false;

            var ffTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraFrostFever).TimeLeft;
            var bpTime = StyxWoW.Me.CurrentTarget.GetAuraById(S.AuraBloodPlague).TimeLeft;

            return ffTime < TimeSpan.FromSeconds(4) || bpTime < TimeSpan.FromSeconds(4);
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

        public static bool DeathPactSelected()
        {
          return TalentManager.IsSelected(13);
        }

        #endregion Logics
    }
}
