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
            if (Paused) return false;

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


            if (await ExampleOpener(Me.Level >= 100))
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

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Heals

        #region CombatCoroutine

        private static async Task<bool> CombatCoroutine(WoWUnit onunit)
        {
            if (Paused) return false;

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

            //if (await LowbieRotation(onunit, Me.Combat && Me.GotTarget && Me.Level < 100)) return true;

            if (await LowbieRotation(onunit, Me.Combat && Me.GotTarget && Me.Level < 100))
            {
                return true;
            }


            // Actual Routine
            // outbreak,target_if=!dot.virulent_plague.ticking
            if (await Spell.CoCast(S.Outbreak, onunit, !Me.CurrentTarget.HasMyAura(S.AuraVirulentPlague))) return true;

            // dark_transformation
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet && !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation")))
                return true;

            // blighted_rune_weapon
            if (await Spell.CoCast(S.BlightedRuneWeapon, Me,
                Capabilities.IsCooldownUsageAllowed && Me.GotTarget && Me.Combat && Me.CurrentTarget.IsWithinMeleeRange)) return true;

            // run_action_list,name=valkyr,if=talent.dark_arbiter.enabled&pet.valkyr_battlemaiden.active
            //if (await ValkyrActive(onunit,
            //            Me.Combat && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable &&
            //            DarkArbiterSelected() && Me.HasAura(S.AuraSummonGargoyle)))
            //    return true;

            if (await ValkyrActive(onunit,
                        Me.Combat && Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.Attackable &&
                        DarkArbiterSelected() && Me.HasAura(S.AuraSummonGargoyle)))
            {
                return true;
            }

            // dark_arbiter,if=runic_power>80
            if (await Spell.CoCast(S.DarkArbiter, Me,
                Me.CurrentTarget.Distance <= 8 && Me.GotTarget && Me.CurrentTarget.Attackable &&
                Me.CurrentRunicPower > 80)) return true;

            // summon_gargoyle
            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Me.GotTarget && Me.CurrentTarget.Distance <= 30 && DeathKnightSettings.Instance.SummonGargoyleOnCd &&
                Capabilities.IsCooldownUsageAllowed)) return true;

            // death_coil,if=runic_power>80
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentTarget.Distance <= 40 && Me.CurrentRunicPower > 80))
                return true;

            // death_coil,if=talent.dark_arbiter.enabled&buff.sudden_doom.react&cooldown.dark_arbiter.remains>5
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentTarget.Distance <= 40 && DarkArbiterSelected() && Me.HasAura(S.AuraSuddenDoom) &&
                Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 5)) return true;

            // death_coil,if=!talent.dark_arbiter.enabled&buff.sudden_doom.react
            if (await Spell.CoCast(S.DeathCoil, onunit,
                Me.CurrentTarget.Distance <= 40 && !DarkArbiterSelected() && Me.HasAura(S.AuraSuddenDoom)))
                return true;

            // soul_reaper,if=debuff.festering_wound.stack>=3
            if (await Spell.CoCast(S.SoulReaper, onunit,
                SoulReaperSelected() && Me.CurrentTarget.IsWithinMeleeRange &&
                Me.CurrentTarget.HasMyAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 3)) return true;

            // festering_strike,if=debuff.soul_reaper.up&!debuff.festering_wound.up
            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && SoulReaperSelected() &&
                Me.CurrentTarget.HasMyAura(S.AuraSoulReaper) &&
                !Me.CurrentTarget.HasMyAura(S.AuraFesteringWound))) return true;

            // scourge_strike,if=debuff.soul_reaper.up&debuff.festering_wound.stack>=1
            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                SoulReaperSelected() && Me.CurrentTarget.HasMyAura(S.AuraSoulReaper) &&
                Me.CurrentTarget.HasMyAura(S.AuraFesteringWound) &&
                Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return true;

            // clawing_shadows,if=debuff.soul_reaper.up&debuff.festering_wound.stack>=1
            if (await Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        SoulReaperSelected() && Me.CurrentTarget.HasMyAura(S.AuraSoulReaper) &&
                        Me.CurrentTarget.HasMyAura(S.AuraFesteringWound) &&
                        Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1)) return true;

            // defile
            if (await Spell.CastOnGround(S.Defile, Me,
                Me.GotTarget && Me.CurrentTarget.Attackable && Me.CurrentTarget.IsWithinMeleeRange &&
                !Me.CurrentTarget.IsMoving
                && Capabilities.IsAoeAllowed)) return true;

            // call_action_list,name=aoe,if=active_enemies>=2
            if (await AOE(onunit, Units.EnemiesInRange(10) >= 2))
            {
                return true;
            }

            // festering_strike,if=debuff.festering_wound.stack<=4
            if (
                await
                    Spell.CoCast(S.FesteringStrike, onunit,
                        Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasMyAura(S.AuraFesteringWound) &&
                        Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 4)) return true;

            // scourge_strike,if=buff.necrosis.react
            if (
                await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraNecrosis))) return true; //Aura on Me or Target?

            // clawing_shadows,if=buff.necrosis.react
            if (
                await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraNecrosis))) return true;

            // scourge_strike,if=buff.unholy_strength.react
            if (
                await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraUnholyStrength))) return true;

            // clawing_shadows,if=buff.unholy_strength.react
            if (
                await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.HasAura(S.AuraUnholyStrength))) return true;

            // scourge_strike,if=rune>=3
            if (
                await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentRunes >= 3)) return true; // Check RuneCount API

            // clawing_shadows,if=rune>=3
            if (
                await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Me.CurrentRunes >= 3)) return true;

            if (Me.CurrentTarget.Distance <= 40)
            {
                // death_coil,if=talent.shadow_infusion.enabled&talent.dark_arbiter.enabled&!buff.dark_transformation.up&cooldown.dark_arbiter.remains>15
                if (
                    await
                        Spell.CoCast(S.DeathCoil, onunit,
                            ShadowInfusionSelected() && DarkArbiterSelected() &&
                            !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") &&
                            Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15)) return true;

                // death_coil,if=talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled&!buff.dark_transformation.up
                if (
                    await
                        Spell.CoCast(S.DeathCoil, onunit,
                            ShadowInfusionSelected() && !DarkArbiterSelected() &&
                            !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation"))) return true;

                // death_coil,if=talent.dark_arbiter.enabled&cooldown.dark_arbiter.remains>15
                if (
                    await
                        Spell.CoCast(S.DeathCoil, onunit,
                            DarkArbiterSelected() && Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15))
                    return true;

                // death_coil,if=!talent.shadow_infusion.enabled&!talent.dark_arbiter.enabled
                if (await Spell.CoCast(S.DeathCoil, onunit, !ShadowInfusionSelected() && !DarkArbiterSelected()))
                    return true;
            }

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
            if (Paused) return false;

            if (!Me.IsAlive)
                return true;

            //if (await Spell.CoCast(S.UnholyPresence, Me, !Me.HasAura(S.UnholyPresence)))
            //    return true;

            if (
                await
                    Spell.CoCast(S.RaiseDead, Me,
                        !Me.GotAlivePet && Capabilities.IsPetSummonAllowed && !Me.OnTaxi && !Me.Mounted))
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

            if (await Spell.CoCast(S.RaiseDead, Me, !Me.GotAlivePet && Capabilities.IsPetSummonAllowed && !Me.OnTaxi && !Me.Mounted))
                return true;

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion CombatBuffs

        #region Coroutine Valkyr

        private static async Task<bool> ValkyrActive(WoWUnit onunit, bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            // death_coil
            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower >= 35 && Me.CurrentTarget.Distance <= 40))
                return true;

            // call_action_list,name=aoe,if=active_enemies>=2
            if (await AOE(onunit, Units.EnemiesInRange(10) >= 2))
            {
                return true;
            }

            // festering_strike,if=debuff.festering_wound.stack<=6
            if (
                await
                    Spell.CoCast(S.FesteringStrike, onunit,
                        Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HasMyAura(S.AuraFesteringWound) &&
                        Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) <= 6)) return true; //Fix stacks checking

            // scourge_strike,if=debuff.festering_wound.up
            if (
                await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        Me.CurrentTarget.IsWithinMeleeRange && !SpellManager.HasSpell(S.ClawingShadows) &&
                        Me.CurrentTarget.HasMyAura(S.AuraFesteringWound))) return true;

            // clawing_shadows,if=debuff.festering_wound.up
            if (
                await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        Me.CurrentTarget.IsWithinMeleeRange && SpellManager.HasSpell(S.ClawingShadows) &&
                        Me.CurrentTarget.HasMyAura(S.AuraFesteringWound))) return true;


            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Coroutine Valkyr

        #region LowbieRotation

        private static async Task<bool> LowbieRotation(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            //Custom - SingularBased LowbieRotation
            if (await Spell.CoCast(S.SummonGargoyle, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.CurrentTarget.IsStressful() && DeathKnightSettings.Instance.SummonGargoyleOnCd)) return true;

            if (await Spell.CoCast(S.Outbreak, onunit,
                Me.CurrentTarget.GetAuraTimeLeft(S.AuraVirulentPlague).TotalSeconds < 1.8)) return true;

            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet && !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation")))
                return true;

            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) < 5))
                return true;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.GetAuraStacks(S.AuraFesteringWound) >= 1))
                return true;

            if (await Spell.CoCast(S.DeathCoil, onunit, RunicPowerDeficit < 10)) return true;


            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion LowbieRotation

        #region AOE

        // ReSharper disable once InconsistentNaming
        private static async Task<bool> AOE(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            // dark_transformation
            if (await Spell.CoCast(S.DarkTransformation, onunit,
                Capabilities.IsCooldownUsageAllowed && Me.GotAlivePet && !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") && !Me.HasActiveAura("Dark Transformation")))
                return true;

            // death_and_decay,if=spell_targets.death_and_decay>=2
            if (await Spell.CastOnGround(S.DeathandDecay, Me,
                Me.GotTarget && Me.CurrentTarget.IsWithinMeleeRange && !Me.CurrentTarget.IsMoving &&
                Capabilities.IsAoeAllowed && Units.EnemiesInRange(10) >= 2))
                return true;

            // epidemic,if=spell_targets.epidemic>4
            if (
                await
                    Spell.CoCast(S.Epidemic, onunit, Capabilities.IsCooldownUsageAllowed && Capabilities.IsAoeAllowed && Me.CurrentTarget.IsWithinMeleeRange && Units.EnemiesInRange(10) > 4))
                return true;

            // scourge_strike,if=spell_targets.scourge_strike>=2&(dot.death_and_decay.ticking|dot.defile.ticking)
            if (
                await
                    Spell.CoCast(S.ScourgeStrike, onunit,
                        !SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Units.EnemiesInRange(5) >= 2 && (Me.HasAura(S.AuraDeathandDecay) || Me.HasAura(S.AuraDefile))))
                return true; //Recheck logic

            // clawing_shadows,if=spell_targets.clawing_shadows>=2&(dot.death_and_decay.ticking|dot.defile.ticking)
            if (
                await
                    Spell.CoCast(S.ClawingShadows, onunit,
                        SpellManager.HasSpell(S.ClawingShadows) && Me.CurrentTarget.IsWithinMeleeRange &&
                        Units.EnemiesInRange(5) >= 2 && (Me.HasAura(S.AuraDeathandDecay) || Me.HasAura(S.AuraDefile))))
                return true; //Recheck logic

            // epidemic,if=spell_targets.epidemic>2
            if (
                await
                    Spell.CoCast(S.Epidemic, onunit,
                        Capabilities.IsCooldownUsageAllowed && Capabilities.IsAoeAllowed && EpidemicSelected() && Me.CurrentTarget.IsWithinMeleeRange && Units.EnemiesInRange(10) > 2))
                return true;

            if (await Spell.CoCast(S.FesteringStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.DeathCoil, onunit,
                            ShadowInfusionSelected() && DarkArbiterSelected() &&
                            !Me.Pet.ActiveAuras.ContainsKey("Dark Transformation") &&
                            Spell.GetCooldownLeft(S.DarkArbiter).TotalSeconds > 15)) return true;

            if (await Spell.CoCast(S.DeathCoil, onunit, Me.CurrentRunicPower > 60)) return true;

            if (await Spell.CoCast(S.ClawingShadows, onunit,
                Me.CurrentTarget.IsWithinMeleeRange)) return true;

            if (await Spell.CoCast(S.ScourgeStrike, onunit,
                Me.CurrentTarget.IsWithinMeleeRange)) return true;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion AOE

        #region Openers

        private static async Task<bool> ExampleOpener(bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;
            if (await Spell.CoCast(S.ArmyoftheDead,
                Capabilities.IsCooldownUsageAllowed && DeathKnightSettings.Instance.UseAotD)) return true;

            return true;
        }

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

            await CommonCoroutines.SleepForLagDuration();

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
            return TalentManager.IsSelected(4);
        }

        public static bool PestilentPustulesSelected()
        {
            return TalentManager.IsSelected(5);
        }

        public static bool ShadowInfusionSelected()
        {
            return TalentManager.IsSelected(16);
        }

        public static bool DarkArbiterSelected()
        {
            return TalentManager.IsSelected(19);
        }

        public static bool DefileSelected()
        {
            return TalentManager.IsSelected(20);
        }

        public static bool SoulReaperSelected()
        {
            return TalentManager.IsSelected(21);
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
    }
}