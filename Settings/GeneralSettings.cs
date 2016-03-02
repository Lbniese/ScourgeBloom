/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.ComponentModel;
using System.IO;
using Styx.Helpers;
using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace ScourgeBloom.Settings
{
    public class GeneralSettings : Styx.Helpers.Settings
    {
        public static GeneralSettings Instance = new GeneralSettings();

        public GeneralSettings()
            : base(Path.Combine(Styx.Common.Utilities.AssemblyDirectory, @"Settings/ScourgeBloom/General.xml"))
        {
        }

        [Setting, DefaultValue(true), Category("Behaviour")]
        public bool Targeting { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        public bool Movement { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        public bool Facing { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        public bool Cooldowns { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        public bool Interrupts { get; set; }

        [Setting, DefaultValue(false), Category("Behaviour")]
        public bool AutoAttack { get; set; }

        [Setting, DefaultValue(false), Category("Environment")]
        [Browsable(true)]
        [ReadOnly(true)]
        public bool PvP { get; set; }

        [Setting, DefaultValue(true), Category("Resting")]
        [Browsable(true)]
        [ReadOnly(false)]
        public bool RestingEatFood { get; set; }

        #region Resting

        [Setting, DefaultValue(60), Category("Resting")]
        [Browsable(true)]
        [ReadOnly(false)]
        public int RestingEatFoodHp { get; set; }

        #endregion Resting

        [Setting, DefaultValue(false), Category("Environment")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Auto Release Spirit while dead")]
        public bool AutoReleaseSpirit { get; set; }

        [Setting, DefaultValue(true), Category("Behavior")]
        [Browsable(true)]
        [ReadOnly(false)]
        public bool PetUsage { get; set; }

        [Setting, DefaultValue(true), Category("Behavior")]
        [Browsable(true)]
        [ReadOnly(false)]
        public bool AoE { get; set; }

        [Setting, DefaultValue(true), Category("Behavior")]
        [Browsable(false)]
        [ReadOnly(true)]
        public bool UseRacials { get; set; }

        [Setting, DefaultValue(true), Category("Behavior")]
        [Browsable(false)]
        [ReadOnly(true)]
        public bool UseTrinket1 { get; set; }

        [Setting, DefaultValue(true), Category("Behavior")]
        [Browsable(false)]
        [ReadOnly(true)]
        public bool UseTrinket2 { get; set; }

        [Setting, DefaultValue(false), Category("Debugging")]
        [Browsable(false)]
        [ReadOnly(true)]
        public bool LogCanCastResults { get; set; }

        [Setting, DefaultValue(true), Category("Environment")]
        [Browsable(false)]
        [ReadOnly(true)]
        public bool AutoDismount { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool ArcaneTorrentUse { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool ArcaneTorrentOnlyInterruptHealing { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool EveryManForHimselfUse { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool GiftOfTheNaaruUse { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool GiftOfTheNaaruHealGroup { get; set; }

        [Setting, DefaultValue(60), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int GiftOfTheNaaruHealHp { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool StoneformUse { get; set; }

        [Setting, DefaultValue(60), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int StoneformUseHp { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool StoneformUseOnlyToClearDot { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool WarStompUse { get; set; }

        [Setting, DefaultValue(2), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int WarStompEnemiesToUse { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool BloodFuryUse { get; set; }

        [Setting, DefaultValue(true), Category("Racials")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool BerserkingUse { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1Use { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1LossOfControl { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1EnemyAdd { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1EnemyHealthBelow { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1MyHealthBelow { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1OnCooldown { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket1OnBurst { get; set; }

        [Setting, DefaultValue(35), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int Trinket1EnemyHealth { get; set; }

        [Setting, DefaultValue(35), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int Trinket1MyHealth { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2Use { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2LossOfControl { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2EnemyAdd { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2EnemyHealthBelow { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2MyHealthBelow { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2OnCooldown { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static bool Trinket2OnBurst { get; set; }

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int Trinket2EnemyHealth { get; set; }

        [Setting, DefaultValue(35), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(true)]
        public static int Trinket2MyHealth { get; set; }

        [Setting, DefaultValue(true), Category("Debugging")]
        public static bool DebugStopMoving { get; set; }

        [Setting, DefaultValue(true)]
        public bool ExtendedLogging { get; set; }

        [Setting, DefaultValue(true)]
        [Category("Targeting")]
        [DisplayName(@"Target World PVP Regardless")]
        [Description( "Solo Only. True: when attacked by player, ignore everything else and fight back;  False: attack based upon Targeting priority list.")]
        public static bool TargetWorldPvpRegardless { get; set; }

        #region Healing Tonic

        /* Added to the GUI
        [Setting, DefaultValue(true)]´*/
        public bool HealingTonicUse { get; set; }

        /* Added to the GUI
        [Setting, DefaultValue(50)]*/
        public int HealingTonicHp { get; set; }

        #endregion Healing Tonic

        #region Healthstones

        /* Added to the GUI
        [Setting, DefaultValue(true)]*/
        public bool HealthstoneUse { get; set; }

        /* Added to the GUI
        [Setting, DefaultValue(50)]*/
        public int HealthstoneHp { get; set; }

        #endregion

    }
}