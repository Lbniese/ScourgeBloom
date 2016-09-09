/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.ComponentModel;
using System.IO;
using Styx;
using Styx.Helpers;
using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace ScourgeBloom.Settings
{
    public class GeneralSettings : Styx.Helpers.Settings
    {
        public static GeneralSettings Instance = new GeneralSettings();

        public string SavePath = Path.Combine(Styx.Common.Utilities.AssemblyDirectory,
            $@"Routines/ScourgeBloom/Settings/{StyxWoW.Me.RealmName}/{StyxWoW.Me.Name}_ScourgeBloom_GeneralSettings.xml");

        public GeneralSettings()
            : base(
                Path.Combine(Styx.Common.Utilities.AssemblyDirectory,
                    $@"Routines/ScourgeBloom/Settings/{StyxWoW.Me.RealmName}/{StyxWoW.Me.Name}_ScourgeBloom_GeneralSettings.xml")
                )
        {
        }

        [Setting, Styx.Helpers.DefaultValue(false), Category("Behaviour")]
        [Browsable(true)]
        public bool Targeting { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        [Browsable(true)]
        public bool Movement { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        [Browsable(true)]
        public bool Facing { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        [Browsable(true)]
        public bool Cooldowns { get; set; }

        [Setting, DefaultValue(true), Category("Behaviour")]
        [Browsable(true)]
        public bool Interrupts { get; set; }

        [Setting, DefaultValue(false), Category("Behaviour")]
        [Browsable(true)]
        public bool AutoAttack { get; set; }

        [Setting, DefaultValue(false), Category("Environment")]
        [Browsable(true)]
        [ReadOnly(true)]
        public bool PvP { get; set; }

        [Setting, DefaultValue(false), Category("Environment")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Auto Release Spirit while dead")]
        public bool AutoReleaseSpirit { get; set; }

        [Setting, DefaultValue(false), Category("Pet")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Auto Control Pet Taunt")]
        public bool PetAutoControlTaunt { get; set; }

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

        [Setting, DefaultValue(true), Category("Debugging")]
        [Browsable(true)]
        [ReadOnly(false)]
        public bool LogCanCastResults { get; set; }

        [Setting, DefaultValue(true), Category("Environment")]
        [Browsable(true)]
        [ReadOnly(false)]
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
        public static bool Trinket1OnBoS { get; set; }

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

        [Setting, DefaultValue(true), Category("Trinkets")]
        [Browsable(false)]
        [ReadOnly(false)]
        public static bool Trinket2OnBoS { get; set; }

        [Setting, DefaultValue(false), Category("Debugging")]
        [Browsable(false)]
        public static bool DebugStopMoving { get; set; }

        [Setting, DefaultValue(true), Category("Debugging")]
        [Browsable(true)]
        public bool ExtendedLogging { get; set; }

        [Setting, DefaultValue(true), Category("Debugging")]
        [Browsable(true)]
        public bool Debug { get; set; }

        [Setting, ReadOnly(false)]
        [DefaultValue(0)]
        [Category("General")]
        [DisplayName("Disable Spells with Cooldown (secs)")]
        [Description(
            "Prevent the CR from casting any spell with this cooldown or greater; set to 0 to allow the CR to cast all spells"
            )]
        public int DisableSpellsWithCooldown { get; set; }

        [Setting, ReadOnly(false)]
        [DefaultValue(true)]
        [Category("Movement")]
        [DisplayName("Use Cast While Moving Buffs")]
        [Description(
            "True: attempting to use a non-instant while moving will first cast Spiritwalker's Grace, Ice Floes, Kil'Jaedan's Cunning, etc."
            )]
        public bool UseCastWhileMovingBuffs { get; set; }

        [Setting, DefaultValue(true)]
        [Category("Targeting")]
        [DisplayName(@"Target World PVP Regardless")]
        [Description(
            "Solo Only. false: when attacked by player, ignore everything else and fight back;  False: attack based upon Targeting priority list."
            )]
        public static bool TargetWorldPvpRegardless { get; set; }

        [Setting, DefaultValue(false), Category("Behaviour")]
        [Browsable(false)]
        public static bool DeathGrip { get; set; }

        [Setting, DefaultValue(true)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("Allow automatically using Death Strike")]
        public static bool UseDeathStrike { get; set; }

        [Setting, DefaultValue(50)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("The health percentage we use Death Strike at")]
        public static int UseDeathStrikeHp { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("The health percentage we use Icebound Fortitude at")]
        public static bool UseIceBoundFortitude { get; set; }

        [Setting, DefaultValue(50)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("The health percentage we use Icebound Fortitude at")]
        public static int UseIceBoundFortitudeHp { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Unholy")]
        [Browsable(false)]
        [Description("Summon Gargoyle on cooldown")]
        public static bool SummonGargoyleOnCd { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        public static bool UseAms { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Unholy")]
        [Browsable(false)]
        [Description("Pillar of Frost on cooldown")]
        public static bool PillarofFrostOnCd { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("Use Army of the Dead")]
        public static bool UseAotD { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public static bool BosArcaneTorrent { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public static bool BosBloodFury { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public static bool BosBerserking { get; set; }

        #region Resting

        [Setting, DefaultValue(true), Category("Resting")]
        [Browsable(true)]
        [ReadOnly(false)]
        public bool RestingEatFood { get; set; }

        [Setting, DefaultValue(60), Category("Resting")]
        [Browsable(true)]
        [ReadOnly(false)]
        public int RestingEatFoodHp { get; set; }

        #endregion Resting

        #region Healing Tonic

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool HealingTonicUse { get; set; }

        [Setting, DefaultValue(50)]
        [Browsable(false)]
        public int HealingTonicHp { get; set; }

        #endregion Healing Tonic

        #region Healthstones

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool HealthstoneUse { get; set; }

        [Setting, DefaultValue(50)]
        [Browsable(false)]
        public int HealthstoneHp { get; set; }

        #endregion

        #region Hotkeys

        [Setting, DefaultValue(1)] // default Alt = 1
        public int HotkeyPauseModifier { get; set; }

        [Setting, DefaultValue("P")]
        public string HotkeyPauseKey { get; set; }

        #endregion Hotkeys

        #region Mind Freeze

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public static bool MindFreezeUse { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public static bool MindFreezeRandomTimerUse { get; set; }

        [Setting, DefaultValue(40)]
        [Browsable(false)]
        public static double MindFreezeRandomTimerMin { get; set; }

        [Setting, DefaultValue(60)]
        [Browsable(false)]
        public static double MindFreezeRandomTimerMax { get; set; }

        #endregion Mind Freeze

        #region Strangulate

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool StrangulateUse { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool StrangulateRandomTimerUse { get; set; }

        [Setting, DefaultValue(40)]
        [Browsable(false)]
        public double StrangulateRandomTimerMin { get; set; }

        [Setting, DefaultValue(60)]
        [Browsable(false)]
        public double StrangulateRandomTimerMax { get; set; }

        #endregion Strangulate
    }
}