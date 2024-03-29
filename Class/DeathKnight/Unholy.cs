/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

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
        public static long RunicPowerDeficit => Me.MaxRunicPower - Me.CurrentRunicPower;

        #region PullRoutine

        private static async Task<bool> PullRoutine()
        {
            if (Paused || !Me.IsAlive || Me.IsOnTransport || Me.OnTaxi || Me.InVehicle)
                return true;

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (!Me.GotTarget || !Me.CurrentTarget.IsAlive)
                return true;

            if (Me.CurrentTarget.IsValidCombatUnit())
            {
                if (!Me.CurrentTarget.IsWithinMeleeRangeOf(Me) && Capabilities.IsMovingAllowed)
                {
                    //L.infoLog("Tried to pull");
                    await MovementManager.EnsureMeleeRange(Me.CurrentTarget);
                }

                if (Capabilities.IsFacingAllowed)
                {
                    // check to see if we need to face target
                    await MovementManager.FaceTarget(Me.CurrentTarget);
                }

                if (!Me.IsAutoAttacking)
                {
                    Lua.DoString("StartAttack()");
                    return true;
                }

                if (await Spell.CoCast(S.Outbreak,
                    GeneralSettings.Instance.AutoAttack && Me.GotTarget &&
                    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight &&
                    Me.IsSafelyFacing(Me.CurrentTarget)))
                    return true;
            }

            if (await Spell.CoCast(S.Outbreak,
                GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.IsAboveTheGround() &&
                Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight &&
                Me.IsSafelyFacing(Me.CurrentTarget)))
                return true;

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

            //if (await ExampleOpener(Me.Level >= 100 && Me.CurrentTarget.IsBoss))
            //{
            //    return true;
            //}

            //await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion PullRoutine

        #region Heals

        private static async Task<bool> HealRoutine()
        {
            if (Paused || Globals.Mounted) return false;

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

            //await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Heals

        #region CombatCoroutine

        private static async Task<bool> CombatCoroutine(WoWUnit onunit)
        {
            if (Paused || !Me.IsAlive || Globals.Mounted)
                return true;

            if (Capabilities.IsTargetingAllowed)
                TargetManager.EnsureTarget(onunit);

            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            if (!Me.GotTarget || !Me.CurrentTarget.IsAlive)
                return true;

            if (Capabilities.IsInterruptingAllowed && Me.CurrentTarget.Distance <= 15 && Me.CurrentTarget.IsCasting &&
                Me.CurrentTarget.CanInterruptCurrentSpellCast)
                await Interrupts.MindFreezeMethod();

            if (await LowbieRotation(onunit, Me.Combat && Me.GotTarget && Me.Level < 100)) return true;

            // Actual Routine
            // outbreak,target_if=!dot.virulent_plague.ticking
            if (await Spell.CoCast(S.Outbreak, onunit, !Me.CurrentTarget.HasAura(S.AuraVirulentPlague))) return true;

            // dark_transformation,if= equipped.137075 & cooldown.dark_arbiter.remains > 165
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 165))
                return true;

            // dark_transformation,if= equipped.137075 & !talent.shadow_infusion.enabled & cooldown.dark_arbiter.remains > 55
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 && TalentManager.UnholyShadowInfusion &&
                Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 55))
                return true;

            // dark_transformation,if= equipped.137075 & talent.shadow_infusion.enabled & cooldown.dark_arbiter.remains > 35
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 && TalentManager.UnholyShadowInfusion &&
                Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 35))
                return true;

            // dark_transformation,if= equipped.137075 & target.time_to_die < cooldown.dark_arbiter.remains - 8
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                (TimeToDeath.TimeToDeathExtension.TimeToDeath(Me.CurrentTarget) <
                 Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds - 8)))
                return true;

            // dark_transformation,if= equipped.137075 & cooldown.summon_gargoyle.remains > 160
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                Spell.GetCooldownLeft(S.SummonGargoyle).TotalSeconds > 160))
                return true;

            // dark_transformation,if= equipped.137075 & !talent.shadow_infusion.enabled & cooldown.summon_gargoyle.remains > 55
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 && !ShadowInfusionSelected() &&
                Spell.GetCooldownLeft(S.SummonGargoyle).TotalSeconds > 55))
                return true;

            // dark_transformation,if= equipped.137075 & talent.shadow_infusion.enabled & cooldown.summon_gargoyle.remains > 35
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 && ShadowInfusionSelected() &&
                Spell.GetCooldownLeft(S.SummonGargoyle).TotalSeconds > 35))
                return true;

            // dark_transformation,if= equipped.137075 & target.time_to_die < cooldown.summon_gargoyle.remains - 8
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                (TimeToDeath.TimeToDeathExtension.TimeToDeath(Me.CurrentTarget) <
                 Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds - 8)))
                return true;

            // dark_transformation,if= !equipped.137075 & rune <= 3
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation") &&
                Me.Inventory.Equipped.Shoulder.ItemInfo.Id != 137075 && Me.CurrentRunes <= 3))
                return true;

            // blighted_rune_weapon,if=rune<=3
            if (await Spell.CoCast(S.BlightedRuneWeapon, Me,
                Capabilities.IsCooldownUsageAllowed && Me.GotTarget && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentRunes <= 3))
                return true;

            // valkyr,if=talent.dark_arbiter.enabled&pet.valkyr_battlemaiden.active
            if (await ValkyrActive(onunit,
                Me.Combat && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.CanWeAttack() &&
                DarkArbiterSelected() && Me.HasAura(S.AuraSummonGargoyle))) return true;

            // dark_arbiter,if=!equipped.137075&runic_power.deficit<30
            if (await Spell.CoCast(S.DarkArbiter, onunit,
                Me.CurrentTarget.Distance <= 8 && Me.GotTarget && Me.CurrentTarget.CanWeAttack() &&
                RunicPowerDeficit < 30 && Me.Inventory.Equipped.Shoulder.ItemInfo.Id != 137075)) return true;

            // dark_arbiter,if= equipped.137075 & runic_power.deficit < 30 & cooldown.dark_transformation.remains < 2
            if (await Spell.CoCast(S.DarkArbiter, onunit,
                Me.CurrentTarget.Distance <= 8 && Me.GotTarget && Me.CurrentTarget.CanWeAttack() &&
                RunicPowerDeficit < 30 && Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                Spell.GetCooldownLeft(S.DarkTransformation).TotalSeconds < 2)) return true;

            // summon_gargoyle,if=!equipped.137075,if=rune<=3
            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Me.GotTarget && Me.CurrentTarget.Distance <= 30 && GeneralSettings.SummonGargoyleOnCd &&
                Capabilities.IsCooldownUsageAllowed && Me.Inventory.Equipped.Shoulder.ItemInfo.Id != 137075 &&
                Me.CurrentRunes <= 3)) return true;

            // summon_gargoyle,if=equipped.137075&cooldown.dark_transformation.remains<10&rune<=3
            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Me.GotTarget && Me.CurrentTarget.Distance <= 30 && GeneralSettings.SummonGargoyleOnCd &&
                Capabilities.IsCooldownUsageAllowed && Me.Inventory.Equipped.Shoulder.ItemInfo.Id == 137075 &&
                Me.CurrentRunes <= 3 && Spell.GetCooldownLeft(S.DarkTransformation).TotalSeconds < 10)) return true;

            // soul_reaper,if=debuff.festering_wound.stack>=7&cooldown.apocalypse.remains<2

            // apocalypse,if=debuff.festering_wound.stack>=7
            if (await Spell.CoCast(S.Apocalypse, onunit,
                Me.Inventory.Equipped.MainHand.ItemInfo.Id == 128403 &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 7)) return true;

            // death_coil,if=runic_power.deficit<30
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentTarget.Distance <= 40 && RunicPowerDeficit < 30))
                return true;

            // death_coil,if=!talent.dark_arbiter.enabled&buff.sudden_doom.up&!buff.necrosis.up&rune<=3
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentTarget.Distance <= 40 && DarkArbiterSelected() && Me.HasAura(S.AuraSuddenDoom) &&
                !Me.HasAura(S.AuraNecrosis) && Me.CurrentRunes <= 3)) return true;

            // death_coil,if=talent.dark_arbiter.enabled&buff.sudden_doom.up&cooldown.dark_arbiter.remains>5&rune<=3
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentTarget.Distance <= 40 && DarkArbiterSelected() && Me.HasAura(S.AuraSuddenDoom) &&
                Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 5 && Me.CurrentRunes <= 3)) return true;

            // festering_strike,if= debuff.festering_wound.stack < 7 & cooldown.apocalypse.remains < 5
            if (await Spell.CoCast(S.FesteringStrike, onunit, Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                                                              Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) < 7 &&
                                                              Spell.GetCooldownLeft(S.Apocalypse).TotalSeconds < 5))
                return true;

            // wait,sec=cooldown.apocalypse.remains,if=cooldown.apocalypse.remains<=1&cooldown.apocalypse.remains

            // soul_reaper,if=debuff.festering_wound.stack>=3
            if (await Spell.CoCast(S.SoulReaper, onunit,
                SoulReaperSelected() && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return true;

            // festering_strike,if=debuff.soul_reaper.up&!debuff.festering_wound.up
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && SoulReaperSelected() &&
                Me.CurrentTarget.HasAura(S.AuraSoulReaper) &&
                !Me.CurrentTarget.HasAura(S.AuraFesteringWound))) return true;

            // scourge_strike,if=debuff.soul_reaper.up&debuff.festering_wound.stack>=1
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                SoulReaperSelected() && Me.CurrentTarget.HasAura(S.AuraSoulReaper) &&
                Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return true;

            // clawing_shadows,if=debuff.soul_reaper.up&debuff.festering_wound.stack>=1
            if (await Spell.CoCast(S.ClawingShadows, onunit,
                SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 && Me.CurrentTarget.IsWithinMeleeRange &&
                SoulReaperSelected() && Me.CurrentTarget.HasAura(S.AuraSoulReaper) &&
                Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return true;

            // defile
            if (await Spell.CastOnGround(S.Defile, Me,
                Me.GotTarget && Me.CurrentTarget.CanWeAttack() && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving
                && Capabilities.IsAoeAllowed)) return true;

            // call_action_list,name=aoe,if=active_enemies>=2
            //if (await AOE(onunit, Units.ActiveEnemies(Me.Location, 8f).Count() >= 2))
            if (await AOE(onunit, Units.EnemiesInRange(10) >= 2))
                return true;

            // call_action_list,name = instructors,if= equipped.132448
            if (await Instructors(onunit, Me.Inventory.Equipped.Wrist.ItemInfo.Id == 132448))
                return true;

            // call_action_list,name = standard,if= !talent.castigator.enabled & !equipped.132448
            if (await Standard(onunit,
                !TalentManager.UnholyCastigator && Me.Inventory.Equipped.Wrist.ItemInfo.Id != 132448))
                return true;

            // call_action_list,name = castigator,if= talent.castigator.enabled & !equipped.132448
            return await Castigator(onunit,
                TalentManager.UnholyCastigator && Me.Inventory.Equipped.Wrist.ItemInfo.Id != 132448);

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion CombatCoroutine

        #region PreCombatBuffs

        private static async Task<bool> PreCombatBuffs()
        {
            if (Paused) return false;

            if (!Me.IsAlive)
                return true;

            if (await Spell.CoCast(S.RaiseDead, Me,
                Capabilities.IsPetUsageAllowed && !Me.GotAlivePet && Capabilities.IsPetSummonAllowed &&
                !Me.OnTaxi && !Me.Mounted))
                return true;

            //if (!Me.GotTarget || !Me.CurrentTarget.IsAlive || Globals.Mounted)
            //    return true;

            //if (GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.CanWeAttack() &&
            //    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight && Me.IsSafelyFacing(Me.CurrentTarget))
            //{
            //    if (Me.CurrentTarget.Distance > 7 && GeneralSettings.DeathGrip)
            //        return await Spell.CoCast(S.DeathGrip,
            //            SpellManager.CanCast(S.DeathGrip));

            //    if (Spell.GetCooldownLeft(S.Outbreak).TotalSeconds < 1)
            //        return await Spell.CoCast(S.Outbreak,
            //            SpellManager.CanCast(S.Outbreak));
            //}

            return false;
        }

        #endregion PreCombatBuffs

        #region PullBuffs

#pragma warning disable 1998

        private static async Task<bool> PullBuffs()
#pragma warning restore 1998
        {
            if (Paused) return false;

            //return await Spell.CoCast(S.Outbreak,
            //    GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.CanWeAttack() &&
            //    Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight &&
            //    Me.IsSafelyFacing(Me.CurrentTarget));

            return true;
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

            if (Me.Combat)
            {
                await Defensives.DefensivesMethod();
            }

            if (SpellManager.GlobalCooldown)
                return false;

            if (await Spell.CoCast(S.RaiseDead, Me,
                Capabilities.IsPetUsageAllowed && !Me.GotAlivePet && Capabilities.IsPetSummonAllowed &&
                !Me.OnTaxi && !Me.Mounted))
                return true;

            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
                return true;
            }

            // Attack if not attacking
            if (Capabilities.IsPetUsageAllowed && !Me.Pet.IsAutoAttacking && Me.GotAlivePet)
            {
                await PetAttack();
                return true;
            }

            //await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Valkyr

        private static async Task<bool> ValkyrActive(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            // death_coil
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower >= 35 && Me.CurrentTarget.Distance <= 40))
                return true;

            // apocalypse,if=debuff.festering_wound.stack= 8
            if (await Spell.CoCast(S.Apocalypse, onunit,
                Me.Inventory.Equipped.MainHand.ItemInfo.Id == 128403 &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) == 8)) return true;

            // festering_strike,if=debuff.festering_wound.stack<8&cooldown.apocalypse.remains<5
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) < 8 &&
                Spell.GetCooldownLeft(S.Apocalypse).TotalSeconds < 5)) return true;

            // call_action_list,name=aoe,if=active_enemies>=2
            if (await AOE(onunit, Units.EnemiesInRange(10) >= 2))
            {
                return true;
            }

            // festering_strike,if=debuff.festering_wound.stack<=3
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 3)) return true;

            // scourge_strike,if=debuff.festering_wound.up
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && !SpellManager.HasSpell(S.ClawingShadows) &&
                Me.CurrentTarget.HasAura(S.AuraFesteringWound))) return true;

            // clawing_shadows,if=debuff.festering_wound.up
            return await Spell.CoCast(S.ClawingShadows, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && SpellManager.HasSpell(S.ClawingShadows) &&
                Me.CurrentRunes >= 1 &&
                Me.CurrentTarget.HasAura(S.AuraFesteringWound));

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion Routine Valkyr

        #region LowbieRotation

        private static async Task<bool> LowbieRotation(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            //Custom - SingularBased LowbieRotation
            if (await Spell.CoCast(S.RaiseDead, Me,
                Capabilities.IsPetUsageAllowed && !Me.GotAlivePet && Capabilities.IsPetSummonAllowed &&
                !Me.OnTaxi && !Me.Mounted))
                return true;

            if (await Spell.CoCast(S.DeathStrike, onunit,
                (Me.HasActiveAura("Dark Succor") && Me.HealthPercent < 80) || Me.HealthPercent <= 40))
                return true;

            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Capabilities.IsCooldownUsageAllowed &&
                GeneralSettings.SummonGargoyleOnCd)) return true;

            if (await Spell.CoCast(S.Outbreak, onunit,
                Me.CurrentTarget.GetAuraTimeLeft(S.AuraVirulentPlague).TotalSeconds < 1.8)) return true;

            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet &&
                !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation")))
                return true;

            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) < 5))
                return true;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1))
                return true;

            return await Spell.CoCast(S.DeathCoil, onunit, RunicPowerDeficit < 10);

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion LowbieRotation

        #region Instructors

        private static async Task<bool> Instructors(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // festering_strike,if= debuff.festering_wound.stack <= 4 & runic_power.deficit > 23
            if (await
                Spell.CoCast(S.FesteringStrike, onunit,
                    Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 4 && RunicPowerDeficit > 23)) return true;

            // death_coil,if= !buff.necrosis.up & talent.necrosis.enabled & rune <= 3
            if (await
                Spell.CoCast(S.DeathCoil, onunit,
                    Me.CurrentTarget.Distance <= 40 && Me.HasAura(S.AuraNecrosis) && Me.CurrentRunes <= 3))
                return true;

            if (Me.CurrentTarget.HasAura("Festering Wound") &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 5 && RunicPowerDeficit > 29)
            {
                // scourge_strike,if= buff.necrosis.react & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraNecrosis))) return true;

                // clawing_shadows,if= buff.necrosis.react & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                        Me.CurrentTarget.Distance <= 8 &&
                        Me.HasAura(S.AuraNecrosis)))
                    return true;

                // scourge_strike,if= buff.unholy_strength.react & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraUnholyStrength))) return true;

                // clawing_shadows,if= buff.unholy_strength.react & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                        Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraUnholyStrength))) return true;

                // scourge_strike,if= rune >= 2 & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentRunes >= 2)) return true;

                // clawing_shadows,if= rune >= 2 & debuff.festering_wound.stack >= 5 & runic_power.deficit > 29
                if (await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                        Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentRunes >= 2)) return true;
            }
            if (Me.CurrentTarget.Distance <= 40)
            {
                // death_coil,if=talent.shadow_infusion.enabled&talent.dark_arbiter.enabled&!buff.dark_transformation.up&cooldown.dark_arbiter.remains>15
                if (await
                    Spell.CoCast(S.DeathCoil, onunit,
                        ShadowInfusionSelected() && DarkArbiterSelected() &&
                        !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") &&
                        Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15)) return true;

                // death_coil,if=talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled&!buff.dark_transformation.up
                if (await
                    Spell.CoCast(S.DeathCoil, onunit,
                        ShadowInfusionSelected() && !DarkArbiterSelected() &&
                        !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation"))) return true;

                // death_coil,if=talent.dark_arbiter.enabled&cooldown.dark_arbiter.remains>15
                if (await Spell.CoCast(S.DeathCoil, onunit,
                    DarkArbiterSelected() && Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15))
                    return true;

                // death_coil,if=!talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled
                if (await Spell.CoCast(S.DeathCoil, onunit, !ShadowInfusionSelected() && !DarkArbiterSelected()))
                    return true;
            }

            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- START
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 5))
                return false;

            if (await Spell.CoCast(S.SoulReaper, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && TalentManager.UnholySoulReaper &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3))
                return false;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.ClawingShadows, onunit,
                SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.Apocalypse, onunit,
                SpellManager.HasSpell(S.Apocalypse) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 7)) return false;

            if (TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;
            }

            if (!TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;
            }

            return await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 50);

            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- END

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion

        #region Standard

        private static async Task<bool> Standard(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // festering_strike,if=debuff.festering_wound.stack<=4&runic_power.deficit>23
            if (await
                Spell.CoCast(S.FesteringStrike, onunit,
                    Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasAura(S.AuraFesteringWound) &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 4 && RunicPowerDeficit > 23)) return true;

            // death_coil,if= !buff.necrosis.up & talent.necrosis.enabled & rune <= 3
            if (await
                Spell.CoCast(S.DeathCoil, onunit,
                    Me.CurrentTarget.Distance <= 40 && !Me.HasAura(S.AuraNecrosis) && Me.CurrentRunes <= 3))
                return true;

            // debuff.festering_wound.stack>=1&runic_power.deficit>15
            if (Me.CurrentTarget.HasAura("Festering Wound") &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1 && RunicPowerDeficit > 15)
            {
                // scourge_strike,if=buff.necrosis.react&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraNecrosis))) return true;

                // clawing_shadows,if=buff.necrosis.react&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                    Me.CurrentTarget.Distance <= 8 &&
                    Me.HasAura(S.AuraNecrosis)))
                    return true;

                // scourge_strike,if=buff.unholy_strength.react&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.HasAura(S.AuraUnholyStrength))) return true;

                // clawing_shadows,if=buff.unholy_strength.react&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                    Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.HasAura(S.AuraUnholyStrength))) return true;


                // scourge_strike,if=rune>=2&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentRunes >= 2)) return true;

                // clawing_shadows,if=rune>=2&debuff.festering_wound.stack>=1&runic_power.deficit>15
                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                    Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentRunes >= 2)) return true;
            }

            if (Me.CurrentTarget.Distance <= 40)
            {
                // death_coil,if=talent.shadow_infusion.enabled&talent.dark_arbiter.enabled&!buff.dark_transformation.up&cooldown.dark_arbiter.remains>15
                if (await Spell.CoCast(S.DeathCoil, onunit,
                    ShadowInfusionSelected() && DarkArbiterSelected() &&
                    !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") &&
                    Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15)) return true;

                // death_coil,if=talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled&!buff.dark_transformation.up
                if (await Spell.CoCast(S.DeathCoil, onunit,
                    ShadowInfusionSelected() && !DarkArbiterSelected() &&
                    !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation"))) return true;

                // death_coil,if=talent.dark_arbiter.enabled&cooldown.dark_arbiter.remains>15
                if (await Spell.CoCast(S.DeathCoil, onunit,
                    DarkArbiterSelected() && Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15))
                    return true;

                // death_coil,if=!talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled
                if (await Spell.CoCast(S.DeathCoil, onunit, !ShadowInfusionSelected() && !DarkArbiterSelected()))
                    return true;
            }


            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- START
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 5))
                return false;

            if (await Spell.CoCast(S.SoulReaper, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && TalentManager.UnholySoulReaper &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3))
                return false;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.ClawingShadows, onunit,
                SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.Apocalypse, onunit,
                SpellManager.HasSpell(S.Apocalypse) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 7)) return false;

            if (TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;
            }

            if (!TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;
            }

            return await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 50);

            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- END

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion

        #region Castigator

        private static async Task<bool> Castigator(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // festering_strike,if= debuff.festering_wound.stack <= 4 & runic_power.deficit > 23
            if (await
                Spell.CoCast(S.FesteringStrike, onunit,
                    Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 4 && RunicPowerDeficit > 23)) return true;

            // death_coil,if= !buff.necrosis.up & talent.necrosis.enabled & rune <= 3
            if (await
                Spell.CoCast(S.DeathCoil, onunit,
                    Me.CurrentTarget.Distance <= 40 && !Me.HasAura(S.AuraNecrosis) && Me.CurrentRunes <= 3))
                return true;

            if (Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3 && RunicPowerDeficit > 23)
            {
                // scourge_strike,if= buff.necrosis.react & debuff.festering_wound.stack >= 3 & runic_power.deficit > 23
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraNecrosis))) return true;

                // scourge_strike,if= buff.unholy_strength.react & debuff.festering_wound.stack >= 3 & runic_power.deficit > 23
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraUnholyStrength))) return true;

                // scourge_strike,if= rune >= 2 & debuff.festering_wound.stack >= 3 & runic_power.deficit > 23
                if (await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentRunes >= 2)) return true;
            }
            if (Me.CurrentTarget.Distance <= 40)
            {
                // death_coil,if= talent.shadow_infusion.enabled & talent.dark_arbiter.enabled & !buff.dark_transformation.up & cooldown.dark_arbiter.remains > 15
                if (await
                    Spell.CoCast(S.DeathCoil, onunit,
                        ShadowInfusionSelected() && DarkArbiterSelected() &&
                        !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") &&
                        Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15)) return true;

                // death_coil,if= talent.shadow_infusion.enabled & !talent.dark_arbiter.enabled & !buff.dark_transformation.up
                if (await
                    Spell.CoCast(S.DeathCoil, onunit,
                        ShadowInfusionSelected() && !DarkArbiterSelected() &&
                        !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation"))) return true;

                // death_coil,if= talent.dark_arbiter.enabled & cooldown.dark_arbiter.remains > 15
                if (await
                    Spell.CoCast(S.DeathCoil, onunit,
                        DarkArbiterSelected() && Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15))
                    return true;

                // death_coil,if= !talent.shadow_infusion.enabled & !talent.dark_arbiter.enabled
                if (await Spell.CoCast(S.DeathCoil, onunit, !ShadowInfusionSelected() && !DarkArbiterSelected()))
                    return true;
            }

            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- START
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 5))
                return false;

            if (await Spell.CoCast(S.SoulReaper, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && TalentManager.UnholySoulReaper &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3))
                return false;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.ClawingShadows, onunit,
                SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.HasAura(S.AuraSoulReaper))) return false;

            if (await Spell.CoCast(S.Apocalypse, onunit,
                SpellManager.HasSpell(S.Apocalypse) && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 7)) return false;

            if (TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return false;
            }

            if (!TalentManager.UnholyCastigator)
            {
                if (await Spell.CoCast(S.ScourgeStrike, onunit,
                    !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;

                if (await Spell.CoCast(S.ClawingShadows, onunit,
                    SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                    Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return false;
            }

            return await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 50);

            //CUSTOM - BASED ON ICY VEINS' SOUL REAPER PRIORITYLIST -- END

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion

        #region AOE

        // ReSharper disable once InconsistentNaming
        private static async Task<bool> AOE(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // death_and_decay,if=spell_targets.death_and_decay>=2
            if (await Spell.CastOnGround(S.DeathandDecay, Me,
                Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.IsMoving &&
                Capabilities.IsAoeAllowed && Units.EnemiesInRange(10) >= 2))
                return true;

            // custom defile
            if (await Spell.CastOnGround(S.Defile, Me,
                TalentManager.UnholyDefile && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving &&
                Capabilities.IsAoeAllowed))
                return true;

            // custom deathanddecay
            if (await Spell.CastOnGround(S.DeathandDecay, Me,
                !TalentManager.UnholyDefile && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving &&
                Capabilities.IsAoeAllowed))
                return true;

            // epidemic,if=spell_targets.epidemic>4
            if (await Spell.CoCast(S.Epidemic, onunit,
                Capabilities.IsCooldownUsageAllowed && Capabilities.IsAoeAllowed &&
                Me.CurrentTarget.IsWithinMeleeRange && Units.EnemiesInRange(10) > 4))
                return true;

            // scourge_strike,if=spell_targets.scourge_strike>=2&(dot.death_and_decay.ticking|dot.defile.ticking)
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                Units.EnemiesInRange(5) >= 2 && (Me.HasAura(S.AuraDeathandDecay) || Me.HasAura(S.AuraDefile))))
                return true; //Recheck logic

            // clawing_shadows,if=spell_targets.clawing_shadows>=2&(dot.death_and_decay.ticking|dot.defile.ticking)
            if (await Spell.CoCast(S.ClawingShadows, onunit,
                SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentRunes >= 1 &&
                Me.CurrentTarget.IsWithinMeleeRange &&
                Units.EnemiesInRange(5) >= 2 && (Me.HasAura(S.AuraDeathandDecay) || Me.HasAura(S.AuraDefile))))
                return true; //Recheck logic

            // epidemic,if=spell_targets.epidemic>2
            return await Spell.CoCast(S.Epidemic, onunit,
                Capabilities.IsCooldownUsageAllowed && Capabilities.IsAoeAllowed && EpidemicSelected() &&
                Me.CurrentTarget.IsWithinMeleeRange && Units.EnemiesInRange(10) > 2);

            //await CommonCoroutines.SleepForLagDuration();
        }

        #endregion AOE

        #region Openers

        //private static async Task<bool> ExampleOpener(bool reqs)
        //{
        //    if (Paused) return false;

        //    if (!reqs) return false;

        //    await CommonCoroutines.SleepForLagDuration();

        //    return true;
        //}

        #endregion Openers

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

            //await CommonCoroutines.SleepForLagDuration();

            return await Coroutine.Wait(1000, () => Me.HasAura("Food"));
        }

        public static bool CanBuffEat()
        {
            return !Me.Mounted && !Me.IsDead && !Me.IsGhost && !Me.IsOnTransport && !Me.OnTaxi;
        }

        #endregion RestCoroutine

        #region Logics

        #region TalentSelected

        public static bool EpidemicSelected()
        {
            return TalentManager.UnholyEpidemic;
        }

        public static bool PestilentPustulesSelected()
        {
            return TalentManager.UnholyPestilentPustules;
        }

        public static bool ShadowInfusionSelected()
        {
            return TalentManager.UnholyShadowInfusion;
        }

        public static bool DarkArbiterSelected()
        {
            return TalentManager.UnholyDarkArbiter;
        }

        public static bool DefileSelected()
        {
            return TalentManager.UnholyDefile;
        }

        public static bool SoulReaperSelected()
        {
            return TalentManager.UnholySoulReaper;
        }

        #endregion TalentSelected

        #region PetAttack

        private static async Task<bool> PetAttack()
        {
            if (Me.GotAlivePet && Me.GotTarget && Me.CurrentTarget.CanWeAttack())
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

        protected override Composite CreateDeathBehavior()
        {
            return new ActionRunCoroutine(ret => DeathKnight.Death.DeathBehavor());
        }

        public override bool NeedDeath => Me.IsDead;

        #endregion Overrides
    }
}