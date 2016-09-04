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
    public class DeathKnightSettings : Styx.Helpers.Settings
    {
        public static DeathKnightSettings Instance = new DeathKnightSettings();

        public string SavePath = Path.Combine(Styx.Common.Utilities.AssemblyDirectory,
            $@"Routines/ScourgeBloom/Settings/{StyxWoW.Me.RealmName}/{StyxWoW.Me.Name}_ScourgeBloom_DeathKnightSettings.xml");


        public DeathKnightSettings()
            : base(Path.Combine(Styx.Common.Utilities.AssemblyDirectory,
                $@"Routines/ScourgeBloom/Settings/{StyxWoW.Me.RealmName}/{StyxWoW.Me.Name}_ScourgeBloom_DeathKnightSettings.xml")
                )
        {
        }

        [Setting, DefaultValue(false), Category("Behaviour")]
        [Browsable(false)]
        public bool DeathGrip { get; set; }

        [Setting, DefaultValue(true)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("Allow automatically using Death Strike")]
        public bool UseDeathStrike { get; set; }

        [Setting, DefaultValue(50)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("The health percentage we use Death Strike at")]
        public int UseDeathStrikeHp { get; set; }

        [Setting, DefaultValue(true)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("Allow automatically using Death Siphon")]
        public bool UseDeathSiphon { get; set; }

        [Setting, DefaultValue(50)]
        [Category("Combat")]
        [Browsable(false)]
        [Description("The health percentage we use Death Siphon at")]
        public int UseDeathSiphonHp { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("The health percentage we use Icebound Fortitude at")]
        public bool UseIceBoundFortitude { get; set; }

        [Setting, DefaultValue(50)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("The health percentage we use Icebound Fortitude at")]
        public int UseIceBoundFortitudeHp { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Unholy")]
        [Browsable(false)]
        [Description("Summon Gargoyle on cooldown")]
        public bool SummonGargoyleOnCd { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        public bool UseAms { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Unholy")]
        [Browsable(false)]
        [Description("Pillar of Frost on cooldown")]
        public bool PillarofFrostOnCd { get; set; }

        [Setting, DefaultValue(true)]
        [ReadOnly(true)]
        [Category("Common")]
        [Browsable(false)]
        [Description("Use Army of the Dead")]
        public bool UseAotD { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool BosArcaneTorrent { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool BosBloodFury { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool BosBerserking { get; set; }

        #region Mind Freeze

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool MindFreezeUse { get; set; }

        [Setting, DefaultValue(true)]
        [Browsable(false)]
        public bool MindFreezeRandomTimerUse { get; set; }

        [Setting, DefaultValue(40)]
        [Browsable(false)]
        public double MindFreezeRandomTimerMin { get; set; }

        [Setting, DefaultValue(60)]
        [Browsable(false)]
        public double MindFreezeRandomTimerMax { get; set; }

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