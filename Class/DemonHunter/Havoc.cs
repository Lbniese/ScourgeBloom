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

namespace ScourgeBloom.Class.DemonHunter
{
    [UsedImplicitly]
    public class Havoc : ScourgeBloom
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


            if (GeneralSettings.Instance.AutoAttack && Me.GotTarget && Me.CurrentTarget.Attackable &&
                Me.CurrentTarget.Distance <= 30 && Me.CurrentTarget.InLineOfSight && Me.IsSafelyFacing(Me.CurrentTarget))
            {
                // pull spells


                await CommonCoroutines.SleepForLagDuration();
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

            // if (await ExampleOpener(Me.Combat))
            // {
            //     return false;
            // }

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        #endregion Pull

        #region Openers

        private static async Task<bool> ExampleOpener(bool reqs)
        {
            if (Paused) return false;

            if (!reqs) return false;

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        #endregion Openers

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

            //if (await LowbieRotation(onunit, Me.Combat && Me.GotTarget && Me.Level < 100)) return true;

            //if (await LowbieRotation(onunit, Me.Combat && Me.GotTarget && Me.Level < 100))
            //{
            //    return true;
            //}

            // Actual Routine
            //  auto_attack
            //  pick_up_fragment,if= talent.demonic_appetite.enabled & fury.deficit >= 30
            //  consume_magic
            // Vengeful Retreat backwards through the target to minimize downtime.
            //  vengeful_retreat,if= (talent.prepared.enabled | talent.momentum.enabled) & buff.prepared.down & buff.momentum.down
            // Fel Rush for Momentum and for fury from Fel Mastery.
            //  fel_rush,animation_cancel = 1,if= (talent.momentum.enabled | talent.fel_mastery.enabled) & (!talent.momentum.enabled | (charges = 2 | cooldown.vengeful_retreat.remains > 4) & buff.momentum.down) & (!talent.fel_mastery.enabled | fury.deficit >= 25) & raid_event.movement.in> charges * 10
            //  eye_beam,if= talent.demonic.enabled & buff.metamorphosis.down & (!talent.first_blood.enabled | fury >= 80 | fury.deficit < 30)
            // If Metamorphosis is ready, pool fury first before using it.
            //  demons_bite,sync = metamorphosis,if= fury.deficit >= 25
            //  call_action_list,name = cooldown
            //  fury_of_the_illidari,if= active_enemies > desired_targets | raid_event.adds.in> 55
            // Use Death Sweep if it is more effective than Annihilation. See the Demon Hunter section of the wiki for how this is calculated.
            //  death_sweep,if= death_sweep_worth_using
            //  demons_bite,if= buff.metamorphosis.remains > gcd & cooldown.blade_dance.remains < gcd & fury < 70 & death_sweep_worth_using
            // Use Blade Dance if it is more effective than Chaos Strike. See the Demon Hunter section of the wiki for how this is calculated.
            //  blade_dance,if= blade_dance_worth_using
            // Use Fel Barrage at max charges, saving it for Momentum and adds if possible.
            //  fel_barrage,if= charges >= 5 & (buff.momentum.up | !talent.momentum.enabled) & (active_enemies > desired_targets | raid_event.adds.in> 30)
            //  throw_glaive,if= talent.bloodlet.enabled & spell_targets >= 2 + talent.chaos_cleave.enabled & (!talent.master_of_the_glaive.enabled | !talent.momentum.enabled | buff.momentum.up)
            //  fel_eruption
            //  felblade,if= fury.deficit >= 30 + buff.prepared.up * 8
            //  annihilation,if= !talent.momentum.enabled | buff.momentum.up | fury.deficit <= 30 + buff.prepared.up * 8 | buff.metamorphosis.remains < 2
            //  throw_glaive,if= talent.bloodlet.enabled & (!talent.master_of_the_glaive.enabled | !talent.momentum.enabled | buff.momentum.up)
            //  eye_beam,if= !talent.demonic.enabled & (spell_targets.eye_beam_tick > desired_targets | (raid_event.adds.in> 45 & buff.metamorphosis.down & (artifact.anguish_of_the_deceiver.enabled | active_enemies > 1)))
            // Pool fury for various different upcoming abilities.
            //  demons_bite,if= buff.metamorphosis.down & cooldown.blade_dance.remains < gcd & fury < 55 & blade_dance_worth_using
            //  demons_bite,if= talent.demonic.enabled & buff.metamorphosis.down & cooldown.eye_beam.remains < gcd & fury.deficit >= 20
            //  demons_bite,if= talent.demonic.enabled & buff.metamorphosis.down & cooldown.eye_beam.remains < 2 * gcd & fury.deficit >= 45
            //  throw_glaive,if= buff.metamorphosis.down & spell_targets >= 3
            //  chaos_strike,if= !talent.momentum.enabled | buff.momentum.up | fury.deficit <= 30 + buff.prepared.up * 8
            // Use Fel Barrage if its nearing max charges, saving it for Momentum and adds if possible.
            //  fel_barrage,if= charges = 4 & buff.metamorphosis.down & (buff.momentum.up | !talent.momentum.enabled) & (active_enemies > desired_targets | raid_event.adds.in> 30)
            //  fel_rush,animation_cancel = 1,if= !talent.momentum.enabled & raid_event.movement.in> charges * 10
            //  demons_bite
            //  fel_rush,if= movement.distance > 15 | (buff.out_of_range.up & !talent.momentum.enabled)
            //  vengeful_retreat,if= movement.distance > 15


            if (Capabilities.IsTargetingAllowed)
                MovementManager.AutoTarget();

            if (Capabilities.IsMovingAllowed || Capabilities.IsFacingAllowed)
                await MovementManager.MoveToTarget();

            await CommonCoroutines.SleepForLagDuration();

            return false;
        }

        private static async Task<bool> LowbieRotation(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            //Custom - SingularBased LowbieRotation


            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        private static async Task<bool> Cooldown(WoWUnit onunit, bool reqs)
        {
            if (!reqs) return false;

            //  use_item,slot = trinket2,if= buff.chaos_blades.up | !talent.chaos_blades.enabled
            //  nemesis,target_if = min:target.time_to_die,if= raid_event.adds.exists & debuff.nemesis.down & (active_enemies > desired_targets | raid_event.adds.in> 60)
            //  nemesis,if= !raid_event.adds.exists & (cooldown.metamorphosis.remains > 100 | target.time_to_die < 70)
            //  nemesis,sync = metamorphosis,if= !raid_event.adds.exists
            //  chaos_blades,if= buff.metamorphosis.up | cooldown.metamorphosis.remains > 100 | target.time_to_die < 20
            // Use Metamorphosis if Nemesis and Chaos Blades are ready.
            //  metamorphosis,if= buff.metamorphosis.down & (!talent.demonic.enabled | !cooldown.eye_beam.ready) & (!talent.chaos_blades.enabled | cooldown.chaos_blades.ready) & (!talent.nemesis.enabled | debuff.nemesis.up | cooldown.nemesis.ready)
            //  potion,name = draenic_agility_potion,if= buff.metamorphosis.remains > 25
      

            await CommonCoroutines.SleepForLagDuration();

            return true;
        }

        // ReSharper disable once InconsistentNaming
        private static async Task<bool> AOE(WoWUnit onunit, bool reqs)
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

        #region TalentSelected

        public static bool AbyssalStrikeSelected()
        {
            return TalentManager.IsSelected(1);
        }

        public static bool AgonizingFlamesSelected()
        {
            return TalentManager.IsSelected(2);
        }

        public static bool RazorSpikesSelected()
        {
            return TalentManager.IsSelected(3);
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
    }
}