/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Settings;
using Styx;

namespace ScourgeBloom
{
    public partial class ScourgeBloomSettings : Form
    {
        public ScourgeBloomSettings()
        {
            InitializeComponent();
        }

        private void On_Load(object sender, EventArgs e)
        {
            ClassSettings.Initialize();
            //ClassSGrid.SelectedObject = ClassSettings.Settings;
            GeneralSGrid.SelectedObject = GeneralSettings.Instance;
            RacialsLoad();
            Trinket1Load();
            Trinket2Load();
            InterruptsLoad();
            DeathGripLoad();
            SurvivabilityLoad();
            ItemsLoad();

            // Changelog
            textBox1.Multiline = true;
            var webClient = new WebClient();
            var textboxValue =
                webClient.DownloadString(
                    "https://botpipe.com:2087/product/6b041b2c-fc26-11e5-84d4-11866f97a98b/changelog.txt");
            textBox1.Text = textboxValue;
        }

        private void On_Exit(object sender, EventArgs e)
        {
            Log.WriteLog("Saving Settings");
            ClassSettings.Settings.Save();
            ClassSettings.Initialize();
            GeneralSettings.Instance.Save();
            Close();
        }

        private void ScourgeBloomSettings_Load(object sender, EventArgs e)
        {
            On_Load(sender, e);
        }

        private void InterruptsLoad()
        {
            //Mind Freeze
            UseMFcheckBox.Checked = DeathKnightSettings.Instance.MindFreezeUse;
            DelayMFcheckBox.Checked = DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
            numMFmax.Value = (decimal) DeathKnightSettings.Instance.MindFreezeRandomTimerMax;
            numMFmin.Value = (decimal) DeathKnightSettings.Instance.MindFreezeRandomTimerMin;

            DelayMFcheckBox.Enabled = DeathKnightSettings.Instance.MindFreezeUse;
            numMFmax.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
            numMFmin.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;

            //Strangulate
            StrangulateCheckBox.Checked = DeathKnightSettings.Instance.StrangulateUse;
            DelayStrangulateCheckBox.Checked = DeathKnightSettings.Instance.StrangulateRandomTimerUse;
            numStrangulatemax.Value = (decimal) DeathKnightSettings.Instance.StrangulateRandomTimerMax;
            numStrangulatemin.Value = (decimal) DeathKnightSettings.Instance.StrangulateRandomTimerMin;

            DelayStrangulateCheckBox.Enabled = DeathKnightSettings.Instance.StrangulateUse;
            numStrangulatemax.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
            numStrangulatemin.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
        }

        private void RacialsLoad()
        {
            UseRacialsCheckBox.Checked = GeneralSettings.Instance.UseRacials;

            //Human
            UseHumanRacialCheckBox.Checked = GeneralSettings.EveryManForHimselfUse;

            //Dwarf
            UseStoneformCheckBox.Checked = GeneralSettings.StoneformUse;
            UseStoneformOnlyToClearCheckBox.Checked = GeneralSettings.StoneformUseOnlyToClearDot;
            numStoneformHp.Value = GeneralSettings.StoneformUseHp;
            UseStoneformOnlyToClearCheckBox.Enabled = GeneralSettings.StoneformUse;
            numStoneformHp.Enabled = GeneralSettings.StoneformUse;

            //Draenei
            UseGiftoftheNaruuCheckBox.Checked = GeneralSettings.GiftOfTheNaaruUse;
            numGiftOfTheNaaruHp.Value = GeneralSettings.GiftOfTheNaaruHealHp;
            numGiftOfTheNaaruHp.Enabled = GeneralSettings.GiftOfTheNaaruUse;

            //Blood Elf
            UseArcaneTorrentCheckBox.Checked = GeneralSettings.ArcaneTorrentUse;

            //Tauren
            UseWarStompCheckBox.Checked = GeneralSettings.WarStompUse;
            numWarStompEnemies.Value = GeneralSettings.WarStompEnemiesToUse;
            numWarStompEnemies.Enabled = GeneralSettings.WarStompUse;
        }

        private void Trinket1Load()
        {
            UseTrinket1CheckBox.Checked = GeneralSettings.Trinket1Use;
            T1OnLoCCheckBox.Checked = GeneralSettings.Trinket1LossOfControl;
            T1OnEnemyHealthBelowCheckBox.Checked = GeneralSettings.Trinket1EnemyHealthBelow;
            T1OnMyHealthBelowCheckBox.Checked = GeneralSettings.Trinket1MyHealthBelow;
            T1OnCdCheckBox.Checked = GeneralSettings.Trinket1OnCooldown;
            T1OnBurstCheckBox.Checked = GeneralSettings.Trinket1OnBurst;
            numTrinket1EnemyHealth.Value = GeneralSettings.Trinket1EnemyHealth;
            numTrinket1MyHealth.Value = GeneralSettings.Trinket1MyHealth;
            UseTrinket1OnBoS.Checked = GeneralSettings.Trinket1OnBoS;

            T1OnEnemyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnMyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnCdCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnBurstCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            numTrinket1EnemyHealth.Enabled = !GeneralSettings.Trinket1LossOfControl;
            numTrinket1MyHealth.Enabled = !GeneralSettings.Trinket1LossOfControl;
            UseTrinket1OnBoS.Enabled = !GeneralSettings.Trinket1LossOfControl;
        }

        private void Trinket2Load()
        {
            UseTrinket2CheckBox.Checked = GeneralSettings.Trinket2Use;
            T2OnLoCCheckBox.Checked = GeneralSettings.Trinket2LossOfControl;
            T2OnEnemyHealthBelowCheckBox.Checked = GeneralSettings.Trinket2EnemyHealthBelow;
            T2OnMyHealthBelowCheckBox.Checked = GeneralSettings.Trinket2MyHealthBelow;
            T2OnCdCheckBox.Checked = GeneralSettings.Trinket2OnCooldown;
            T2OnBurstCheckBox.Checked = GeneralSettings.Trinket2OnBurst;
            numTrinket2EnemyHealth.Value = GeneralSettings.Trinket2EnemyHealth;
            numTrinket2MyHealth.Value = GeneralSettings.Trinket2MyHealth;
            UseTrinket2OnBoS.Checked = GeneralSettings.Trinket2OnBoS;

            T2OnEnemyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnMyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnCdCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnBurstCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            numTrinket2EnemyHealth.Enabled = !GeneralSettings.Trinket2LossOfControl;
            numTrinket2MyHealth.Enabled = !GeneralSettings.Trinket2LossOfControl;
            UseTrinket2OnBoS.Enabled = !GeneralSettings.Trinket2LossOfControl;
        }

        private void DeathGripLoad()
        {
            UseDeathGripCheckBox.Checked = DeathKnightSettings.Instance.DeathGrip;
        }

        private void SurvivabilityLoad()
        {
            DeathStrikecheckBox.Checked = DeathKnightSettings.Instance.UseDeathStrike;
            numDeathStrikeHp.Value = DeathKnightSettings.Instance.UseDeathStrikeHp;

            DeathSiphoncheckBox.Checked = DeathKnightSettings.Instance.UseDeathSiphon;
            numDeathSiphonHp.Value = DeathKnightSettings.Instance.UseDeathSiphonHp;
        }

        private void ItemsLoad()
        {
            useHealingToniccheckBox.Checked = GeneralSettings.Instance.HealingTonicUse;
            numHealingTonicHp.Value = GeneralSettings.Instance.HealingTonicHp;

            UseHealthstonecheckBox.Checked = GeneralSettings.Instance.HealthstoneUse;
            numHealthstoneUseHp.Value = GeneralSettings.Instance.HealthstoneHp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SpellLists.SpellDump();
        }

        private void Savebutton_Click(object sender, EventArgs e)
        {
            On_Exit(sender, e);
        }

        private void AFKPresetButton_Click(object sender, EventArgs e)
        {
            GeneralSettings.Instance.AutoDismount = true;
            GeneralSettings.Instance.Movement = true;
            GeneralSettings.Instance.AutoReleaseSpirit = true;
            GeneralSettings.Instance.AutoAttack = true;
            GeneralSettings.Instance.Targeting = true;
            DeathKnightSettings.Instance.DeathGrip = true;
            GeneralSettings.Instance.Interrupts = true;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            DeathKnightSettings.Instance.UseDeathStrike = true;
            DeathKnightSettings.Instance.UseDeathStrikeHp = 60;
            DeathKnightSettings.Instance.UseDeathSiphon = true;
            DeathKnightSettings.Instance.UseDeathSiphonHp = 60;
            GeneralSettings.Instance.UseTrinket1 = true;
            GeneralSettings.Trinket1OnCooldown = true;
            GeneralSettings.Instance.UseTrinket2 = true;
            GeneralSettings.Trinket2OnCooldown = true;
            On_Exit(sender, e);
        }

        private void RotationOnlyPresetButton_Click(object sender, EventArgs e)
        {
            GeneralSettings.Instance.AutoDismount = true;
            GeneralSettings.Instance.Movement = false;
            GeneralSettings.Instance.AutoReleaseSpirit = false;
            GeneralSettings.Instance.AutoAttack = false;
            GeneralSettings.Instance.Targeting = false;
            DeathKnightSettings.Instance.DeathGrip = false;
            GeneralSettings.Instance.Interrupts = false;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            DeathKnightSettings.Instance.UseDeathStrike = false;
            DeathKnightSettings.Instance.UseDeathSiphon = false;
            On_Exit(sender, e);
        }

        private void DungeonBuddyPresetButton_Click(object sender, EventArgs e)
        {
            GeneralSettings.Instance.AutoDismount = true;
            GeneralSettings.Instance.Movement = true;
            GeneralSettings.Instance.AutoReleaseSpirit = true;
            GeneralSettings.Instance.AutoAttack = false;
            GeneralSettings.Instance.Targeting = true;
            GeneralSettings.Instance.Facing = true;
            DeathKnightSettings.Instance.DeathGrip = false;
            GeneralSettings.Instance.Interrupts = true;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            DeathKnightSettings.Instance.UseDeathStrike = true;
            DeathKnightSettings.Instance.UseDeathStrikeHp = 15;
            DeathKnightSettings.Instance.UseDeathSiphon = true;
            DeathKnightSettings.Instance.UseDeathSiphonHp = 20;
            On_Exit(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void UseHumanRacialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.EveryManForHimselfUse = UseHumanRacialCheckBox.Checked;
        }

        private void UseGiftoftheNaruuCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.GiftOfTheNaaruUse = UseGiftoftheNaruuCheckBox.Checked;

            GeneralSettings.GiftOfTheNaaruUse = numGiftOfTheNaaruHp.Enabled;
        }

        private void numGiftOfTheNaaruHp_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.GiftOfTheNaaruHealHp = (int) numGiftOfTheNaaruHp.Value;
        }

        private void UseArcaneTorrentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.ArcaneTorrentUse = UseArcaneTorrentCheckBox.Checked;
        }

        private void UseStoneformCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.StoneformUse = UseStoneformCheckBox.Checked;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.StoneformUseHp = (int) numStoneformHp.Value;
        }

        private void numWarStompEnemies_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.WarStompEnemiesToUse = (int) numWarStompEnemies.Value;
        }

        private void UseWarStompCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.WarStompUse = UseWarStompCheckBox.Checked;
        }

        private void GargoyleOnCDcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.SummonGargoyleOnCd = GargoyleOnCDcheckBox.Checked;
        }

        private void BosArcaneTorrentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.BosArcaneTorrent = BosArcaneTorrentCheckBox.Checked;
        }

        private void T1OnCdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1OnCooldown = T1OnCdCheckBox.Checked;
        }

        private void T1OnLoCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1LossOfControl = T1OnLoCCheckBox.Checked;

            T1OnEnemyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnMyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnCdCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
            numTrinket1EnemyHealth.Enabled = !GeneralSettings.Trinket1LossOfControl;
            numTrinket1MyHealth.Enabled = !GeneralSettings.Trinket1LossOfControl;
            T1OnBurstCheckBox.Enabled = !GeneralSettings.Trinket1LossOfControl;
        }

        private void T1OnBurstCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1OnBurst = T1OnBurstCheckBox.Checked;
        }

        private void T1OnMyHealthBelowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1MyHealthBelow = T1OnMyHealthBelowCheckBox.Checked;
            numTrinket1MyHealth.Enabled = GeneralSettings.Trinket1MyHealthBelow;
        }

        private void T1OnEnemyHealthBelowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1EnemyHealthBelow = T1OnEnemyHealthBelowCheckBox.Checked;
            numTrinket1EnemyHealth.Enabled = GeneralSettings.Trinket1EnemyHealthBelow;
        }

        private void numTrinket1MyHealth_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1MyHealth = (int) numTrinket1MyHealth.Value;
        }

        private void numTrinket1EnemyHealth_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1EnemyHealth = (int) numTrinket1EnemyHealth.Value;
        }

        private void numTrinket2EnemyHealth_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2EnemyHealth = (int) numTrinket2EnemyHealth.Value;
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {
        }

        private void UseTrinket1CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1Use = UseTrinket1CheckBox.Checked;

            if (UseTrinket1CheckBox.Checked)
            {
                T1OnLoCCheckBox.Enabled = true;
                T1OnCdCheckBox.Enabled = true;
                T1OnBurstCheckBox.Enabled = true;
                T1OnMyHealthBelowCheckBox.Enabled = true;
                numTrinket1MyHealth.Enabled = true;
                T1OnEnemyHealthBelowCheckBox.Enabled = true;
                numTrinket1EnemyHealth.Enabled = true;
                UseTrinket1OnBoS.Enabled = true;
            }

            if (UseTrinket1CheckBox.Checked == false)
            {
                T1OnLoCCheckBox.Enabled = false;
                T1OnCdCheckBox.Enabled = false;
                T1OnBurstCheckBox.Enabled = false;
                T1OnMyHealthBelowCheckBox.Enabled = false;
                numTrinket1MyHealth.Enabled = false;
                T1OnEnemyHealthBelowCheckBox.Enabled = false;
                numTrinket1EnemyHealth.Enabled = false;
                UseTrinket1OnBoS.Enabled = false;
            }
        }

        private void UseTrinket2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2Use = UseTrinket2CheckBox.Checked;

            if (UseTrinket2CheckBox.Checked)
            {
                T2OnLoCCheckBox.Enabled = true;
                T2OnCdCheckBox.Enabled = true;
                T2OnBurstCheckBox.Enabled = true;
                T2OnMyHealthBelowCheckBox.Enabled = true;
                numTrinket2MyHealth.Enabled = true;
                T2OnEnemyHealthBelowCheckBox.Enabled = true;
                numTrinket2EnemyHealth.Enabled = true;
                UseTrinket2OnBoS.Enabled = true;
            }

            if (UseTrinket2CheckBox.Checked == false)
            {
                T2OnLoCCheckBox.Enabled = false;
                T2OnCdCheckBox.Enabled = false;
                T2OnBurstCheckBox.Enabled = false;
                T2OnMyHealthBelowCheckBox.Enabled = false;
                numTrinket2MyHealth.Enabled = false;
                T2OnEnemyHealthBelowCheckBox.Enabled = false;
                numTrinket2EnemyHealth.Enabled = false;
                UseTrinket2OnBoS.Enabled = false;
            }
        }

        private void UseRacialsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.UseRacials = UseRacialsCheckBox.Checked;

            if (UseRacialsCheckBox.Checked)
            {
                UseHumanRacialCheckBox.Enabled = true;
                UseGiftoftheNaruuCheckBox.Enabled = true;
                UseStoneformCheckBox.Enabled = true;
                UseArcaneTorrentCheckBox.Enabled = true;
                BosArcaneTorrentCheckBox.Enabled = true;
                UseWarStompCheckBox.Enabled = true;
                UseBloodFuryCheckBox.Enabled = true;
                BloodFuryOnCdCheckBox.Enabled = true;
                BosBloodFuryCheckBox.Enabled = true;
                UseBerserkingCheckBox.Enabled = true;
                BerserkingOnCdCheckBox.Enabled = true;
                BosBerserkingCheckBox.Enabled = true;
            }

            if (UseRacialsCheckBox.Checked == false)
            {
                UseHumanRacialCheckBox.Enabled = false;
                UseGiftoftheNaruuCheckBox.Enabled = false;
                UseStoneformCheckBox.Enabled = false;
                UseArcaneTorrentCheckBox.Enabled = false;
                BosArcaneTorrentCheckBox.Enabled = false;
                UseWarStompCheckBox.Enabled = false;
                UseBloodFuryCheckBox.Enabled = false;
                BloodFuryOnCdCheckBox.Enabled = false;
                BosBloodFuryCheckBox.Enabled = false;
                UseBerserkingCheckBox.Enabled = false;
                BerserkingOnCdCheckBox.Enabled = false;
                BosBerserkingCheckBox.Enabled = false;

                UseHumanRacialCheckBox.Checked = false;
                UseGiftoftheNaruuCheckBox.Checked = false;
                UseStoneformCheckBox.Checked = false;
                UseArcaneTorrentCheckBox.Checked = false;
                BosArcaneTorrentCheckBox.Checked = false;
                UseWarStompCheckBox.Checked = false;
                UseBloodFuryCheckBox.Checked = false;
                BloodFuryOnCdCheckBox.Checked = false;
                BosBloodFuryCheckBox.Checked = false;
                UseBerserkingCheckBox.Checked = false;
                BerserkingOnCdCheckBox.Checked = false;
                BosBerserkingCheckBox.Checked = false;
            }
        }

        private void T2OnLoCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2LossOfControl = T2OnLoCCheckBox.Checked;

            T2OnEnemyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnMyHealthBelowCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnCdCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
            numTrinket2EnemyHealth.Enabled = !GeneralSettings.Trinket2LossOfControl;
            numTrinket2MyHealth.Enabled = !GeneralSettings.Trinket2LossOfControl;
            T2OnBurstCheckBox.Enabled = !GeneralSettings.Trinket2LossOfControl;
        }

        private void T2OnCdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2OnCooldown = T2OnCdCheckBox.Checked;
        }

        private void T2OnBurstCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2OnBurst = T2OnBurstCheckBox.Checked;
        }

        private void T2OnMyHealthBelowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2MyHealthBelow = T2OnMyHealthBelowCheckBox.Checked;
            numTrinket2MyHealth.Enabled = GeneralSettings.Trinket2MyHealthBelow;
        }

        private void T2OnEnemyHealthBelowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2EnemyHealthBelow = T2OnEnemyHealthBelowCheckBox.Checked;
            numTrinket2EnemyHealth.Enabled = GeneralSettings.Trinket2EnemyHealthBelow;
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {
        }

        private void numTrinket2MyHealth_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2MyHealth = (int) numTrinket2MyHealth.Value;
        }

        #region Form Dragging API Support

        //The SendMessage function sends a message to a window or windows.
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        //private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        //ReleaseCapture releases a mouse capture
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        #endregion Form Dragging API Support

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void UseDeathGripInPvPCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void UseDeathGripCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.DeathGrip = UseDeathGripCheckBox.Checked;
        }

        private void BosBloodFuryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.BosBloodFury = BosBerserkingCheckBox.Checked;
        }

        private void BosBerserkingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.BosBerserking = BosBerserkingCheckBox.Checked;
        }

        private void UsePillarofFrostOnCdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.PillarofFrostOnCd = UsePillarofFrostOnCdCheckBox.Checked;
        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {
        }

        private void UseIceboundFCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseIceBoundFortitude = UseIceboundFCheckBox.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseIceBoundFortitudeHp = (int) numIceBoundFortitudeHp.Value;
        }

        private void numHealingTonicHp_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.HealingTonicHp = (int) numHealingTonicHp.Value;
        }

        private void useHealingToniccheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.HealingTonicUse = useHealingToniccheckBox.Checked;
        }

        private void UseHealthstonecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.HealthstoneUse = useHealingToniccheckBox.Checked;
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            GeneralSettings.Instance.HealthstoneHp = (int) numHealthstoneUseHp.Value;
        }

        private void DeathStrikecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseDeathStrike = DeathStrikecheckBox.Checked;
        }

        private void numericUpDown1_ValueChanged_2(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseDeathStrikeHp = (int) numDeathStrikeHp.Value;
        }

        private void DeathSiphoncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseDeathSiphon = DeathSiphoncheckBox.Checked;
        }

        private void numDeathSiphonHp_ValueChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseDeathSiphonHp = (int) numDeathSiphonHp.Value;
        }

        private void UseMFcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.MindFreezeUse = UseMFcheckBox.Checked;
            DelayMFcheckBox.Enabled = DeathKnightSettings.Instance.MindFreezeUse;
            numMFmax.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
            numMFmin.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
        }

        private void DelayMFcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.MindFreezeRandomTimerUse = DelayMFcheckBox.Checked;
            numMFmax.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
            numMFmin.Enabled = DeathKnightSettings.Instance.MindFreezeUse &&
                               DeathKnightSettings.Instance.MindFreezeRandomTimerUse;
        }

        private void numericUpDown1_ValueChanged_3(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.MindFreezeRandomTimerMin = (int) numMFmin.Value;
        }

        private void numMFmax_ValueChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.MindFreezeRandomTimerMax = (int) numMFmax.Value;
        }

        private void StrangulateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.StrangulateUse = StrangulateCheckBox.Checked;
            DelayStrangulateCheckBox.Enabled = DeathKnightSettings.Instance.StrangulateUse;
            numStrangulatemax.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
            numStrangulatemin.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
        }

        private void DelayStrangulateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.StrangulateRandomTimerUse = DelayStrangulateCheckBox.Checked;
            numStrangulatemax.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
            numStrangulatemin.Enabled = DeathKnightSettings.Instance.StrangulateUse &&
                                        DeathKnightSettings.Instance.StrangulateRandomTimerUse;
        }

        private void numStrangulatemin_ValueChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.StrangulateRandomTimerMin = (int) numStrangulatemin.Value;
        }

        private void numStrangulatemax_ValueChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.StrangulateRandomTimerMax = (int) numStrangulatemax.Value;
        }

        private void UseTrinket1OnBoS_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket1OnBoS = UseTrinket1OnBoS.Checked;
        }

        private void UseTrinket2OnBoS_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.Trinket2OnBoS = UseTrinket2OnBoS.Checked;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void AutoAmsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseAms = AutoAmsCheckbox.Checked;
        }

        public static class ClassSettings
        {
            public static Styx.Helpers.Settings Settings;

            public static void Initialize()
            {
                Settings = null;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (StyxWoW.Me.Class)
                {
                    case WoWClass.DeathKnight:
                        Settings = DeathKnightSettings.Instance;
                        break;
                }
                Settings?.Load();
            }
        }

        private void UseArmyoftheDeadcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeathKnightSettings.Instance.UseAotD = UseArmyoftheDeadcheckBox.Checked;
        }

        private void PauseCrModcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.ModPauseHotkey = PauseCrModcomboBox.SelectedItem.ToString();
        }

        private void PauseCrKeycomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeneralSettings.Instance.PauseHotkey = PauseCrKeycomboBox.SelectedItem.ToString();
        }
    }
}