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
            Asphyxiate = 108194,
            BloodBoil = 50842,
            BloodTap = 45529,
            BoneShield = 49222,
            Conversion = 119975,
            DancingRuneWeapon = 49028,
            DarkTransformation = 63560,
            DeathandDecay = 43265,
            DeathCoil = 47541,
            DeathPact = 48743,
            DeathSiphon = 108196,
            DeathStrike = 49998,
            DesecratedGround = 108201,
            EmpowerRuneWeapon = 47568,
            FrostStrike = 49143,
            HeartStrike = 55050,
            HornofWinter = 57330,
            HowlingBlast = 49184,
            IceboundFortitude = 48792,
            IcyTouch = 45477,
            Lichborne = 49039,
            NecroticStrike = 73975,
            Obliterate = 49020,
            Outbreak = 77575,
            Pestilence = 50842,
            PillarofFrost = 51271,
            PlagueLeech = 123693,
            PlagueStrike = 45462,
            RaiseDead = 46584,
            RemorselessWinter = 108200,
            RuneStrike = 56815,
            RuneTap = 48982,
            ScourgeStrike = 55090,
            SummonGargoyle = 49206,
            UnholyBlight = 115989,
            VampiricBlood = 55233,
            BreathofSindragosa = 152279,
            Defile = 152280,
            FesteringStrike = 85948,
            UnholyPresence = 48265,
            MindFreeze = 47528,
            Strangulate = 47476,
            DeathsAdvance = 96268,
            FrostPresence = 48266,
            BloodPresence = 48263,
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
            AuraNecroticPlague = 155159,
            AuraBloodPlague = 55078,
            AuraFrostFever = 55095,
            AuraBreathofSindragosa = 155166,
            AuraSuddenDoom = 81340,
            AuraBloodCharge = 114851,
            AuraShadowInfusion = 91342,
            AuraRime = 59057,
            AuraFreezingFog = 59052,
            AuraCrimsonScourge = 81136,
            AuraDancingRuneWeapon = 49028,  //Is that one correct?
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
            AuraObliterationProc = 187893;
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
