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
using Buddy.Coroutines;
using JetBrains.Annotations;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using S = ScourgeBloom.Lists.SpellLists;

namespace ScourgeBloom.Class
{
    [UsedImplicitly]
    public class Racials : ScourgeBloom
    {
        public static async Task<bool> RacialsMethod()
        {
            if (!Capabilities.IsRacialUsageAllowed)
                return false;

            switch (StyxWoW.Me.Race)
            {
                case WoWRace.Human:
                    return await Human();

                case WoWRace.Draenei:
                    return await Draenei();

                case WoWRace.Dwarf:
                    return await Dwarf();

                case WoWRace.BloodElf:
                    return await BloodElf();

                case WoWRace.Tauren:
                    return await Tauren();

                case WoWRace.Orc:
                    return await Orc();

                case WoWRace.Troll:
                    return await Troll();
            }

            return false;
        }

        #region Human

        private static async Task<bool> Human()
        {
            if (!GeneralSettings.EveryManForHimselfUse)
                return false;

            if (!StyxWoW.Me.HasAuraWithMechanic(
                WoWSpellMechanic.Dazed,
                WoWSpellMechanic.Disoriented,
                WoWSpellMechanic.Frozen,
                WoWSpellMechanic.Incapacitated,
                WoWSpellMechanic.Rooted,
                WoWSpellMechanic.Slowed,
                WoWSpellMechanic.Snared))
                return false;

            if (!SpellManager.CanCast(S.EveryManForHimself))
                return false;

            if (!await Spell.CoCast(S.EveryManForHimself))
                return false;

            Logging.Write(Colors.SpringGreen, "Tried to use Every Man for Himself");

            return true;
        }

        #endregion Human

        #region Draenei

        private static async Task<bool> Draenei()
        {
            if (!GeneralSettings.GiftOfTheNaaruUse)
                return false;

            if (StyxWoW.Me.HealthPercent <= GeneralSettings.GiftOfTheNaaruHealHp)
            {
                if (!SpellManager.CanCast(S.GiftOfTheNaaru))
                    return false;

                if (!await Spell.CoCast(S.GiftOfTheNaaru))
                    return false;

                Logging.Write(Colors.SpringGreen, "Used Gift of the Naaru on ourself at {0}%", StyxWoW.Me.HealthPercent);
            }
            return true;
        }

        #endregion Draenei

        #region Dwarf

        private static async Task<bool> Dwarf()
        {
            if (!GeneralSettings.StoneformUse)
                return false;

            if (!GeneralSettings.StoneformUseOnlyToClearDot &&
                StyxWoW.Me.HealthPercent <= GeneralSettings.StoneformUseHp)
            {
                if (!SpellManager.CanCast(S.StoneForm))
                    return false;

                if (!await Spell.CoCast(S.StoneForm))
                    return false;

                Logging.Write(Colors.SpringGreen, "Used Stoneform at {0}", StyxWoW.Me.HealthPercent);

                return true;
            }

            if (!GeneralSettings.StoneformUseOnlyToClearDot)
                return false;

            // If I don't have a bleed effect, poison, magic, curse or disease on me return false
            if (!StyxWoW.Me.HasAuraWithMechanic(WoWSpellMechanic.Bleeding) ||
                !StyxWoW.Me.Debuffs.Values.Any(u => u.Spell.DispelType == WoWDispelType.Poison ||
                                                    u.Spell.DispelType == WoWDispelType.Magic ||
                                                    u.Spell.DispelType == WoWDispelType.Curse ||
                                                    u.Spell.DispelType == WoWDispelType.Disease))
                return false;

            if (!SpellManager.CanCast(S.StoneForm))
                return false;

            if (!await Spell.CoCast(S.StoneForm))
                return false;

            Logging.Write(Colors.SpringGreen, "Used Stoneform to remove negative effect");

            return true;
        }

        #endregion Dwarf

        #region Blood Elf

        private static async Task<bool> BloodElf()
        {
            if (!GeneralSettings.ArcaneTorrentUse)
                return false;

            if (!StyxWoW.Me.GotTarget)
                return false;

            if (StyxWoW.Me.CurrentTarget.IsPet)
                return false;

            if (!StyxWoW.Me.CurrentTarget.IsCasting)
                return false;

            if (!StyxWoW.Me.CurrentTarget.CanInterruptCurrentSpellCast)
                return false;

            var rnd = new Random();
            var delay = rnd.Next(50, 500);

            await Coroutine.Sleep(delay);

            if (StyxWoW.Me.CurrentTarget.Distance > 8)
                return false;

            if (!SpellManager.CanCast(S.ArcaneTorrent))
                return false;

            if (!await Spell.CoCast(S.ArcaneTorrent))
                return false;

            Logging.Write(Colors.SpringGreen, "Used Arcane Torrent on {0}", StyxWoW.Me.CurrentTarget.SafeName);

            return true;
        }

        #endregion Blood Elf

        #region Tauren

        private static async Task<bool> Tauren()
        {
            if (!GeneralSettings.WarStompUse)
                return false;

            if (Units.EnemyUnitsSub8.Count() < GeneralSettings.WarStompEnemiesToUse)
                return false;

            if (!SpellManager.CanCast(S.WarStomp))
                return false;

            if (!await Spell.CoCast(S.WarStomp))
                return false;

            Logging.Write(Colors.SpringGreen, "Used War Stomp");

            return true;
        }

        #endregion Tauren

        #region Orc

        private static async Task<bool> Orc()
        {
            if (!GeneralSettings.BloodFuryUse)
                return false;

            if (!SpellManager.CanCast(S.BloodFury))
                return false;

            if (!await Spell.CoCast(S.BloodFury))
                return false;

            Logging.Write(Colors.SpringGreen, "Used Blood Fury");

            return true;
        }

        #endregion

        #region Troll

        private static async Task<bool> Troll()
        {
            if (!GeneralSettings.BerserkingUse)
                return false;

            if (!SpellManager.CanCast(S.Berserking))
                return false;

            if (!await Spell.CoCast(S.Berserking))
                return false;

            Logging.Write(Colors.SpringGreen, "Used Berserking");

            return true;
        }

        #endregion
    }
}