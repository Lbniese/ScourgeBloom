/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Collections.Generic;
using JetBrains.Annotations;
using ScourgeBloom.Helpers;
using Styx.CommonBot;

namespace ScourgeBloom.Lists
{
    [UsedImplicitly]
    internal class SpellLists : ScourgeBloom
    {
        #region Class Spells

        public const int
            AntiMagicShell = 48707,
            ArmyoftheDead = 42650,
            DarkTransformation = 63560,
            DeathandDecay = 43265,
            DeathCoil = 47541,
            DeathStrike = 49998,
            DesecratedGround = 108201,
            EmpowerRuneWeapon = 47568,
            FrostStrike = 49143,
            HornofWinter = 57330,
            HowlingBlast = 49184,
            IceboundFortitude = 48792,
            Obliterate = 49020,
            Outbreak = 77575,
            PillarofFrost = 51271,
            RaiseDead = 46584,
            RemorselessWinter = 108200,
            ScourgeStrike = 55090,
            SummonGargoyle = 49206,
            BreathofSindragosa = 152279,
            Defile = 152280,
            FesteringStrike = 85948,
            MindFreeze = 47528,
            RaiseAlly = 61999,
            PathofFrost = 3714,
            DeathGate = 50977,
            DarkCommand = 56222,
            ControlUndead = 111673,
            ChainsofIce = 45524,
            DeathGrip = 49576,
            ChainsOfIce = 45524,

            //New Spells
            SindragosasFury = 190778,
            BlightedRuneWeapon = 194918,

            //Unholy Talents --
            DarkArbiter = 207349,
            ClawingShadows = 207311,
            Epidemic = 207317,
            SoulReaper = 130736,

            //Frost Talents --
            HungeringRuneWeapon = 207127,
            GlacialAdvance = 194913,
            Frostscythe = 207230,
            Obliteration = 207256;

        #endregion Class Spells

        #region Racials

        public const int
            // Human
            EveryManForHimself = 59752,
            // Draenei
            GiftOfTheNaaru = 28880,
            // Dwarf
            StoneForm = 20594,
            // Blood Elf
            ArcaneTorrent = 155145,
            // Tauren
            WarStomp = 20549,
            // Troll
            Berserking = 26297,
            // Orc
            BloodFury = 20572;

        #endregion Racials

        #region Class Auras

        public const int
            AuraBreathofSindragosa = 155166,
            AuraSuddenDoom = 81340,
            AuraBloodCharge = 114851,
            AuraShadowInfusion = 91342,
            AuraRime = 59057,
            AuraFreezingFog = 59052,
            AuraCrimsonScourge = 81136,
            AuraKillingMachine = 51124,

            // New Auras
            AuraVirulentPlague = 191587,
            AuraFesteringWound = 194310,
            AuraDeathandDecay = 188290,
            AuraDefile = 188290,
            AuraSoulReaper = 130736,
            AuraNecrosis = 216974,
            AuraUnholyStrength = 53365,
            AuraSummonGargoyle = 61777,
            AuraObliterationProc = 187893,
            AuraObliterationTalent = 207256;

        #endregion Auras

        #region ChannedInteruptableSpells

        public static readonly HashSet<int> ChanneledInteruptableSpells = new HashSet<int>
        {
            5143, // Arcane Missiles, //
            42650, // Army of the Dead, //
            10, // Blizzard, //
            64843, // Divine Hymn, //
            689, // Drain Life, //
            89420, // Drain Life, //
            1120, // Drain Soul, //
            755, // Health Funnel, //
            1949, // Hellfire, //
            85403, // Hellfire, //
            16914, // Hurricane, //
            64901, // Hymn of Hope, //
            50589, // Immolation Aura, //
            15407, // Mind Flay, //
            47540, // Penance, //
            5740, // Rain of Fire, //
            740, // Tranquility, //
            103103 // Malefic Grasp //
        };

        #endregion ChannedInteruptableSpells

        #region SpellDump

        public static void SpellDump()
        {
            foreach (var spell in SpellManager.Spells)
            {
                Log.WriteLog(string.Format("{0} = {1},", spell.Value.Name, spell.Value.Id));
            }
        }

        #endregion SpellDump
    }
}