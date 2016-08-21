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
using System.Windows.Media;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    internal class Item : ScourgeBloom
    {
        public static WoWItem Trinket1
        {
            get { return StyxWoW.Me.Inventory.GetItemBySlot(12); }
        }

        public static WoWItem Trinket2
        {
            get { return StyxWoW.Me.Inventory.GetItemBySlot(13); }
        }

        public static void UseContainerItem(string name)
        {
            var item = StyxWoW.Me.BagItems.FirstOrDefault(x => x.Name == name && x.Usable && x.Cooldown <= 0);
            if (item != null && item.CooldownTimeLeft.Equals(TimeSpan.Zero))
            {
                item.UseContainerItem();
            }
        }

        public static void UseContainerItem(string name, Func<bool> req)
        {
            if (req()) UseContainerItem(name);
        }

        public static void UseContainerItem(int id)
        {
            var item = StyxWoW.Me.BagItems.FirstOrDefault(x => x.Entry == id && x.Usable && x.Cooldown <= 0);
            if (item != null && item.CooldownTimeLeft.Equals(TimeSpan.Zero))
            {
                item.UseContainerItem();
            }
        }

        public static void UseContainerItem(int id, Func<bool> req)
        {
            if (req()) UseContainerItem(id);
        }

        public static bool CanUseItem(WoWItem item)
        {
            if (item == null)
                return false;

            // Check if we can even use the item
            //if (item.Effects.Any(u => u.TriggerType != ItemEffectTriggerType.OnUse))
            //    return false;

            //var itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            //
            //if (string.IsNullOrEmpty(itemSpell))
            //    return false;
            //

            return item.Usable && item.Cooldown <= 0 && !MerchantFrame.Instance.IsVisible;
        }

        public static async Task<bool> Healthstone()
        {
            if (!GeneralSettings.Instance.HealthstoneUse)
                return false;

            if (StyxWoW.Me.HealthPercent > GeneralSettings.Instance.HealthstoneHp)
                return false;

            var healthstone = FindItem(5512);

            if (!CanUseItem(healthstone))
                return false;

            healthstone.UseContainerItem();
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Using Healthstone");
            return true;
        }

        public static async Task<bool> HealingTonic()
        {
            if (!GeneralSettings.Instance.HealingTonicUse)
                return false;

            if (StyxWoW.Me.HealthPercent > GeneralSettings.Instance.HealingTonicHp)
                return false;

            var healingtonic = FindItem(109223);

            if (!CanUseItem(healingtonic))
                return false;

            healingtonic.UseContainerItem();
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            Logging.Write(Colors.YellowGreen, "[ScourgeBloom] Using Healing Tonic");
            return true;
        }

        public static WoWItem FindItem(int id)
        {
            return StyxWoW.Me.CarriedItems.FirstOrDefault(u => u.Entry == id);
        }
    }
}