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
using ScourgeBloom.Managers;
using ScourgeBloom.Settings;

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
            GeneralSGrid.SelectedObject = GeneralSettings.Instance;
            RacialsLoad();
            Trinket1Load();
            Trinket2Load();
            InterruptsLoad();
            DeathGripLoad();
            SurvivabilityLoad();
            HotkeysLoad();
            CooldownsLoad();
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
            HotkeyManager.RegisterHotKeys();
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
            UseMFcheckBox.Checked = GeneralSettings.MindFreezeUse;
            DelayMFcheckBox.Checked = GeneralSettings.MindFreezeRandomTimerUse;
            numMFmax.Value = (decimal) GeneralSettings.MindFreezeRandomTimerMax;
            numMFmin.Value = (decimal) GeneralSettings.MindFreezeRandomTimerMin;

            DelayMFcheckBox.Enabled = GeneralSettings.MindFreezeUse;
            numMFmax.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
            numMFmin.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
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
            UseDeathGripCheckBox.Checked = GeneralSettings.DeathGrip;
        }

        private void SurvivabilityLoad()
        {
            DeathStrikecheckBox.Checked = GeneralSettings.UseDeathStrike;
            numDeathStrikeHp.Value = GeneralSettings.UseDeathStrikeHp;
        }

        private void CooldownsLoad()
        {
            UseIceboundFCheckBox.Checked = GeneralSettings.UseIceBoundFortitude;
            numIceBoundFortitudeHp.Value = GeneralSettings.UseIceBoundFortitudeHp;

            UseArmyoftheDeadcheckBox.Checked = GeneralSettings.UseAotD;
            AutoAmsCheckbox.Checked = GeneralSettings.UseAms;

            GargoyleOnCDcheckBox.Checked = GeneralSettings.SummonGargoyleOnCd;
        }

        private void HotkeysLoad()
        {
            LoadButtonText(GeneralSettings.Instance.HotkeyPauseModifier, GeneralSettings.Instance.HotkeyPauseKey,
                btnHotkeysPause);
        }

        private void ItemsLoad()
        {
            useHealingToniccheckBox.Checked = GeneralSettings.Instance.HealingTonicUse;
            numHealingTonicHp.Value = GeneralSettings.Instance.HealingTonicHp;

            UseHealthstonecheckBox.Checked = GeneralSettings.Instance.HealthstoneUse;
            numHealthstoneUseHp.Value = GeneralSettings.Instance.HealthstoneHp;
        }

        private void LoadButtonText(int modifierKey, string key, Button btn)
        {
            if (modifierKey <= 0 || string.IsNullOrEmpty(key))
            {
                btn.Text = "Click to Set";
                return;
            }

            var shift = false;
            var alt = false;
            var ctrl = false;

            // singles
            if (modifierKey == (int) Styx.Common.ModifierKeys.Alt)
            {
                alt = true;
                shift = false;
                ctrl = false;
            }
            if (modifierKey == (int) Styx.Common.ModifierKeys.Shift)
            {
                alt = false;
                shift = true;
                ctrl = false;
            }
            if (modifierKey == (int) Styx.Common.ModifierKeys.Control)
            {
                alt = false;
                shift = false;
                ctrl = true;
            }

            // doubles
            if (modifierKey == (int) Styx.Common.ModifierKeys.Alt + (int) Styx.Common.ModifierKeys.Control)
            {
                alt = true;
                shift = false;
                ctrl = true;
            }
            if (modifierKey == (int) Styx.Common.ModifierKeys.Alt + (int) Styx.Common.ModifierKeys.Shift)
            {
                alt = true;
                shift = true;
                ctrl = false;
            }
            if (modifierKey == (int) Styx.Common.ModifierKeys.Control + (int) Styx.Common.ModifierKeys.Shift)
            {
                alt = false;
                shift = true;
                ctrl = true;
            }

            // one triple
            if (modifierKey ==
                (int) Styx.Common.ModifierKeys.Alt + (int) Styx.Common.ModifierKeys.Control +
                (int) Styx.Common.ModifierKeys.Shift)
            {
                alt = true;
                shift = true;
                ctrl = true;
            }
            //MessageBox.Show("shift:" + shift.ToString() + ", alt:" + alt.ToString() + ", ctrl:" + ctrl.ToString());
            var btnText = GetKeyModifierText(alt, shift, ctrl, key);
            btn.Text = btnText;
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
            GeneralSettings.DeathGrip = true;
            GeneralSettings.Instance.Interrupts = true;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            GeneralSettings.UseDeathStrike = true;
            GeneralSettings.UseDeathStrikeHp = 60;
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
            GeneralSettings.DeathGrip = false;
            GeneralSettings.Instance.Interrupts = false;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            GeneralSettings.UseDeathStrike = false;
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
            GeneralSettings.DeathGrip = false;
            GeneralSettings.Instance.Interrupts = true;
            GeneralSettings.Instance.UseRacials = true;
            GeneralSettings.Instance.Cooldowns = true;
            GeneralSettings.UseDeathStrike = true;
            GeneralSettings.UseDeathStrikeHp = 15;
            On_Exit(sender, e);
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
            GeneralSettings.SummonGargoyleOnCd = GargoyleOnCDcheckBox.Checked;
        }

        private void BosArcaneTorrentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.BosArcaneTorrent = BosArcaneTorrentCheckBox.Checked;
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

            if (!UseTrinket1CheckBox.Checked)
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

            if (!UseTrinket2CheckBox.Checked)
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

            if (!UseRacialsCheckBox.Checked)
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
            GeneralSettings.DeathGrip = UseDeathGripCheckBox.Checked;
        }

        private void BosBloodFuryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.BosBloodFury = BosBerserkingCheckBox.Checked;
        }

        private void BosBerserkingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.BosBerserking = BosBerserkingCheckBox.Checked;
        }

        private void UsePillarofFrostOnCdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.PillarofFrostOnCd = UsePillarofFrostOnCdCheckBox.Checked;
        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {
        }

        private void UseIceboundFCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.UseIceBoundFortitude = UseIceboundFCheckBox.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.UseIceBoundFortitudeHp = (int) numIceBoundFortitudeHp.Value;
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
            GeneralSettings.UseDeathStrike = DeathStrikecheckBox.Checked;
        }

        private void numericUpDown1_ValueChanged_2(object sender, EventArgs e)
        {
            GeneralSettings.UseDeathStrikeHp = (int) numDeathStrikeHp.Value;
        }

        private void UseMFcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.MindFreezeUse = UseMFcheckBox.Checked;
            DelayMFcheckBox.Enabled = GeneralSettings.MindFreezeUse;
            numMFmax.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
            numMFmin.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
        }

        private void DelayMFcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.MindFreezeRandomTimerUse = DelayMFcheckBox.Checked;
            numMFmax.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
            numMFmin.Enabled = GeneralSettings.MindFreezeUse &&
                               GeneralSettings.MindFreezeRandomTimerUse;
        }

        private void numericUpDown1_ValueChanged_3(object sender, EventArgs e)
        {
            GeneralSettings.MindFreezeRandomTimerMin = (int) numMFmin.Value;
        }

        private void numMFmax_ValueChanged(object sender, EventArgs e)
        {
            GeneralSettings.MindFreezeRandomTimerMax = (int) numMFmax.Value;
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
            GeneralSettings.UseAms = AutoAmsCheckbox.Checked;
        }

        private void UseArmyoftheDeadcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettings.UseAotD = UseArmyoftheDeadcheckBox.Checked;
        }

        private void btnHotkeysPause_Click(object sender, EventArgs e)
        {
            captureKeyPress = true;
            btnHotkeysPause.Text = "";
        }

        private void btnHotkeysPause_KeyDown(object sender, KeyEventArgs e)
        {
            CheckIfKeyPressed(e);
            btnHotkeysPause.Text = GetKeyModifierText();
        }

        private void btnHotkeysPause_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ClearButtonHotkey(btnHotkeysPause, KeybindTypes.Pause);
                return;
            }
            if (DidPressCorrectKey())
            {
                GeneralSettings.Instance.HotkeyPauseKey = keyPressed;
                GeneralSettings.Instance.HotkeyPauseModifier = GetKeyModifierPressed();
            }
            ResetKeys();
        }

        #region Hotkey Methods

        private bool captureKeyPress;
        private bool controlPressed;
        private bool altPressed;
        private bool shiftPressed;
        private string keyPressed = "";

        private void CheckIfKeyPressed(KeyEventArgs e)
        {
            if (captureKeyPress)
            {
                if (e.Alt)
                {
                    altPressed = true;
                }
                if (e.Control)
                {
                    controlPressed = true;
                }
                if (e.Shift)
                {
                    shiftPressed = true;
                }
                var isLetterOrDigit = char.IsLetterOrDigit((char) e.KeyCode);
                if (isLetterOrDigit)
                {
                    keyPressed = new KeysConverter().ConvertToString(e.KeyCode);
                }
            }
        }

        private enum KeybindTypes
        {
            Pause
        }

        private void ClearButtonHotkey(Button btn, KeybindTypes kbType)
        {
            btn.Text = "Click to Set";
            ResetKeys();
            if (kbType == KeybindTypes.Pause)
            {
                GeneralSettings.Instance.HotkeyPauseKey = "";
                GeneralSettings.Instance.HotkeyPauseModifier = 0;
            }
            ResetKeys();
        }

        private void ResetKeys()
        {
            captureKeyPress = false;
            altPressed = false;
            controlPressed = false;
            shiftPressed = false;
            keyPressed = "";
        }

        private string GetKeyModifierText()
        {
            var text = "";
            if ((altPressed || controlPressed || shiftPressed) && !string.IsNullOrEmpty(keyPressed))
            {
                #region Control + other stuff

                if (controlPressed && !altPressed && !shiftPressed)
                {
                    return string.Format($"Ctrl + {keyPressed.ToUpper()}");
                }
                if (controlPressed && altPressed && !shiftPressed)
                {
                    return string.Format($"Ctrl + Alt + {keyPressed.ToUpper()}");
                }
                if (controlPressed && !altPressed && shiftPressed)
                {
                    return string.Format($"Ctrl + Shift + {keyPressed.ToUpper()}");
                }
                if (controlPressed && altPressed && shiftPressed)
                {
                    return string.Format($"Ctrl + Alt + Shift + {keyPressed.ToUpper()}");
                }

                #endregion

                if (!controlPressed && altPressed && !shiftPressed)
                {
                    return string.Format($"Alt + {keyPressed.ToUpper()}");
                }
                if (!controlPressed && altPressed && shiftPressed)
                {
                    return string.Format($"Alt + Shift + {keyPressed.ToUpper()}");
                }
                if (!controlPressed && !altPressed && shiftPressed)
                {
                    return string.Format($"Shift + {keyPressed.ToUpper()}");
                }
            }
            return text;
        }

        private string GetKeyModifierText(bool alt, bool shift, bool ctrl, string key)
        {
            var text = "";
            if ((alt || ctrl || shift) && !string.IsNullOrEmpty(key))
            {
                #region Control + other stuff

                if (ctrl && !alt && !shift)
                {
                    return string.Format($"Ctrl + {key.ToUpper()}");
                }
                if (ctrl && alt && !shift)
                {
                    return string.Format($"Ctrl + Alt + {key.ToUpper()}");
                }
                if (ctrl && !alt && shift)
                {
                    return string.Format($"Ctrl + Shift + {key.ToUpper()}");
                }
                if (ctrl && alt && shift)
                {
                    return string.Format($"Ctrl + Alt + Shift + {key.ToUpper()}");
                }

                #endregion

                if (!ctrl && alt && !shift)
                {
                    return string.Format($"Alt + {key.ToUpper()}");
                }
                if (!ctrl && alt && shift)
                {
                    return string.Format($"Alt + Shift + {key.ToUpper()}");
                }
                if (!ctrl && !alt && shift)
                {
                    return string.Format($"Shift + {key.ToUpper()}");
                }
            }
            return text;
        }

        private bool DidPressCorrectKey()
        {
            if (captureKeyPress)
            {
                if ((altPressed || controlPressed || shiftPressed) && !string.IsNullOrEmpty(keyPressed))
                {
                    return true;
                }
            }
            return false;
        }

        private int GetKeyModifierPressed()
        {
            if (altPressed || controlPressed || shiftPressed)
            {
                var modifierKey = 0;
                if (altPressed)
                {
                    modifierKey += (int) Styx.Common.ModifierKeys.Alt;
                }
                if (controlPressed)
                {
                    modifierKey += (int) Styx.Common.ModifierKeys.Control;
                }
                if (shiftPressed)
                {
                    modifierKey += (int) Styx.Common.ModifierKeys.Shift;
                }
                return modifierKey;
            }
            return 0;
        }

        #endregion
    }
}