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
            if (await Spell.CoCast(S.ArmyoftheDead, Me, Me.CurrentTarget.IsBoss && Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD))
                return true;

            //0.00	blood_fury,if=target.time_to_die>120|buff.draenic_armor_potion.remains<=buff.blood_fury.duration

            //0.00	berserking,if=buff.dancing_rune_weapon.up

            //9	5.52	dancing_rune_weapon,if=target.time_to_die>90|buff.draenic_armor_potion.remains<=buff.dancing_rune_weapon.duration

            //A	1.00	potion,name=draenic_armor,if=target.time_to_die<(buff.draenic_armor_potion.duration+13)

            //D	7.17	bone_shield,if=buff.army_of_the_dead.down&buff.bone_shield.down&buff.dancing_rune_weapon.down&buff.icebound_fortitude.down&buff.rune_tap.down

            //0.00	lichborne,if=health.pct<30

            //E	0.01	vampiric_blood,if=health.pct<40

            //0.00	icebound_fortitude,if=health.pct<30&buff.army_of_the_dead.down&buff.dancing_rune_weapon.down&buff.bone_shield.down&buff.rune_tap.down

            //0.00	death_pact,if=health.pct<30

            //run_action_list,name=last,if=target.time_to_die<8|target.time_to_die<13&cooldown.empower_rune_weapon.remains<4
            await Last(onunit, TTD.TimeToDeath(onunit) < 8 ||
                               TTD.TimeToDeath(onunit) < 13 &&
                               Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 4);

            //run_action_list,name=bos,if=dot.breath_of_sindragosa.ticking
            await BoS(Me.HasAura(S.BreathofSindragosa));

            //run_action_list,name=nbos,if=!dot.breath_of_sindragosa.ticking&cooldown.breath_of_sindragosa.remains<4
            await NboS(onunit, !Me.HasAura(S.BreathofSindragosa) && Spell.GetCooldownLeft(S.BreathofSindragosa).TotalSeconds < 4);

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

            //v	0.76	antimagic_shell,if=runic_power<90
            //await Spell.Cast(S.AntiMagicShell, () => Me.CurrentRunicPower < 90);
            //  0.00	blood_tap
            await Spell.CoCast(S.BloodTap,
                DefileSelected() && SpellManager.CanCast(S.BloodTap) && Me.HasAura("Blood Charge") &&
                Me.Auras["Blood Charge"].StackCount >= 5 && Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 &&
                Me.FrostRuneCount == 0 && Me.DeathRuneCount == 0);
            //w	0.39	soul_reaper,if=target.time_to_die>7
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, TTD.TimeToDeath(onunit) > 7))
                return true;
            //x	1.22	death_coil,if=runic_power>80
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 80)) return true;
            //y	1.86	death_strike
            if (await Spell.CoCast(S.DeathStrike, onunit)) return true;
            //z	2.39	blood_boil,if=blood=2|target.time_to_die<=7
            if (await Spell.CoCast(S.BloodBoil, onunit, (Me.BloodRuneCount == 2 || TTD.TimeToDeath(onunit) <= 8)) && Me.CurrentTarget.IsWithinMeleeRange)
                return true;
            //{	1.85	death_coil,if=runic_power>75|target.time_to_die<4|!dot.breath_of_sindragosa.ticking
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentRunicPower > 75 || TTD.TimeToDeath(onunit) < 4 ||
                !(Me.HasAura("Breath of Sindragosa") || Me.Auras["Breath of Sindragosa"].IsActive)))
                return true;
            //|	0.27	plague_strike,if=target.time_to_die<2|cooldown.empower_rune_weapon.remains<2
            if (await Spell.CoCast(S.PlagueStrike, onunit,
                TTD.TimeToDeath(onunit) < 2 || Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 2))
                return true;
            //}	0.03	icy_touch,if=target.time_to_die<2|cooldown.empower_rune_weapon.remains<2
            if (await Spell.CoCast(S.IcyTouch, onunit,
                TTD.TimeToDeath(onunit) < 2 || Spell.GetCooldownLeft(S.EmpowerRuneWeapon).TotalSeconds < 2))
                return true;
            //~	0.20	empower_rune_weapon,if=!blood&!unholy&!frost&runic_power<76|target.time_to_die<5
            await Spell.CoCast(S.EmpowerRuneWeapon, Me,
                Me.BloodRuneCount == 0 && Me.UnholyRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.CurrentRunicPower < 76 || TTD.TimeToDeath(onunit) < 5);
            //!	0.21	plague_leech
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

        private static async Task<bool> BoS(bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            //    0.00	blood_tap,if=buff.blood_charge.stack>=11
            await Spell.CoCast(S.BloodTap, Me,
                Me.HasAura(S.AuraBloodCharge) && Me.Auras["Blood Charge"].StackCount >= 11 &&
                Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.DeathRuneCount == 0);
            //J	3.10	soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<35&runic_power>5

            //    0.00	blood_tap,if=buff.blood_charge.stack>=9&runic_power>80&(blood.frac>1.8|frost.frac>1.8|unholy.frac>1.8)

            //K	1.35	death_coil,if=runic_power>80&(blood.frac>1.8|frost.frac>1.8|unholy.frac>1.8)

            //L	0.41	outbreak,if=(!dot.blood_plague.ticking|!dot.frost_fever.ticking)&runic_power>21

            //M	1.04	chains_of_ice,if=!dot.frost_fever.ticking&runic_power<90

            //N	0.91	plague_strike,if=!dot.blood_plague.ticking&runic_power>5

            //    0.00	icy_touch,if=!dot.frost_fever.ticking&runic_power>5

            //O	7.14	death_strike,if=runic_power<16

            //    0.00	blood_tap,if=runic_power<16

            //P	1.29	blood_boil,if=runic_power<16&runic_power>5&buff.crimson_scourge.down&(blood>=1&blood.death=0|blood=2&blood.death<2)

            //Q	3.01	arcane_torrent,if=runic_power<16

            //R	2.62	chains_of_ice,if=runic_power<16

            //S	0.41	blood_boil,if=runic_power<16&buff.crimson_scourge.down&(blood>=1&blood.death=0|blood=2&blood.death<2)

            //    0.00	icy_touch,if=runic_power<16

            //T	0.08	plague_strike,if=runic_power<16

            //U	0.28	rune_tap,if=runic_power<16&blood>=1&blood.death=0&frost=0&unholy=0&buff.crimson_scourge.up

            //V	0.92	empower_rune_weapon,if=runic_power<16&blood=0&frost=0&unholy=0

            //W	14.23	death_strike,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8|buff.blood_charge.stack>=11)

            //    0.00	blood_tap,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8)

            //X	13.39	blood_boil,if=(blood>=1&blood.death=0&target.health.pct-3*(target.health.pct%target.time_to_die)>35|blood=2&blood.death<2)&buff.crimson_scourge.down

            //Y	4.49	antimagic_shell,if=runic_power<65

            //Z	4.68	plague_leech,if=runic_power<65

            //a	0.00	outbreak,if=!dot.blood_plague.ticking

            //b	1.18	outbreak,if=pet.dancing_rune_weapon.active&!pet.dancing_rune_weapon.dot.blood_plague.ticking

            //c	2.23	death_and_decay,if=buff.crimson_scourge.up

            //d	3.66	blood_boil,if=buff.crimson_scourge.up

            return true;
        }

        #endregion Coroutine BoS

        #region Coroutine nBoS

        private static async Task<bool> NboS(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            //breath_of_sindragosa,if=runic_power>=80
            await Spell.CoCast(S.BreathofSindragosa, Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 80);

            //soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<=35
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, Me.CurrentTarget.HealthPercent <= 37))
                return true;

            //chains_of_ice,if=!dot.frost_fever.ticking
            if (await Spell.CoCast(S.ChainsOfIce, onunit, !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever)))
                return true;

            //icy_touch,if=!dot.frost_fever.ticking
            if (await Spell.CoCast(S.IcyTouch, onunit, !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraFrostFever)))
                return true;

            //plague_strike,if=!dot.blood_plague.ticking
            if (await Spell.CoCast(S.PlagueStrike, onunit, !NecroticPlagueSelected() && !Me.CurrentTarget.HasMyAura(S.AuraBloodPlague) && Me.CurrentTarget.IsWithinMeleeRange))
                return true;

            //death_strike,if=(blood.frac>1.8&blood.death>=1|frost.frac>1.8|unholy.frac>1.8)&runic_power<80
            if (await Spell.CoCast(S.DeathStrike, onunit,
                (Me.BloodRuneCount > 2 || Me.FrostRuneCount > 2 || Me.UnholyRuneCount > 2) &&
                Me.CurrentRunicPower < 80)) return true;

            //death_and_decay,if=buff.crimson_scourge.up
            if (await Spell.CastOnGround(S.DeathandDecay, Me, Me.HasAura("Crimson Scourge"))) return true;

            //blood_boil,if=buff.crimson_scourge.up|(blood=2&runic_power<80&blood.death<2)
            if (await Spell.CastOnGround(S.BloodBoil, Me, Me.HasAura("Crimson Scourge") || Me.BloodRuneCount == 2 && Me.CurrentRunicPower < 80)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Coroutine nBoS

        #region Coroutine CdBoS

        private static async Task<bool> CdBoS(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;
            //e	16.50	soul_reaper,if=target.health.pct-3*(target.health.pct%target.time_to_die)<=35
            if (await Spell.CoCast(S.SoulReaperBlood, onunit, Me.CurrentTarget.HealthPercent <= 37))
                return true;

            //   0.00	blood_tap,if=buff.blood_charge.stack>=10
            await Spell.CoCast(S.BloodTap, onunit,
                Me.HasAura("Blood Charge") && Me.Auras["Blood Charge"].StackCount >= 5 &&
                SpellManager.CanCast(S.SoulReaperUh) && Me.CurrentTarget.HealthPercent < 47 &&
                Me.UnholyRuneCount == 0 && Me.BloodRuneCount == 0 && Me.FrostRuneCount == 0 &&
                Me.DeathRuneCount == 0);

            //f	23.04	death_coil,if=runic_power>65
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 65)) return true;

            //g	0.10	plague_strike,if=!dot.blood_plague.ticking&unholy=2
            //h	0.06	icy_touch,if=!dot.frost_fever.ticking&frost=2
            //i	7.28	death_strike,if=unholy=2|frost=2|blood=2&blood.death>=1
            //j	15.01	blood_boil,if=blood=2&blood.death<2
            //k	0.04	outbreak,if=!dot.blood_plague.ticking
            //l	0.10	plague_strike,if=!dot.blood_plague.ticking
            //m	0.07	icy_touch,if=!dot.frost_fever.ticking
            //n	3.57	outbreak,if=pet.dancing_rune_weapon.active&!pet.dancing_rune_weapon.dot.blood_plague.ticking
            //o	0.08	blood_boil,if=((dot.frost_fever.remains<4&dot.frost_fever.ticking)|(dot.blood_plague.remains<4&dot.blood_plague.ticking))
            //p	7.55	death_and_decay,if=buff.crimson_scourge.up
            //q	13.37	blood_boil,if=buff.crimson_scourge.up
            //r	62.96	death_coil,if=runic_power>45
            //   0.00	blood_tap
            //s	56.12	death_strike
            //t	33.07	blood_boil,if=blood>=1&blood.death=0
            //u	49.29	death_coil

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

        #endregion Logics
    }
}