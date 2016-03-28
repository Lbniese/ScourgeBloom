/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot.Coroutines;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScourgeBloom.Class
{
    public class Trinkets : ScourgeBloom
    {
        public static async Task<bool> Trinket1Method()
        {
            if (!Item.CanUseItem(Item.Trinket1))
                return false;

            if (GeneralSettings.Trinket1LossOfControl)
            {
                if (StyxWoW.Me.IsCrowdControlled())
                    await UseTrinket1();

                return false;
            }

            if (GeneralSettings.Trinket1OnCooldown)
                return await UseTrinket1();

            if (GeneralSettings.Trinket1OnBurst &&
                StyxWoW.Me.HasAura(SpellLists.BreathofSindragosa | SpellLists.PillarofFrost | SpellLists.UnholyBlight |
                                   SpellLists.SummonGargoyle))
                return await UseTrinket1();

            if (StyxWoW.Me.GotTarget && GeneralSettings.Trinket1EnemyHealthBelow &&
                StyxWoW.Me.CurrentTarget.HealthPercent <= GeneralSettings.Trinket1EnemyHealth)
                return await UseTrinket1();

            if (GeneralSettings.Trinket1MyHealthBelow && Globals.MyHp <= GeneralSettings.Trinket1MyHealth)
                return await UseTrinket1();

            if (GeneralSettings.Trinket1OnBoS && (Me.HasAura("Breath of Sindragosa")/* || Me.Auras["Breath of Sindragosa"].IsActive*/))
                return await UseTrinket1();

            return false;
        }

        private static async Task<bool> UseTrinket1()
        {
            Item.Trinket1.Use();
            Logging.Write(Colors.SpringGreen, "Using {0}", Item.Trinket1.Name);
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }

        public static async Task<bool> Trinket2Method()
        {
            if (!Item.CanUseItem(Item.Trinket2))
                return false;

            if (GeneralSettings.Trinket2LossOfControl)
            {
                if (StyxWoW.Me.IsCrowdControlled())
                    await UseTrinket2();

                return false;
            }

            if (GeneralSettings.Trinket2OnCooldown)
                return await UseTrinket2();

            if (GeneralSettings.Trinket2OnBurst &&
                StyxWoW.Me.HasAura(SpellLists.BreathofSindragosa | SpellLists.AuraBreathofSindragosa | SpellLists.PillarofFrost | SpellLists.UnholyBlight |
                                   SpellLists.SummonGargoyle))
                return await UseTrinket2();

            if (StyxWoW.Me.GotTarget && GeneralSettings.Trinket1EnemyHealthBelow &&
                StyxWoW.Me.CurrentTarget.HealthPercent <= GeneralSettings.Trinket2EnemyHealth)
                return await UseTrinket2();

            if (GeneralSettings.Trinket2MyHealthBelow && Globals.MyHp <= GeneralSettings.Trinket2MyHealth)
                return await UseTrinket2();

            if (GeneralSettings.Trinket2OnBoS && (Me.HasAura("Breath of Sindragosa")/* || Me.Auras["Breath of Sindragosa"].IsActive)*/))
                return await UseTrinket2();

            return false;
        }

        private static async Task<bool> UseTrinket2()
        {
            Item.Trinket2.Use();
            Logging.Write(Colors.SpringGreen, "Using {0}", Item.Trinket2.Name);
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }
    }
}