/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.ComponentModel;
using System.Windows.Forms;

namespace ScourgeBloom
{
    partial class ScourgeBloomSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.Changelog = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.General = new System.Windows.Forms.TabPage();
            this.GeneralSGrid = new System.Windows.Forms.PropertyGrid();
            this.Cooldowns = new System.Windows.Forms.TabPage();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.UseArmyoftheDeadcheckBox = new System.Windows.Forms.CheckBox();
            this.AutoAmsCheckbox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numIceBoundFortitudeHp = new System.Windows.Forms.NumericUpDown();
            this.UseIceboundFCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.UsePillarofFrostOnCdCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.GargoyleOnCDcheckBox = new System.Windows.Forms.CheckBox();
            this.Talents = new System.Windows.Forms.TabPage();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.UseBoScheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.UseDefilecheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.UseDesecratedGroundcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.UseRemorselessWintercheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.UseGorefiendsGraspcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.UseConversioncheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.UseDeathSiphoncheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.UseDeathPactcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.UseBloodTapcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.UseAsphyxiatecheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.UseDeathsAdvancecheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.UseAMZcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.UseLichBornecheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.UseUnholyBlightcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.UsePLagueLeechcheckBox = new System.Windows.Forms.CheckBox();
            this.Racials = new System.Windows.Forms.TabPage();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.BerserkingOnCdCheckBox = new System.Windows.Forms.CheckBox();
            this.BosBerserkingCheckBox = new System.Windows.Forms.CheckBox();
            this.UseBerserkingCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.BloodFuryOnCdCheckBox = new System.Windows.Forms.CheckBox();
            this.BosBloodFuryCheckBox = new System.Windows.Forms.CheckBox();
            this.UseBloodFuryCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numWarStompEnemies = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.UseWarStompCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.BosArcaneTorrentCheckBox = new System.Windows.Forms.CheckBox();
            this.UseArcaneTorrentCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numStoneformHp = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.UseStoneformOnlyToClearCheckBox = new System.Windows.Forms.CheckBox();
            this.UseStoneformCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.UseGiftoftheNaruuCheckBox = new System.Windows.Forms.CheckBox();
            this.numGiftOfTheNaaruHp = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.UseHumanRacialCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.UseRacialsCheckBox = new System.Windows.Forms.CheckBox();
            this.Trinkets = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.UseTrinket2OnBoS = new System.Windows.Forms.CheckBox();
            this.numTrinket2EnemyHealth = new System.Windows.Forms.NumericUpDown();
            this.numTrinket2MyHealth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.T2OnEnemyHealthBelowCheckBox = new System.Windows.Forms.CheckBox();
            this.T2OnMyHealthBelowCheckBox = new System.Windows.Forms.CheckBox();
            this.T2OnBurstCheckBox = new System.Windows.Forms.CheckBox();
            this.T2OnCdCheckBox = new System.Windows.Forms.CheckBox();
            this.T2OnLoCCheckBox = new System.Windows.Forms.CheckBox();
            this.UseTrinket2CheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.UseTrinket1OnBoS = new System.Windows.Forms.CheckBox();
            this.numTrinket1EnemyHealth = new System.Windows.Forms.NumericUpDown();
            this.numTrinket1MyHealth = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.T1OnEnemyHealthBelowCheckBox = new System.Windows.Forms.CheckBox();
            this.T1OnMyHealthBelowCheckBox = new System.Windows.Forms.CheckBox();
            this.T1OnBurstCheckBox = new System.Windows.Forms.CheckBox();
            this.T1OnCdCheckBox = new System.Windows.Forms.CheckBox();
            this.T1OnLoCCheckBox = new System.Windows.Forms.CheckBox();
            this.UseTrinket1CheckBox = new System.Windows.Forms.CheckBox();
            this.Advanced = new System.Windows.Forms.TabPage();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.numStrangulatemax = new System.Windows.Forms.NumericUpDown();
            this.numStrangulatemin = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.DelayStrangulateCheckBox = new System.Windows.Forms.CheckBox();
            this.StrangulateCheckBox = new System.Windows.Forms.CheckBox();
            this.numMFmax = new System.Windows.Forms.NumericUpDown();
            this.numMFmin = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.DelayMFcheckBox = new System.Windows.Forms.CheckBox();
            this.UseMFcheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.numHealthstoneUseHp = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.UseHealthstonecheckBox = new System.Windows.Forms.CheckBox();
            this.numHealingTonicHp = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.useHealingToniccheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.numDeathSiphonHp = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.numDeathStrikeHp = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.DeathSiphoncheckBox = new System.Windows.Forms.CheckBox();
            this.DeathStrikecheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.UseDeathGripInPvPCheckBox = new System.Windows.Forms.CheckBox();
            this.UseDeathGripCheckBox = new System.Windows.Forms.CheckBox();
            this.Debug = new System.Windows.Forms.TabPage();
            this.DumpSpellsButton = new System.Windows.Forms.Button();
            this.DungeonBuddyPresetButton = new System.Windows.Forms.Button();
            this.RotationOnlyPresetButton = new System.Windows.Forms.Button();
            this.AFKPresetButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.PVPPresetButton = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.Hotkeys = new System.Windows.Forms.TabPage();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.PauseCrModcomboBox = new System.Windows.Forms.ComboBox();
            this.PauseCrKeycomboBox = new System.Windows.Forms.ComboBox();
            this.TabControl1.SuspendLayout();
            this.Changelog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.General.SuspendLayout();
            this.Cooldowns.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIceBoundFortitudeHp)).BeginInit();
            this.groupBox14.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.Talents.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.groupBox33.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox28.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.Racials.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWarStompEnemies)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStoneformHp)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGiftOfTheNaaruHp)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Trinkets.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket2EnemyHealth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket2MyHealth)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket1EnemyHealth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket1MyHealth)).BeginInit();
            this.Advanced.SuspendLayout();
            this.groupBox19.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStrangulatemax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrangulatemin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMFmax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMFmin)).BeginInit();
            this.groupBox18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHealthstoneUseHp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealingTonicHp)).BeginInit();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeathSiphonHp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDeathStrikeHp)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.Debug.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.Hotkeys.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.Changelog);
            this.TabControl1.Controls.Add(this.General);
            this.TabControl1.Controls.Add(this.Cooldowns);
            this.TabControl1.Controls.Add(this.Talents);
            this.TabControl1.Controls.Add(this.Racials);
            this.TabControl1.Controls.Add(this.Trinkets);
            this.TabControl1.Controls.Add(this.Hotkeys);
            this.TabControl1.Controls.Add(this.Advanced);
            this.TabControl1.Controls.Add(this.Debug);
            this.TabControl1.Location = new System.Drawing.Point(0, 1);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(583, 530);
            this.TabControl1.TabIndex = 0;
            // 
            // Changelog
            // 
            this.Changelog.Controls.Add(this.textBox1);
            this.Changelog.Controls.Add(this.pictureBox1);
            this.Changelog.Location = new System.Drawing.Point(4, 22);
            this.Changelog.Name = "Changelog";
            this.Changelog.Size = new System.Drawing.Size(575, 504);
            this.Changelog.TabIndex = 4;
            this.Changelog.Text = "Changelog";
            this.Changelog.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 390);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(575, 114);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ScourgeBloom.Properties.Resources._700by250;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(574, 366);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // General
            // 
            this.General.Controls.Add(this.GeneralSGrid);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(575, 504);
            this.General.TabIndex = 0;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // GeneralSGrid
            // 
            this.GeneralSGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.GeneralSGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralSGrid.Location = new System.Drawing.Point(3, 3);
            this.GeneralSGrid.Name = "GeneralSGrid";
            this.GeneralSGrid.Size = new System.Drawing.Size(569, 498);
            this.GeneralSGrid.TabIndex = 0;
            // 
            // Cooldowns
            // 
            this.Cooldowns.Controls.Add(this.groupBox16);
            this.Cooldowns.Controls.Add(this.groupBox15);
            this.Cooldowns.Controls.Add(this.groupBox14);
            this.Cooldowns.Controls.Add(this.groupBox7);
            this.Cooldowns.Location = new System.Drawing.Point(4, 22);
            this.Cooldowns.Name = "Cooldowns";
            this.Cooldowns.Size = new System.Drawing.Size(575, 504);
            this.Cooldowns.TabIndex = 7;
            this.Cooldowns.Text = "Cooldowns";
            this.Cooldowns.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.UseArmyoftheDeadcheckBox);
            this.groupBox16.Controls.Add(this.AutoAmsCheckbox);
            this.groupBox16.Controls.Add(this.label10);
            this.groupBox16.Controls.Add(this.numIceBoundFortitudeHp);
            this.groupBox16.Controls.Add(this.UseIceboundFCheckBox);
            this.groupBox16.Location = new System.Drawing.Point(8, 3);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(558, 108);
            this.groupBox16.TabIndex = 3;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Common";
            // 
            // UseArmyoftheDeadcheckBox
            // 
            this.UseArmyoftheDeadcheckBox.AutoSize = true;
            this.UseArmyoftheDeadcheckBox.Checked = true;
            this.UseArmyoftheDeadcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseArmyoftheDeadcheckBox.Location = new System.Drawing.Point(168, 19);
            this.UseArmyoftheDeadcheckBox.Name = "UseArmyoftheDeadcheckBox";
            this.UseArmyoftheDeadcheckBox.Size = new System.Drawing.Size(130, 17);
            this.UseArmyoftheDeadcheckBox.TabIndex = 9;
            this.UseArmyoftheDeadcheckBox.Text = "Use Army of the Dead";
            this.UseArmyoftheDeadcheckBox.UseVisualStyleBackColor = true;
            this.UseArmyoftheDeadcheckBox.CheckedChanged += new System.EventHandler(this.UseArmyoftheDeadcheckBox_CheckedChanged);
            // 
            // AutoAmsCheckbox
            // 
            this.AutoAmsCheckbox.AutoSize = true;
            this.AutoAmsCheckbox.Checked = true;
            this.AutoAmsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoAmsCheckbox.Location = new System.Drawing.Point(6, 68);
            this.AutoAmsCheckbox.Name = "AutoAmsCheckbox";
            this.AutoAmsCheckbox.Size = new System.Drawing.Size(124, 17);
            this.AutoAmsCheckbox.TabIndex = 8;
            this.AutoAmsCheckbox.Text = "Use Anti-Magic Shell";
            this.AutoAmsCheckbox.UseVisualStyleBackColor = true;
            this.AutoAmsCheckbox.CheckedChanged += new System.EventHandler(this.AutoAmsCheckbox_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Hp% :";
            // 
            // numIceBoundFortitudeHp
            // 
            this.numIceBoundFortitudeHp.Location = new System.Drawing.Point(71, 42);
            this.numIceBoundFortitudeHp.Name = "numIceBoundFortitudeHp";
            this.numIceBoundFortitudeHp.Size = new System.Drawing.Size(61, 20);
            this.numIceBoundFortitudeHp.TabIndex = 2;
            this.numIceBoundFortitudeHp.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numIceBoundFortitudeHp.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // UseIceboundFCheckBox
            // 
            this.UseIceboundFCheckBox.AutoSize = true;
            this.UseIceboundFCheckBox.Checked = true;
            this.UseIceboundFCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseIceboundFCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseIceboundFCheckBox.Name = "UseIceboundFCheckBox";
            this.UseIceboundFCheckBox.Size = new System.Drawing.Size(140, 17);
            this.UseIceboundFCheckBox.TabIndex = 1;
            this.UseIceboundFCheckBox.Text = "Auto Icebound Fortitude";
            this.UseIceboundFCheckBox.UseVisualStyleBackColor = true;
            this.UseIceboundFCheckBox.CheckedChanged += new System.EventHandler(this.UseIceboundFCheckBox_CheckedChanged);
            // 
            // groupBox15
            // 
            this.groupBox15.Location = new System.Drawing.Point(8, 345);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(558, 108);
            this.groupBox15.TabIndex = 2;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Blood";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.UsePillarofFrostOnCdCheckBox);
            this.groupBox14.Location = new System.Drawing.Point(8, 231);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(558, 108);
            this.groupBox14.TabIndex = 1;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Frost";
            this.groupBox14.Enter += new System.EventHandler(this.groupBox14_Enter);
            // 
            // UsePillarofFrostOnCdCheckBox
            // 
            this.UsePillarofFrostOnCdCheckBox.AutoSize = true;
            this.UsePillarofFrostOnCdCheckBox.Checked = true;
            this.UsePillarofFrostOnCdCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UsePillarofFrostOnCdCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UsePillarofFrostOnCdCheckBox.Name = "UsePillarofFrostOnCdCheckBox";
            this.UsePillarofFrostOnCdCheckBox.Size = new System.Drawing.Size(173, 17);
            this.UsePillarofFrostOnCdCheckBox.TabIndex = 1;
            this.UsePillarofFrostOnCdCheckBox.Text = "Use Pillar of Frost on Cooldown";
            this.UsePillarofFrostOnCdCheckBox.UseVisualStyleBackColor = true;
            this.UsePillarofFrostOnCdCheckBox.CheckedChanged += new System.EventHandler(this.UsePillarofFrostOnCdCheckBox_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.GargoyleOnCDcheckBox);
            this.groupBox7.Location = new System.Drawing.Point(8, 117);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(558, 108);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Unholy";
            // 
            // GargoyleOnCDcheckBox
            // 
            this.GargoyleOnCDcheckBox.AutoSize = true;
            this.GargoyleOnCDcheckBox.Checked = true;
            this.GargoyleOnCDcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GargoyleOnCDcheckBox.Location = new System.Drawing.Point(6, 19);
            this.GargoyleOnCDcheckBox.Name = "GargoyleOnCDcheckBox";
            this.GargoyleOnCDcheckBox.Size = new System.Drawing.Size(177, 17);
            this.GargoyleOnCDcheckBox.TabIndex = 0;
            this.GargoyleOnCDcheckBox.Text = "Summon Gargoyle on Cooldown";
            this.GargoyleOnCDcheckBox.UseVisualStyleBackColor = true;
            this.GargoyleOnCDcheckBox.CheckedChanged += new System.EventHandler(this.GargoyleOnCDcheckBox_CheckedChanged);
            // 
            // Talents
            // 
            this.Talents.Controls.Add(this.groupBox31);
            this.Talents.Controls.Add(this.groupBox30);
            this.Talents.Controls.Add(this.groupBox27);
            this.Talents.Controls.Add(this.groupBox25);
            this.Talents.Controls.Add(this.groupBox20);
            this.Talents.Controls.Add(this.groupBox34);
            this.Talents.Controls.Add(this.groupBox33);
            this.Talents.Controls.Add(this.groupBox32);
            this.Talents.Controls.Add(this.groupBox29);
            this.Talents.Controls.Add(this.groupBox28);
            this.Talents.Controls.Add(this.groupBox26);
            this.Talents.Controls.Add(this.groupBox24);
            this.Talents.Controls.Add(this.groupBox23);
            this.Talents.Controls.Add(this.groupBox22);
            this.Talents.Controls.Add(this.groupBox21);
            this.Talents.Location = new System.Drawing.Point(4, 22);
            this.Talents.Name = "Talents";
            this.Talents.Size = new System.Drawing.Size(575, 504);
            this.Talents.TabIndex = 9;
            this.Talents.Text = "Talents";
            this.Talents.UseVisualStyleBackColor = true;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.UseBoScheckBox);
            this.groupBox31.Location = new System.Drawing.Point(384, 404);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(182, 94);
            this.groupBox31.TabIndex = 18;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Breath of Sindragosa";
            // 
            // UseBoScheckBox
            // 
            this.UseBoScheckBox.AutoSize = true;
            this.UseBoScheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseBoScheckBox.Name = "UseBoScheckBox";
            this.UseBoScheckBox.Size = new System.Drawing.Size(147, 17);
            this.UseBoScheckBox.TabIndex = 2;
            this.UseBoScheckBox.Text = "Use Breath of Sindragosa";
            this.UseBoScheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.UseDefilecheckBox);
            this.groupBox30.Location = new System.Drawing.Point(196, 404);
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.Size = new System.Drawing.Size(182, 94);
            this.groupBox30.TabIndex = 17;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Defile";
            // 
            // UseDefilecheckBox
            // 
            this.UseDefilecheckBox.AutoSize = true;
            this.UseDefilecheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDefilecheckBox.Name = "UseDefilecheckBox";
            this.UseDefilecheckBox.Size = new System.Drawing.Size(75, 17);
            this.UseDefilecheckBox.TabIndex = 2;
            this.UseDefilecheckBox.Text = "Use Defile";
            this.UseDefilecheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.UseDesecratedGroundcheckBox);
            this.groupBox27.Location = new System.Drawing.Point(8, 404);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(182, 94);
            this.groupBox27.TabIndex = 16;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Desecrated Ground";
            // 
            // UseDesecratedGroundcheckBox
            // 
            this.UseDesecratedGroundcheckBox.AutoSize = true;
            this.UseDesecratedGroundcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDesecratedGroundcheckBox.Name = "UseDesecratedGroundcheckBox";
            this.UseDesecratedGroundcheckBox.Size = new System.Drawing.Size(141, 17);
            this.UseDesecratedGroundcheckBox.TabIndex = 2;
            this.UseDesecratedGroundcheckBox.Text = "Use Desecrated Ground";
            this.UseDesecratedGroundcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.UseRemorselessWintercheckBox);
            this.groupBox25.Location = new System.Drawing.Point(384, 304);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(182, 94);
            this.groupBox25.TabIndex = 15;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Remorseless Winter";
            // 
            // UseRemorselessWintercheckBox
            // 
            this.UseRemorselessWintercheckBox.AutoSize = true;
            this.UseRemorselessWintercheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseRemorselessWintercheckBox.Name = "UseRemorselessWintercheckBox";
            this.UseRemorselessWintercheckBox.Size = new System.Drawing.Size(142, 17);
            this.UseRemorselessWintercheckBox.TabIndex = 2;
            this.UseRemorselessWintercheckBox.Text = "Use Remorseless Winter";
            this.UseRemorselessWintercheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.UseGorefiendsGraspcheckBox);
            this.groupBox20.Location = new System.Drawing.Point(196, 304);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(182, 94);
            this.groupBox20.TabIndex = 14;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "Gorefiend\'s Grasp";
            // 
            // UseGorefiendsGraspcheckBox
            // 
            this.UseGorefiendsGraspcheckBox.AutoSize = true;
            this.UseGorefiendsGraspcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseGorefiendsGraspcheckBox.Name = "UseGorefiendsGraspcheckBox";
            this.UseGorefiendsGraspcheckBox.Size = new System.Drawing.Size(132, 17);
            this.UseGorefiendsGraspcheckBox.TabIndex = 2;
            this.UseGorefiendsGraspcheckBox.Text = "Use Gorefiend\'s Grasp";
            this.UseGorefiendsGraspcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.UseConversioncheckBox);
            this.groupBox34.Location = new System.Drawing.Point(8, 304);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(182, 94);
            this.groupBox34.TabIndex = 13;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Conversation";
            // 
            // UseConversioncheckBox
            // 
            this.UseConversioncheckBox.AutoSize = true;
            this.UseConversioncheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseConversioncheckBox.Name = "UseConversioncheckBox";
            this.UseConversioncheckBox.Size = new System.Drawing.Size(101, 17);
            this.UseConversioncheckBox.TabIndex = 2;
            this.UseConversioncheckBox.Text = "Use Conversion";
            this.UseConversioncheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.UseDeathSiphoncheckBox);
            this.groupBox33.Location = new System.Drawing.Point(384, 204);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(182, 94);
            this.groupBox33.TabIndex = 12;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Death Siphon";
            // 
            // UseDeathSiphoncheckBox
            // 
            this.UseDeathSiphoncheckBox.AutoSize = true;
            this.UseDeathSiphoncheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDeathSiphoncheckBox.Name = "UseDeathSiphoncheckBox";
            this.UseDeathSiphoncheckBox.Size = new System.Drawing.Size(113, 17);
            this.UseDeathSiphoncheckBox.TabIndex = 0;
            this.UseDeathSiphoncheckBox.Text = "Use Death Siphon";
            this.UseDeathSiphoncheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.UseDeathPactcheckBox);
            this.groupBox32.Location = new System.Drawing.Point(196, 204);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(182, 94);
            this.groupBox32.TabIndex = 11;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Death Pact";
            // 
            // UseDeathPactcheckBox
            // 
            this.UseDeathPactcheckBox.AutoSize = true;
            this.UseDeathPactcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDeathPactcheckBox.Name = "UseDeathPactcheckBox";
            this.UseDeathPactcheckBox.Size = new System.Drawing.Size(102, 17);
            this.UseDeathPactcheckBox.TabIndex = 2;
            this.UseDeathPactcheckBox.Text = "Use Death Pact";
            this.UseDeathPactcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.UseBloodTapcheckBox);
            this.groupBox29.Location = new System.Drawing.Point(8, 204);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(182, 94);
            this.groupBox29.TabIndex = 8;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Blood Tap";
            // 
            // UseBloodTapcheckBox
            // 
            this.UseBloodTapcheckBox.AutoSize = true;
            this.UseBloodTapcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseBloodTapcheckBox.Name = "UseBloodTapcheckBox";
            this.UseBloodTapcheckBox.Size = new System.Drawing.Size(97, 17);
            this.UseBloodTapcheckBox.TabIndex = 2;
            this.UseBloodTapcheckBox.Text = "Use Blood Tap";
            this.UseBloodTapcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.UseAsphyxiatecheckBox);
            this.groupBox28.Location = new System.Drawing.Point(384, 104);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(182, 94);
            this.groupBox28.TabIndex = 7;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "Asphyxiate";
            // 
            // UseAsphyxiatecheckBox
            // 
            this.UseAsphyxiatecheckBox.AutoSize = true;
            this.UseAsphyxiatecheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseAsphyxiatecheckBox.Name = "UseAsphyxiatecheckBox";
            this.UseAsphyxiatecheckBox.Size = new System.Drawing.Size(99, 17);
            this.UseAsphyxiatecheckBox.TabIndex = 2;
            this.UseAsphyxiatecheckBox.Text = "Use Asphyxiate";
            this.UseAsphyxiatecheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.UseDeathsAdvancecheckBox);
            this.groupBox26.Location = new System.Drawing.Point(196, 104);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(182, 94);
            this.groupBox26.TabIndex = 5;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Death\'s Advance";
            // 
            // UseDeathsAdvancecheckBox
            // 
            this.UseDeathsAdvancecheckBox.AutoSize = true;
            this.UseDeathsAdvancecheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDeathsAdvancecheckBox.Name = "UseDeathsAdvancecheckBox";
            this.UseDeathsAdvancecheckBox.Size = new System.Drawing.Size(130, 17);
            this.UseDeathsAdvancecheckBox.TabIndex = 0;
            this.UseDeathsAdvancecheckBox.Text = "Use Death\'s Advance";
            this.UseDeathsAdvancecheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.UseAMZcheckBox);
            this.groupBox24.Location = new System.Drawing.Point(8, 104);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(182, 94);
            this.groupBox24.TabIndex = 3;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Anti-Magic Zone";
            // 
            // UseAMZcheckBox
            // 
            this.UseAMZcheckBox.AutoSize = true;
            this.UseAMZcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseAMZcheckBox.Name = "UseAMZcheckBox";
            this.UseAMZcheckBox.Size = new System.Drawing.Size(126, 17);
            this.UseAMZcheckBox.TabIndex = 1;
            this.UseAMZcheckBox.Text = "Use Anti-Magic Zone";
            this.UseAMZcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.UseLichBornecheckBox);
            this.groupBox23.Location = new System.Drawing.Point(384, 4);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(182, 94);
            this.groupBox23.TabIndex = 2;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Lichborne";
            // 
            // UseLichBornecheckBox
            // 
            this.UseLichBornecheckBox.AutoSize = true;
            this.UseLichBornecheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseLichBornecheckBox.Name = "UseLichBornecheckBox";
            this.UseLichBornecheckBox.Size = new System.Drawing.Size(95, 17);
            this.UseLichBornecheckBox.TabIndex = 2;
            this.UseLichBornecheckBox.Text = "Use Lichborne";
            this.UseLichBornecheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.UseUnholyBlightcheckBox);
            this.groupBox22.Location = new System.Drawing.Point(196, 4);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(182, 94);
            this.groupBox22.TabIndex = 1;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Unholy Blight";
            // 
            // UseUnholyBlightcheckBox
            // 
            this.UseUnholyBlightcheckBox.AutoSize = true;
            this.UseUnholyBlightcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseUnholyBlightcheckBox.Name = "UseUnholyBlightcheckBox";
            this.UseUnholyBlightcheckBox.Size = new System.Drawing.Size(110, 17);
            this.UseUnholyBlightcheckBox.TabIndex = 2;
            this.UseUnholyBlightcheckBox.Text = "Use Unholy Blight";
            this.UseUnholyBlightcheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.UsePLagueLeechcheckBox);
            this.groupBox21.Location = new System.Drawing.Point(8, 4);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(182, 94);
            this.groupBox21.TabIndex = 1;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Plague Leech";
            // 
            // UsePLagueLeechcheckBox
            // 
            this.UsePLagueLeechcheckBox.AutoSize = true;
            this.UsePLagueLeechcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UsePLagueLeechcheckBox.Name = "UsePLagueLeechcheckBox";
            this.UsePLagueLeechcheckBox.Size = new System.Drawing.Size(114, 17);
            this.UsePLagueLeechcheckBox.TabIndex = 0;
            this.UsePLagueLeechcheckBox.Text = "Use Plague Leech";
            this.UsePLagueLeechcheckBox.UseVisualStyleBackColor = true;
            // 
            // Racials
            // 
            this.Racials.Controls.Add(this.groupBox13);
            this.Racials.Controls.Add(this.groupBox10);
            this.Racials.Controls.Add(this.groupBox6);
            this.Racials.Controls.Add(this.groupBox5);
            this.Racials.Controls.Add(this.groupBox4);
            this.Racials.Controls.Add(this.groupBox3);
            this.Racials.Controls.Add(this.groupBox2);
            this.Racials.Controls.Add(this.groupBox1);
            this.Racials.Location = new System.Drawing.Point(4, 22);
            this.Racials.Name = "Racials";
            this.Racials.Size = new System.Drawing.Size(575, 504);
            this.Racials.TabIndex = 5;
            this.Racials.Text = "Racials";
            this.Racials.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.BerserkingOnCdCheckBox);
            this.groupBox13.Controls.Add(this.BosBerserkingCheckBox);
            this.groupBox13.Controls.Add(this.UseBerserkingCheckBox);
            this.groupBox13.Location = new System.Drawing.Point(291, 286);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(275, 85);
            this.groupBox13.TabIndex = 4;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Troll";
            // 
            // BerserkingOnCdCheckBox
            // 
            this.BerserkingOnCdCheckBox.AutoSize = true;
            this.BerserkingOnCdCheckBox.Location = new System.Drawing.Point(6, 37);
            this.BerserkingOnCdCheckBox.Name = "BerserkingOnCdCheckBox";
            this.BerserkingOnCdCheckBox.Size = new System.Drawing.Size(110, 17);
            this.BerserkingOnCdCheckBox.TabIndex = 3;
            this.BerserkingOnCdCheckBox.Text = "Use on Cooldown";
            this.BerserkingOnCdCheckBox.UseVisualStyleBackColor = true;
            // 
            // BosBerserkingCheckBox
            // 
            this.BosBerserkingCheckBox.AutoSize = true;
            this.BosBerserkingCheckBox.Location = new System.Drawing.Point(6, 60);
            this.BosBerserkingCheckBox.Name = "BosBerserkingCheckBox";
            this.BosBerserkingCheckBox.Size = new System.Drawing.Size(169, 17);
            this.BosBerserkingCheckBox.TabIndex = 2;
            this.BosBerserkingCheckBox.Text = "Use with Breath of Sindragosa";
            this.BosBerserkingCheckBox.UseVisualStyleBackColor = true;
            this.BosBerserkingCheckBox.CheckedChanged += new System.EventHandler(this.BosBerserkingCheckBox_CheckedChanged);
            // 
            // UseBerserkingCheckBox
            // 
            this.UseBerserkingCheckBox.AutoSize = true;
            this.UseBerserkingCheckBox.Location = new System.Drawing.Point(6, 14);
            this.UseBerserkingCheckBox.Name = "UseBerserkingCheckBox";
            this.UseBerserkingCheckBox.Size = new System.Drawing.Size(98, 17);
            this.UseBerserkingCheckBox.TabIndex = 0;
            this.UseBerserkingCheckBox.Text = "Use Berserking";
            this.UseBerserkingCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.BloodFuryOnCdCheckBox);
            this.groupBox10.Controls.Add(this.BosBloodFuryCheckBox);
            this.groupBox10.Controls.Add(this.UseBloodFuryCheckBox);
            this.groupBox10.Location = new System.Drawing.Point(8, 281);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(275, 90);
            this.groupBox10.TabIndex = 3;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Orc";
            // 
            // BloodFuryOnCdCheckBox
            // 
            this.BloodFuryOnCdCheckBox.AutoSize = true;
            this.BloodFuryOnCdCheckBox.Location = new System.Drawing.Point(6, 42);
            this.BloodFuryOnCdCheckBox.Name = "BloodFuryOnCdCheckBox";
            this.BloodFuryOnCdCheckBox.Size = new System.Drawing.Size(110, 17);
            this.BloodFuryOnCdCheckBox.TabIndex = 3;
            this.BloodFuryOnCdCheckBox.Text = "Use on Cooldown";
            this.BloodFuryOnCdCheckBox.UseVisualStyleBackColor = true;
            // 
            // BosBloodFuryCheckBox
            // 
            this.BosBloodFuryCheckBox.AutoSize = true;
            this.BosBloodFuryCheckBox.Location = new System.Drawing.Point(6, 65);
            this.BosBloodFuryCheckBox.Name = "BosBloodFuryCheckBox";
            this.BosBloodFuryCheckBox.Size = new System.Drawing.Size(169, 17);
            this.BosBloodFuryCheckBox.TabIndex = 2;
            this.BosBloodFuryCheckBox.Text = "Use with Breath of Sindragosa";
            this.BosBloodFuryCheckBox.UseVisualStyleBackColor = true;
            this.BosBloodFuryCheckBox.CheckedChanged += new System.EventHandler(this.BosBloodFuryCheckBox_CheckedChanged);
            // 
            // UseBloodFuryCheckBox
            // 
            this.UseBloodFuryCheckBox.AutoSize = true;
            this.UseBloodFuryCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseBloodFuryCheckBox.Name = "UseBloodFuryCheckBox";
            this.UseBloodFuryCheckBox.Size = new System.Drawing.Size(98, 17);
            this.UseBloodFuryCheckBox.TabIndex = 1;
            this.UseBloodFuryCheckBox.Text = "Use Blood Fury";
            this.UseBloodFuryCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.numWarStompEnemies);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.UseWarStompCheckBox);
            this.groupBox6.Location = new System.Drawing.Point(291, 190);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(275, 90);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tauren";
            // 
            // numWarStompEnemies
            // 
            this.numWarStompEnemies.Location = new System.Drawing.Point(62, 38);
            this.numWarStompEnemies.Name = "numWarStompEnemies";
            this.numWarStompEnemies.Size = new System.Drawing.Size(45, 20);
            this.numWarStompEnemies.TabIndex = 2;
            this.numWarStompEnemies.ValueChanged += new System.EventHandler(this.numWarStompEnemies_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enemies:";
            // 
            // UseWarStompCheckBox
            // 
            this.UseWarStompCheckBox.AutoSize = true;
            this.UseWarStompCheckBox.Location = new System.Drawing.Point(6, 20);
            this.UseWarStompCheckBox.Name = "UseWarStompCheckBox";
            this.UseWarStompCheckBox.Size = new System.Drawing.Size(101, 17);
            this.UseWarStompCheckBox.TabIndex = 0;
            this.UseWarStompCheckBox.Text = "Use War Stomp";
            this.UseWarStompCheckBox.UseVisualStyleBackColor = true;
            this.UseWarStompCheckBox.CheckedChanged += new System.EventHandler(this.UseWarStompCheckBox_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.BosArcaneTorrentCheckBox);
            this.groupBox5.Controls.Add(this.UseArcaneTorrentCheckBox);
            this.groupBox5.Location = new System.Drawing.Point(8, 185);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(275, 90);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Blood Elf";
            // 
            // BosArcaneTorrentCheckBox
            // 
            this.BosArcaneTorrentCheckBox.AutoSize = true;
            this.BosArcaneTorrentCheckBox.Location = new System.Drawing.Point(6, 43);
            this.BosArcaneTorrentCheckBox.Name = "BosArcaneTorrentCheckBox";
            this.BosArcaneTorrentCheckBox.Size = new System.Drawing.Size(169, 17);
            this.BosArcaneTorrentCheckBox.TabIndex = 1;
            this.BosArcaneTorrentCheckBox.Text = "Use with Breath of Sindragosa";
            this.BosArcaneTorrentCheckBox.UseVisualStyleBackColor = true;
            this.BosArcaneTorrentCheckBox.CheckedChanged += new System.EventHandler(this.BosArcaneTorrentCheckBox_CheckedChanged);
            // 
            // UseArcaneTorrentCheckBox
            // 
            this.UseArcaneTorrentCheckBox.AutoSize = true;
            this.UseArcaneTorrentCheckBox.Location = new System.Drawing.Point(6, 20);
            this.UseArcaneTorrentCheckBox.Name = "UseArcaneTorrentCheckBox";
            this.UseArcaneTorrentCheckBox.Size = new System.Drawing.Size(119, 17);
            this.UseArcaneTorrentCheckBox.TabIndex = 0;
            this.UseArcaneTorrentCheckBox.Text = "Use Arcane Torrent";
            this.UseArcaneTorrentCheckBox.UseVisualStyleBackColor = true;
            this.UseArcaneTorrentCheckBox.CheckedChanged += new System.EventHandler(this.UseArcaneTorrentCheckBox_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numStoneformHp);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.UseStoneformOnlyToClearCheckBox);
            this.groupBox4.Controls.Add(this.UseStoneformCheckBox);
            this.groupBox4.Location = new System.Drawing.Point(291, 94);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(275, 90);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Dwarf";
            // 
            // numStoneformHp
            // 
            this.numStoneformHp.Location = new System.Drawing.Point(47, 41);
            this.numStoneformHp.Name = "numStoneformHp";
            this.numStoneformHp.Size = new System.Drawing.Size(85, 20);
            this.numStoneformHp.TabIndex = 9;
            this.numStoneformHp.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Hp% :";
            // 
            // UseStoneformOnlyToClearCheckBox
            // 
            this.UseStoneformOnlyToClearCheckBox.AutoSize = true;
            this.UseStoneformOnlyToClearCheckBox.Location = new System.Drawing.Point(6, 66);
            this.UseStoneformOnlyToClearCheckBox.Name = "UseStoneformOnlyToClearCheckBox";
            this.UseStoneformOnlyToClearCheckBox.Size = new System.Drawing.Size(177, 17);
            this.UseStoneformOnlyToClearCheckBox.TabIndex = 7;
            this.UseStoneformOnlyToClearCheckBox.Text = "Use only to clear harmful effects";
            this.UseStoneformOnlyToClearCheckBox.UseVisualStyleBackColor = true;
            this.UseStoneformOnlyToClearCheckBox.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // UseStoneformCheckBox
            // 
            this.UseStoneformCheckBox.AutoSize = true;
            this.UseStoneformCheckBox.Location = new System.Drawing.Point(6, 20);
            this.UseStoneformCheckBox.Name = "UseStoneformCheckBox";
            this.UseStoneformCheckBox.Size = new System.Drawing.Size(96, 17);
            this.UseStoneformCheckBox.TabIndex = 6;
            this.UseStoneformCheckBox.Text = "Use Stoneform";
            this.UseStoneformCheckBox.UseVisualStyleBackColor = true;
            this.UseStoneformCheckBox.CheckedChanged += new System.EventHandler(this.UseStoneformCheckBox_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.UseGiftoftheNaruuCheckBox);
            this.groupBox3.Controls.Add(this.numGiftOfTheNaaruHp);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(8, 94);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(275, 90);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Draenei";
            // 
            // UseGiftoftheNaruuCheckBox
            // 
            this.UseGiftoftheNaruuCheckBox.AutoSize = true;
            this.UseGiftoftheNaruuCheckBox.Location = new System.Drawing.Point(6, 20);
            this.UseGiftoftheNaruuCheckBox.Name = "UseGiftoftheNaruuCheckBox";
            this.UseGiftoftheNaruuCheckBox.Size = new System.Drawing.Size(126, 17);
            this.UseGiftoftheNaruuCheckBox.TabIndex = 8;
            this.UseGiftoftheNaruuCheckBox.Text = "Use Gift of the Naaru";
            this.UseGiftoftheNaruuCheckBox.UseVisualStyleBackColor = true;
            this.UseGiftoftheNaruuCheckBox.CheckedChanged += new System.EventHandler(this.UseGiftoftheNaruuCheckBox_CheckedChanged);
            // 
            // numGiftOfTheNaaruHp
            // 
            this.numGiftOfTheNaaruHp.Location = new System.Drawing.Point(47, 40);
            this.numGiftOfTheNaaruHp.Name = "numGiftOfTheNaaruHp";
            this.numGiftOfTheNaaruHp.Size = new System.Drawing.Size(85, 20);
            this.numGiftOfTheNaaruHp.TabIndex = 7;
            this.numGiftOfTheNaaruHp.ValueChanged += new System.EventHandler(this.numGiftOfTheNaaruHp_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Hp% :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.UseHumanRacialCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(291, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 90);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Human";
            // 
            // UseHumanRacialCheckBox
            // 
            this.UseHumanRacialCheckBox.AutoSize = true;
            this.UseHumanRacialCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseHumanRacialCheckBox.Name = "UseHumanRacialCheckBox";
            this.UseHumanRacialCheckBox.Size = new System.Drawing.Size(151, 17);
            this.UseHumanRacialCheckBox.TabIndex = 3;
            this.UseHumanRacialCheckBox.Text = "Use Every Man for Himself";
            this.UseHumanRacialCheckBox.UseVisualStyleBackColor = true;
            this.UseHumanRacialCheckBox.CheckedChanged += new System.EventHandler(this.UseHumanRacialCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.UseRacialsCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(8, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // UseRacialsCheckBox
            // 
            this.UseRacialsCheckBox.AutoSize = true;
            this.UseRacialsCheckBox.Checked = true;
            this.UseRacialsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseRacialsCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseRacialsCheckBox.Name = "UseRacialsCheckBox";
            this.UseRacialsCheckBox.Size = new System.Drawing.Size(83, 17);
            this.UseRacialsCheckBox.TabIndex = 0;
            this.UseRacialsCheckBox.Text = "Use Racials";
            this.UseRacialsCheckBox.UseVisualStyleBackColor = true;
            this.UseRacialsCheckBox.CheckedChanged += new System.EventHandler(this.UseRacialsCheckBox_CheckedChanged);
            // 
            // Trinkets
            // 
            this.Trinkets.Controls.Add(this.groupBox9);
            this.Trinkets.Controls.Add(this.groupBox8);
            this.Trinkets.Location = new System.Drawing.Point(4, 22);
            this.Trinkets.Name = "Trinkets";
            this.Trinkets.Size = new System.Drawing.Size(575, 504);
            this.Trinkets.TabIndex = 6;
            this.Trinkets.Text = "Trinkets";
            this.Trinkets.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.UseTrinket2OnBoS);
            this.groupBox9.Controls.Add(this.numTrinket2EnemyHealth);
            this.groupBox9.Controls.Add(this.numTrinket2MyHealth);
            this.groupBox9.Controls.Add(this.label7);
            this.groupBox9.Controls.Add(this.label8);
            this.groupBox9.Controls.Add(this.label9);
            this.groupBox9.Controls.Add(this.T2OnEnemyHealthBelowCheckBox);
            this.groupBox9.Controls.Add(this.T2OnMyHealthBelowCheckBox);
            this.groupBox9.Controls.Add(this.T2OnBurstCheckBox);
            this.groupBox9.Controls.Add(this.T2OnCdCheckBox);
            this.groupBox9.Controls.Add(this.T2OnLoCCheckBox);
            this.groupBox9.Controls.Add(this.UseTrinket2CheckBox);
            this.groupBox9.Location = new System.Drawing.Point(291, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(275, 408);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Trinket 2";
            this.groupBox9.Enter += new System.EventHandler(this.groupBox9_Enter);
            // 
            // UseTrinket2OnBoS
            // 
            this.UseTrinket2OnBoS.AutoSize = true;
            this.UseTrinket2OnBoS.Location = new System.Drawing.Point(5, 174);
            this.UseTrinket2OnBoS.Name = "UseTrinket2OnBoS";
            this.UseTrinket2OnBoS.Size = new System.Drawing.Size(162, 17);
            this.UseTrinket2OnBoS.TabIndex = 25;
            this.UseTrinket2OnBoS.Text = "Use on Breath of Sindragosa\r\n";
            this.UseTrinket2OnBoS.UseVisualStyleBackColor = true;
            this.UseTrinket2OnBoS.CheckedChanged += new System.EventHandler(this.UseTrinket2OnBoS_CheckedChanged);
            // 
            // numTrinket2EnemyHealth
            // 
            this.numTrinket2EnemyHealth.Location = new System.Drawing.Point(165, 150);
            this.numTrinket2EnemyHealth.Name = "numTrinket2EnemyHealth";
            this.numTrinket2EnemyHealth.Size = new System.Drawing.Size(39, 20);
            this.numTrinket2EnemyHealth.TabIndex = 24;
            this.numTrinket2EnemyHealth.ValueChanged += new System.EventHandler(this.numTrinket2EnemyHealth_ValueChanged);
            // 
            // numTrinket2MyHealth
            // 
            this.numTrinket2MyHealth.Location = new System.Drawing.Point(165, 127);
            this.numTrinket2MyHealth.Name = "numTrinket2MyHealth";
            this.numTrinket2MyHealth.Size = new System.Drawing.Size(39, 20);
            this.numTrinket2MyHealth.TabIndex = 23;
            this.numTrinket2MyHealth.ValueChanged += new System.EventHandler(this.numTrinket2MyHealth_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(124, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Hp% :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(124, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Hp% :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Use Trinket 2 on:";
            // 
            // T2OnEnemyHealthBelowCheckBox
            // 
            this.T2OnEnemyHealthBelowCheckBox.AutoSize = true;
            this.T2OnEnemyHealthBelowCheckBox.Location = new System.Drawing.Point(5, 151);
            this.T2OnEnemyHealthBelowCheckBox.Name = "T2OnEnemyHealthBelowCheckBox";
            this.T2OnEnemyHealthBelowCheckBox.Size = new System.Drawing.Size(121, 17);
            this.T2OnEnemyHealthBelowCheckBox.TabIndex = 19;
            this.T2OnEnemyHealthBelowCheckBox.Text = "Enemy health below";
            this.T2OnEnemyHealthBelowCheckBox.UseVisualStyleBackColor = true;
            this.T2OnEnemyHealthBelowCheckBox.CheckedChanged += new System.EventHandler(this.T2OnEnemyHealthBelowCheckBox_CheckedChanged);
            // 
            // T2OnMyHealthBelowCheckBox
            // 
            this.T2OnMyHealthBelowCheckBox.AutoSize = true;
            this.T2OnMyHealthBelowCheckBox.Location = new System.Drawing.Point(6, 128);
            this.T2OnMyHealthBelowCheckBox.Name = "T2OnMyHealthBelowCheckBox";
            this.T2OnMyHealthBelowCheckBox.Size = new System.Drawing.Size(103, 17);
            this.T2OnMyHealthBelowCheckBox.TabIndex = 18;
            this.T2OnMyHealthBelowCheckBox.Text = "My health below";
            this.T2OnMyHealthBelowCheckBox.UseVisualStyleBackColor = true;
            this.T2OnMyHealthBelowCheckBox.CheckedChanged += new System.EventHandler(this.T2OnMyHealthBelowCheckBox_CheckedChanged);
            // 
            // T2OnBurstCheckBox
            // 
            this.T2OnBurstCheckBox.AutoSize = true;
            this.T2OnBurstCheckBox.Location = new System.Drawing.Point(6, 105);
            this.T2OnBurstCheckBox.Name = "T2OnBurstCheckBox";
            this.T2OnBurstCheckBox.Size = new System.Drawing.Size(50, 17);
            this.T2OnBurstCheckBox.TabIndex = 17;
            this.T2OnBurstCheckBox.Text = "Burst";
            this.T2OnBurstCheckBox.UseVisualStyleBackColor = true;
            this.T2OnBurstCheckBox.CheckedChanged += new System.EventHandler(this.T2OnBurstCheckBox_CheckedChanged);
            // 
            // T2OnCdCheckBox
            // 
            this.T2OnCdCheckBox.AutoSize = true;
            this.T2OnCdCheckBox.Location = new System.Drawing.Point(6, 82);
            this.T2OnCdCheckBox.Name = "T2OnCdCheckBox";
            this.T2OnCdCheckBox.Size = new System.Drawing.Size(73, 17);
            this.T2OnCdCheckBox.TabIndex = 15;
            this.T2OnCdCheckBox.Text = "Cooldown";
            this.T2OnCdCheckBox.UseVisualStyleBackColor = true;
            this.T2OnCdCheckBox.CheckedChanged += new System.EventHandler(this.T2OnCdCheckBox_CheckedChanged);
            // 
            // T2OnLoCCheckBox
            // 
            this.T2OnLoCCheckBox.AutoSize = true;
            this.T2OnLoCCheckBox.Location = new System.Drawing.Point(6, 59);
            this.T2OnLoCCheckBox.Name = "T2OnLoCCheckBox";
            this.T2OnLoCCheckBox.Size = new System.Drawing.Size(96, 17);
            this.T2OnLoCCheckBox.TabIndex = 14;
            this.T2OnLoCCheckBox.Text = "Loss of Control";
            this.T2OnLoCCheckBox.UseVisualStyleBackColor = true;
            this.T2OnLoCCheckBox.CheckedChanged += new System.EventHandler(this.T2OnLoCCheckBox_CheckedChanged);
            // 
            // UseTrinket2CheckBox
            // 
            this.UseTrinket2CheckBox.AutoSize = true;
            this.UseTrinket2CheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseTrinket2CheckBox.Name = "UseTrinket2CheckBox";
            this.UseTrinket2CheckBox.Size = new System.Drawing.Size(87, 17);
            this.UseTrinket2CheckBox.TabIndex = 13;
            this.UseTrinket2CheckBox.Text = "UseTrinket 2";
            this.UseTrinket2CheckBox.UseVisualStyleBackColor = true;
            this.UseTrinket2CheckBox.CheckedChanged += new System.EventHandler(this.UseTrinket2CheckBox_CheckedChanged);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.UseTrinket1OnBoS);
            this.groupBox8.Controls.Add(this.numTrinket1EnemyHealth);
            this.groupBox8.Controls.Add(this.numTrinket1MyHealth);
            this.groupBox8.Controls.Add(this.label6);
            this.groupBox8.Controls.Add(this.label5);
            this.groupBox8.Controls.Add(this.label4);
            this.groupBox8.Controls.Add(this.T1OnEnemyHealthBelowCheckBox);
            this.groupBox8.Controls.Add(this.T1OnMyHealthBelowCheckBox);
            this.groupBox8.Controls.Add(this.T1OnBurstCheckBox);
            this.groupBox8.Controls.Add(this.T1OnCdCheckBox);
            this.groupBox8.Controls.Add(this.T1OnLoCCheckBox);
            this.groupBox8.Controls.Add(this.UseTrinket1CheckBox);
            this.groupBox8.Location = new System.Drawing.Point(8, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(275, 409);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Trinket 1";
            this.groupBox8.Enter += new System.EventHandler(this.groupBox8_Enter);
            // 
            // UseTrinket1OnBoS
            // 
            this.UseTrinket1OnBoS.AutoSize = true;
            this.UseTrinket1OnBoS.Location = new System.Drawing.Point(5, 175);
            this.UseTrinket1OnBoS.Name = "UseTrinket1OnBoS";
            this.UseTrinket1OnBoS.Size = new System.Drawing.Size(162, 17);
            this.UseTrinket1OnBoS.TabIndex = 13;
            this.UseTrinket1OnBoS.Text = "Use on Breath of Sindragosa";
            this.UseTrinket1OnBoS.UseVisualStyleBackColor = true;
            this.UseTrinket1OnBoS.CheckedChanged += new System.EventHandler(this.UseTrinket1OnBoS_CheckedChanged);
            // 
            // numTrinket1EnemyHealth
            // 
            this.numTrinket1EnemyHealth.Location = new System.Drawing.Point(165, 151);
            this.numTrinket1EnemyHealth.Name = "numTrinket1EnemyHealth";
            this.numTrinket1EnemyHealth.Size = new System.Drawing.Size(39, 20);
            this.numTrinket1EnemyHealth.TabIndex = 12;
            this.numTrinket1EnemyHealth.ValueChanged += new System.EventHandler(this.numTrinket1EnemyHealth_ValueChanged);
            // 
            // numTrinket1MyHealth
            // 
            this.numTrinket1MyHealth.Location = new System.Drawing.Point(165, 128);
            this.numTrinket1MyHealth.Name = "numTrinket1MyHealth";
            this.numTrinket1MyHealth.Size = new System.Drawing.Size(39, 20);
            this.numTrinket1MyHealth.TabIndex = 11;
            this.numTrinket1MyHealth.ValueChanged += new System.EventHandler(this.numTrinket1MyHealth_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(124, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Hp% :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(124, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Hp% :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Use Trinket 1 on:";
            // 
            // T1OnEnemyHealthBelowCheckBox
            // 
            this.T1OnEnemyHealthBelowCheckBox.AutoSize = true;
            this.T1OnEnemyHealthBelowCheckBox.Location = new System.Drawing.Point(5, 152);
            this.T1OnEnemyHealthBelowCheckBox.Name = "T1OnEnemyHealthBelowCheckBox";
            this.T1OnEnemyHealthBelowCheckBox.Size = new System.Drawing.Size(121, 17);
            this.T1OnEnemyHealthBelowCheckBox.TabIndex = 7;
            this.T1OnEnemyHealthBelowCheckBox.Text = "Enemy health below";
            this.T1OnEnemyHealthBelowCheckBox.UseVisualStyleBackColor = true;
            this.T1OnEnemyHealthBelowCheckBox.CheckedChanged += new System.EventHandler(this.T1OnEnemyHealthBelowCheckBox_CheckedChanged);
            // 
            // T1OnMyHealthBelowCheckBox
            // 
            this.T1OnMyHealthBelowCheckBox.AutoSize = true;
            this.T1OnMyHealthBelowCheckBox.Location = new System.Drawing.Point(6, 129);
            this.T1OnMyHealthBelowCheckBox.Name = "T1OnMyHealthBelowCheckBox";
            this.T1OnMyHealthBelowCheckBox.Size = new System.Drawing.Size(103, 17);
            this.T1OnMyHealthBelowCheckBox.TabIndex = 6;
            this.T1OnMyHealthBelowCheckBox.Text = "My health below";
            this.T1OnMyHealthBelowCheckBox.UseVisualStyleBackColor = true;
            this.T1OnMyHealthBelowCheckBox.CheckedChanged += new System.EventHandler(this.T1OnMyHealthBelowCheckBox_CheckedChanged);
            // 
            // T1OnBurstCheckBox
            // 
            this.T1OnBurstCheckBox.AutoSize = true;
            this.T1OnBurstCheckBox.Location = new System.Drawing.Point(6, 106);
            this.T1OnBurstCheckBox.Name = "T1OnBurstCheckBox";
            this.T1OnBurstCheckBox.Size = new System.Drawing.Size(50, 17);
            this.T1OnBurstCheckBox.TabIndex = 5;
            this.T1OnBurstCheckBox.Text = "Burst";
            this.T1OnBurstCheckBox.UseVisualStyleBackColor = true;
            this.T1OnBurstCheckBox.CheckedChanged += new System.EventHandler(this.T1OnBurstCheckBox_CheckedChanged);
            // 
            // T1OnCdCheckBox
            // 
            this.T1OnCdCheckBox.AutoSize = true;
            this.T1OnCdCheckBox.Location = new System.Drawing.Point(6, 83);
            this.T1OnCdCheckBox.Name = "T1OnCdCheckBox";
            this.T1OnCdCheckBox.Size = new System.Drawing.Size(73, 17);
            this.T1OnCdCheckBox.TabIndex = 3;
            this.T1OnCdCheckBox.Text = "Cooldown";
            this.T1OnCdCheckBox.UseVisualStyleBackColor = true;
            this.T1OnCdCheckBox.CheckedChanged += new System.EventHandler(this.T1OnCdCheckBox_CheckedChanged);
            // 
            // T1OnLoCCheckBox
            // 
            this.T1OnLoCCheckBox.AutoSize = true;
            this.T1OnLoCCheckBox.Location = new System.Drawing.Point(6, 59);
            this.T1OnLoCCheckBox.Name = "T1OnLoCCheckBox";
            this.T1OnLoCCheckBox.Size = new System.Drawing.Size(96, 17);
            this.T1OnLoCCheckBox.TabIndex = 2;
            this.T1OnLoCCheckBox.Text = "Loss of Control";
            this.T1OnLoCCheckBox.UseVisualStyleBackColor = true;
            this.T1OnLoCCheckBox.CheckedChanged += new System.EventHandler(this.T1OnLoCCheckBox_CheckedChanged);
            // 
            // UseTrinket1CheckBox
            // 
            this.UseTrinket1CheckBox.AutoSize = true;
            this.UseTrinket1CheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseTrinket1CheckBox.Name = "UseTrinket1CheckBox";
            this.UseTrinket1CheckBox.Size = new System.Drawing.Size(87, 17);
            this.UseTrinket1CheckBox.TabIndex = 1;
            this.UseTrinket1CheckBox.Text = "UseTrinket 1";
            this.UseTrinket1CheckBox.UseVisualStyleBackColor = true;
            this.UseTrinket1CheckBox.CheckedChanged += new System.EventHandler(this.UseTrinket1CheckBox_CheckedChanged);
            // 
            // Advanced
            // 
            this.Advanced.Controls.Add(this.groupBox19);
            this.Advanced.Controls.Add(this.groupBox18);
            this.Advanced.Controls.Add(this.groupBox17);
            this.Advanced.Controls.Add(this.groupBox12);
            this.Advanced.Location = new System.Drawing.Point(4, 22);
            this.Advanced.Name = "Advanced";
            this.Advanced.Size = new System.Drawing.Size(575, 504);
            this.Advanced.TabIndex = 8;
            this.Advanced.Text = "Advanced";
            this.Advanced.UseVisualStyleBackColor = true;
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.numStrangulatemax);
            this.groupBox19.Controls.Add(this.numStrangulatemin);
            this.groupBox19.Controls.Add(this.label18);
            this.groupBox19.Controls.Add(this.label19);
            this.groupBox19.Controls.Add(this.DelayStrangulateCheckBox);
            this.groupBox19.Controls.Add(this.StrangulateCheckBox);
            this.groupBox19.Controls.Add(this.numMFmax);
            this.groupBox19.Controls.Add(this.numMFmin);
            this.groupBox19.Controls.Add(this.label17);
            this.groupBox19.Controls.Add(this.label16);
            this.groupBox19.Controls.Add(this.DelayMFcheckBox);
            this.groupBox19.Controls.Add(this.UseMFcheckBox);
            this.groupBox19.Location = new System.Drawing.Point(10, 130);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(274, 220);
            this.groupBox19.TabIndex = 3;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "Interrupts";
            // 
            // numStrangulatemax
            // 
            this.numStrangulatemax.Location = new System.Drawing.Point(67, 189);
            this.numStrangulatemax.Name = "numStrangulatemax";
            this.numStrangulatemax.Size = new System.Drawing.Size(46, 20);
            this.numStrangulatemax.TabIndex = 18;
            this.numStrangulatemax.ValueChanged += new System.EventHandler(this.numStrangulatemax_ValueChanged);
            // 
            // numStrangulatemin
            // 
            this.numStrangulatemin.Location = new System.Drawing.Point(67, 163);
            this.numStrangulatemin.Name = "numStrangulatemin";
            this.numStrangulatemin.Size = new System.Drawing.Size(46, 20);
            this.numStrangulatemin.TabIndex = 17;
            this.numStrangulatemin.ValueChanged += new System.EventHandler(this.numStrangulatemin_ValueChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 191);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 16;
            this.label18.Text = "Max %";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 167);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(59, 13);
            this.label19.TabIndex = 15;
            this.label19.Text = "Minimum %";
            // 
            // DelayStrangulateCheckBox
            // 
            this.DelayStrangulateCheckBox.AutoSize = true;
            this.DelayStrangulateCheckBox.Location = new System.Drawing.Point(6, 140);
            this.DelayStrangulateCheckBox.Name = "DelayStrangulateCheckBox";
            this.DelayStrangulateCheckBox.Size = new System.Drawing.Size(110, 17);
            this.DelayStrangulateCheckBox.TabIndex = 14;
            this.DelayStrangulateCheckBox.Text = "Delay Strangulate";
            this.DelayStrangulateCheckBox.UseVisualStyleBackColor = true;
            this.DelayStrangulateCheckBox.CheckedChanged += new System.EventHandler(this.DelayStrangulateCheckBox_CheckedChanged);
            // 
            // StrangulateCheckBox
            // 
            this.StrangulateCheckBox.AutoSize = true;
            this.StrangulateCheckBox.Location = new System.Drawing.Point(6, 117);
            this.StrangulateCheckBox.Name = "StrangulateCheckBox";
            this.StrangulateCheckBox.Size = new System.Drawing.Size(102, 17);
            this.StrangulateCheckBox.TabIndex = 13;
            this.StrangulateCheckBox.Text = "Use Strangulate";
            this.StrangulateCheckBox.UseVisualStyleBackColor = true;
            this.StrangulateCheckBox.CheckedChanged += new System.EventHandler(this.StrangulateCheckBox_CheckedChanged);
            // 
            // numMFmax
            // 
            this.numMFmax.Location = new System.Drawing.Point(67, 91);
            this.numMFmax.Name = "numMFmax";
            this.numMFmax.Size = new System.Drawing.Size(46, 20);
            this.numMFmax.TabIndex = 12;
            this.numMFmax.ValueChanged += new System.EventHandler(this.numMFmax_ValueChanged);
            // 
            // numMFmin
            // 
            this.numMFmin.Location = new System.Drawing.Point(67, 65);
            this.numMFmin.Name = "numMFmin";
            this.numMFmin.Size = new System.Drawing.Size(46, 20);
            this.numMFmin.TabIndex = 11;
            this.numMFmin.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged_3);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 93);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(38, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Max %";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 69);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Minimum %";
            // 
            // DelayMFcheckBox
            // 
            this.DelayMFcheckBox.AutoSize = true;
            this.DelayMFcheckBox.Location = new System.Drawing.Point(6, 42);
            this.DelayMFcheckBox.Name = "DelayMFcheckBox";
            this.DelayMFcheckBox.Size = new System.Drawing.Size(114, 17);
            this.DelayMFcheckBox.TabIndex = 1;
            this.DelayMFcheckBox.Text = "Delay Mind Freeze";
            this.DelayMFcheckBox.UseVisualStyleBackColor = true;
            this.DelayMFcheckBox.CheckedChanged += new System.EventHandler(this.DelayMFcheckBox_CheckedChanged);
            // 
            // UseMFcheckBox
            // 
            this.UseMFcheckBox.AutoSize = true;
            this.UseMFcheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseMFcheckBox.Name = "UseMFcheckBox";
            this.UseMFcheckBox.Size = new System.Drawing.Size(106, 17);
            this.UseMFcheckBox.TabIndex = 0;
            this.UseMFcheckBox.Text = "Use Mind Freeze";
            this.UseMFcheckBox.UseVisualStyleBackColor = true;
            this.UseMFcheckBox.CheckedChanged += new System.EventHandler(this.UseMFcheckBox_CheckedChanged);
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.numHealthstoneUseHp);
            this.groupBox18.Controls.Add(this.label13);
            this.groupBox18.Controls.Add(this.UseHealthstonecheckBox);
            this.groupBox18.Controls.Add(this.numHealingTonicHp);
            this.groupBox18.Controls.Add(this.label12);
            this.groupBox18.Controls.Add(this.useHealingToniccheckBox);
            this.groupBox18.Location = new System.Drawing.Point(290, 130);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(276, 152);
            this.groupBox18.TabIndex = 2;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "Items";
            // 
            // numHealthstoneUseHp
            // 
            this.numHealthstoneUseHp.Location = new System.Drawing.Point(59, 91);
            this.numHealthstoneUseHp.Name = "numHealthstoneUseHp";
            this.numHealthstoneUseHp.Size = new System.Drawing.Size(46, 20);
            this.numHealthstoneUseHp.TabIndex = 11;
            this.numHealthstoneUseHp.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numHealthstoneUseHp.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged_1);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 93);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 13);
            this.label13.TabIndex = 10;
            this.label13.Text = "Hp% :";
            // 
            // UseHealthstonecheckBox
            // 
            this.UseHealthstonecheckBox.AutoSize = true;
            this.UseHealthstonecheckBox.Location = new System.Drawing.Point(6, 68);
            this.UseHealthstonecheckBox.Name = "UseHealthstonecheckBox";
            this.UseHealthstonecheckBox.Size = new System.Drawing.Size(105, 17);
            this.UseHealthstonecheckBox.TabIndex = 9;
            this.UseHealthstonecheckBox.Text = "Use Healthstone";
            this.UseHealthstonecheckBox.UseVisualStyleBackColor = true;
            this.UseHealthstonecheckBox.CheckedChanged += new System.EventHandler(this.UseHealthstonecheckBox_CheckedChanged);
            // 
            // numHealingTonicHp
            // 
            this.numHealingTonicHp.Location = new System.Drawing.Point(59, 42);
            this.numHealingTonicHp.Name = "numHealingTonicHp";
            this.numHealingTonicHp.Size = new System.Drawing.Size(46, 20);
            this.numHealingTonicHp.TabIndex = 8;
            this.numHealingTonicHp.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numHealingTonicHp.ValueChanged += new System.EventHandler(this.numHealingTonicHp_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Hp% :";
            // 
            // useHealingToniccheckBox
            // 
            this.useHealingToniccheckBox.AutoSize = true;
            this.useHealingToniccheckBox.Location = new System.Drawing.Point(6, 19);
            this.useHealingToniccheckBox.Name = "useHealingToniccheckBox";
            this.useHealingToniccheckBox.Size = new System.Drawing.Size(114, 17);
            this.useHealingToniccheckBox.TabIndex = 0;
            this.useHealingToniccheckBox.Text = "Use Healing Tonic";
            this.useHealingToniccheckBox.UseVisualStyleBackColor = true;
            this.useHealingToniccheckBox.CheckedChanged += new System.EventHandler(this.useHealingToniccheckBox_CheckedChanged);
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.numDeathSiphonHp);
            this.groupBox17.Controls.Add(this.label15);
            this.groupBox17.Controls.Add(this.numDeathStrikeHp);
            this.groupBox17.Controls.Add(this.label14);
            this.groupBox17.Controls.Add(this.DeathSiphoncheckBox);
            this.groupBox17.Controls.Add(this.DeathStrikecheckBox);
            this.groupBox17.Location = new System.Drawing.Point(290, 3);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(276, 121);
            this.groupBox17.TabIndex = 1;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Survivability";
            // 
            // numDeathSiphonHp
            // 
            this.numDeathSiphonHp.Location = new System.Drawing.Point(47, 91);
            this.numDeathSiphonHp.Name = "numDeathSiphonHp";
            this.numDeathSiphonHp.Size = new System.Drawing.Size(46, 20);
            this.numDeathSiphonHp.TabIndex = 12;
            this.numDeathSiphonHp.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numDeathSiphonHp.ValueChanged += new System.EventHandler(this.numDeathSiphonHp_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 95);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Hp% :";
            // 
            // numDeathStrikeHp
            // 
            this.numDeathStrikeHp.Location = new System.Drawing.Point(47, 42);
            this.numDeathStrikeHp.Name = "numDeathStrikeHp";
            this.numDeathStrikeHp.Size = new System.Drawing.Size(46, 20);
            this.numDeathStrikeHp.TabIndex = 10;
            this.numDeathStrikeHp.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numDeathStrikeHp.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged_2);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 44);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 9;
            this.label14.Text = "Hp% :";
            // 
            // DeathSiphoncheckBox
            // 
            this.DeathSiphoncheckBox.AutoSize = true;
            this.DeathSiphoncheckBox.Location = new System.Drawing.Point(6, 68);
            this.DeathSiphoncheckBox.Name = "DeathSiphoncheckBox";
            this.DeathSiphoncheckBox.Size = new System.Drawing.Size(113, 17);
            this.DeathSiphoncheckBox.TabIndex = 2;
            this.DeathSiphoncheckBox.Text = "Use Death Siphon";
            this.DeathSiphoncheckBox.UseVisualStyleBackColor = true;
            this.DeathSiphoncheckBox.CheckedChanged += new System.EventHandler(this.DeathSiphoncheckBox_CheckedChanged);
            // 
            // DeathStrikecheckBox
            // 
            this.DeathStrikecheckBox.AutoSize = true;
            this.DeathStrikecheckBox.Location = new System.Drawing.Point(6, 19);
            this.DeathStrikecheckBox.Name = "DeathStrikecheckBox";
            this.DeathStrikecheckBox.Size = new System.Drawing.Size(107, 17);
            this.DeathStrikecheckBox.TabIndex = 0;
            this.DeathStrikecheckBox.Text = "Use Death Strike";
            this.DeathStrikecheckBox.UseVisualStyleBackColor = true;
            this.DeathStrikecheckBox.CheckedChanged += new System.EventHandler(this.DeathStrikecheckBox_CheckedChanged);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label11);
            this.groupBox12.Controls.Add(this.checkBox1);
            this.groupBox12.Controls.Add(this.UseDeathGripInPvPCheckBox);
            this.groupBox12.Controls.Add(this.UseDeathGripCheckBox);
            this.groupBox12.Location = new System.Drawing.Point(8, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(276, 121);
            this.groupBox12.TabIndex = 0;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Death Grip";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 39);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Use Conditions:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(6, 78);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(210, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "When target is not within Melee Range";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // UseDeathGripInPvPCheckBox
            // 
            this.UseDeathGripInPvPCheckBox.AutoSize = true;
            this.UseDeathGripInPvPCheckBox.Enabled = false;
            this.UseDeathGripInPvPCheckBox.Location = new System.Drawing.Point(6, 55);
            this.UseDeathGripInPvPCheckBox.Name = "UseDeathGripInPvPCheckBox";
            this.UseDeathGripInPvPCheckBox.Size = new System.Drawing.Size(58, 17);
            this.UseDeathGripInPvPCheckBox.TabIndex = 1;
            this.UseDeathGripInPvPCheckBox.Text = "In PvP";
            this.UseDeathGripInPvPCheckBox.UseVisualStyleBackColor = true;
            this.UseDeathGripInPvPCheckBox.CheckedChanged += new System.EventHandler(this.UseDeathGripInPvPCheckBox_CheckedChanged);
            // 
            // UseDeathGripCheckBox
            // 
            this.UseDeathGripCheckBox.AutoSize = true;
            this.UseDeathGripCheckBox.Location = new System.Drawing.Point(6, 19);
            this.UseDeathGripCheckBox.Name = "UseDeathGripCheckBox";
            this.UseDeathGripCheckBox.Size = new System.Drawing.Size(99, 17);
            this.UseDeathGripCheckBox.TabIndex = 0;
            this.UseDeathGripCheckBox.Text = "Use Death Grip";
            this.UseDeathGripCheckBox.UseVisualStyleBackColor = true;
            this.UseDeathGripCheckBox.CheckedChanged += new System.EventHandler(this.UseDeathGripCheckBox_CheckedChanged);
            // 
            // Debug
            // 
            this.Debug.Controls.Add(this.DumpSpellsButton);
            this.Debug.Location = new System.Drawing.Point(4, 22);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(575, 504);
            this.Debug.TabIndex = 3;
            this.Debug.Text = "Debug";
            this.Debug.UseVisualStyleBackColor = true;
            // 
            // DumpSpellsButton
            // 
            this.DumpSpellsButton.Location = new System.Drawing.Point(8, 3);
            this.DumpSpellsButton.Name = "DumpSpellsButton";
            this.DumpSpellsButton.Size = new System.Drawing.Size(75, 23);
            this.DumpSpellsButton.TabIndex = 0;
            this.DumpSpellsButton.Text = "Dump spells";
            this.DumpSpellsButton.UseVisualStyleBackColor = true;
            this.DumpSpellsButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // DungeonBuddyPresetButton
            // 
            this.DungeonBuddyPresetButton.Location = new System.Drawing.Point(206, 18);
            this.DungeonBuddyPresetButton.Name = "DungeonBuddyPresetButton";
            this.DungeonBuddyPresetButton.Size = new System.Drawing.Size(92, 23);
            this.DungeonBuddyPresetButton.TabIndex = 2;
            this.DungeonBuddyPresetButton.Text = "DungeonBuddy";
            this.DungeonBuddyPresetButton.UseVisualStyleBackColor = true;
            this.DungeonBuddyPresetButton.Click += new System.EventHandler(this.DungeonBuddyPresetButton_Click);
            // 
            // RotationOnlyPresetButton
            // 
            this.RotationOnlyPresetButton.Location = new System.Drawing.Point(108, 18);
            this.RotationOnlyPresetButton.Name = "RotationOnlyPresetButton";
            this.RotationOnlyPresetButton.Size = new System.Drawing.Size(92, 23);
            this.RotationOnlyPresetButton.TabIndex = 1;
            this.RotationOnlyPresetButton.Text = "Rotation Only";
            this.RotationOnlyPresetButton.UseVisualStyleBackColor = true;
            this.RotationOnlyPresetButton.Click += new System.EventHandler(this.RotationOnlyPresetButton_Click);
            // 
            // AFKPresetButton
            // 
            this.AFKPresetButton.Location = new System.Drawing.Point(10, 18);
            this.AFKPresetButton.Name = "AFKPresetButton";
            this.AFKPresetButton.Size = new System.Drawing.Size(92, 23);
            this.AFKPresetButton.TabIndex = 0;
            this.AFKPresetButton.Text = "Full AFK";
            this.AFKPresetButton.UseVisualStyleBackColor = true;
            this.AFKPresetButton.Click += new System.EventHandler(this.AFKPresetButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(503, 555);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.Savebutton_Click);
            // 
            // PVPPresetButton
            // 
            this.PVPPresetButton.Location = new System.Drawing.Point(304, 18);
            this.PVPPresetButton.Name = "PVPPresetButton";
            this.PVPPresetButton.Size = new System.Drawing.Size(92, 23);
            this.PVPPresetButton.TabIndex = 3;
            this.PVPPresetButton.Text = "PVP";
            this.PVPPresetButton.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.PVPPresetButton);
            this.groupBox11.Controls.Add(this.AFKPresetButton);
            this.groupBox11.Controls.Add(this.DungeonBuddyPresetButton);
            this.groupBox11.Controls.Add(this.RotationOnlyPresetButton);
            this.groupBox11.Location = new System.Drawing.Point(12, 537);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(485, 48);
            this.groupBox11.TabIndex = 13;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Presets";
            // 
            // Hotkeys
            // 
            this.Hotkeys.Controls.Add(this.groupBox35);
            this.Hotkeys.Location = new System.Drawing.Point(4, 22);
            this.Hotkeys.Name = "Hotkeys";
            this.Hotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.Hotkeys.Size = new System.Drawing.Size(575, 504);
            this.Hotkeys.TabIndex = 10;
            this.Hotkeys.Text = "Hotkeys";
            this.Hotkeys.UseVisualStyleBackColor = true;
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.PauseCrKeycomboBox);
            this.groupBox35.Controls.Add(this.PauseCrModcomboBox);
            this.groupBox35.Controls.Add(this.label21);
            this.groupBox35.Controls.Add(this.label20);
            this.groupBox35.Location = new System.Drawing.Point(8, 6);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(179, 149);
            this.groupBox35.TabIndex = 0;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "General";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 16);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(140, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Pause Combat Routine Key:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 74);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(159, 13);
            this.label21.TabIndex = 1;
            this.label21.Text = "Pause Combat Routine Modifier:";
            // 
            // PauseCrModcomboBox
            // 
            this.PauseCrModcomboBox.FormattingEnabled = true;
            this.PauseCrModcomboBox.Items.AddRange(new object[] {
            "Alt",
            "Shift",
            "Ctrl"});
            this.PauseCrModcomboBox.Location = new System.Drawing.Point(9, 41);
            this.PauseCrModcomboBox.Name = "PauseCrModcomboBox";
            this.PauseCrModcomboBox.Size = new System.Drawing.Size(144, 21);
            this.PauseCrModcomboBox.TabIndex = 2;
            this.PauseCrModcomboBox.SelectedIndexChanged += new System.EventHandler(this.PauseCrModcomboBox_SelectedIndexChanged);
            // 
            // PauseCrKeycomboBox
            // 
            this.PauseCrKeycomboBox.FormattingEnabled = true;
            this.PauseCrKeycomboBox.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.PauseCrKeycomboBox.Location = new System.Drawing.Point(9, 100);
            this.PauseCrKeycomboBox.Name = "PauseCrKeycomboBox";
            this.PauseCrKeycomboBox.Size = new System.Drawing.Size(144, 21);
            this.PauseCrKeycomboBox.TabIndex = 3;
            this.PauseCrKeycomboBox.SelectedIndexChanged += new System.EventHandler(this.PauseCrKeycomboBox_SelectedIndexChanged);
            // 
            // ScourgeBloomSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 597);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScourgeBloomSettings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ScourgeBloom | Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ScourgeBloomSettings_Load);
            this.TabControl1.ResumeLayout(false);
            this.Changelog.ResumeLayout(false);
            this.Changelog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.General.ResumeLayout(false);
            this.Cooldowns.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIceBoundFortitudeHp)).EndInit();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.Talents.ResumeLayout(false);
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.groupBox30.ResumeLayout(false);
            this.groupBox30.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox34.ResumeLayout(false);
            this.groupBox34.PerformLayout();
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.groupBox24.PerformLayout();
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.Racials.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWarStompEnemies)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStoneformHp)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGiftOfTheNaaruHp)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Trinkets.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket2EnemyHealth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket2MyHealth)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket1EnemyHealth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrinket1MyHealth)).EndInit();
            this.Advanced.ResumeLayout(false);
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStrangulatemax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrangulatemin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMFmax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMFmin)).EndInit();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHealthstoneUseHp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealingTonicHp)).EndInit();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeathSiphonHp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDeathStrikeHp)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.Debug.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.Hotkeys.ResumeLayout(false);
            this.groupBox35.ResumeLayout(false);
            this.groupBox35.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl TabControl1;
        private TabPage General;
        private PropertyGrid GeneralSGrid;
        private TabPage Debug;
        private Button DumpSpellsButton;
        private Button SaveButton;
        private Button AFKPresetButton;
        private Button RotationOnlyPresetButton;
        private Button DungeonBuddyPresetButton;
        private TabPage Changelog;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private TabPage Racials;
        private GroupBox groupBox6;
        private Label label1;
        private CheckBox UseWarStompCheckBox;
        private GroupBox groupBox5;
        private CheckBox UseArcaneTorrentCheckBox;
        private GroupBox groupBox4;
        private CheckBox UseStoneformOnlyToClearCheckBox;
        private CheckBox UseStoneformCheckBox;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private CheckBox UseHumanRacialCheckBox;
        private GroupBox groupBox1;
        private CheckBox UseRacialsCheckBox;
        private NumericUpDown numGiftOfTheNaaruHp;
        private Label label2;
        private NumericUpDown numWarStompEnemies;
        private NumericUpDown numStoneformHp;
        private Label label3;
        private CheckBox UseGiftoftheNaruuCheckBox;
        private TabPage Trinkets;
        private TabPage Cooldowns;
        private GroupBox groupBox7;
        private CheckBox GargoyleOnCDcheckBox;
        private CheckBox BosArcaneTorrentCheckBox;
        private GroupBox groupBox9;
        private GroupBox groupBox8;
        private Label label4;
        private CheckBox T1OnEnemyHealthBelowCheckBox;
        private CheckBox T1OnMyHealthBelowCheckBox;
        private CheckBox T1OnBurstCheckBox;
        private CheckBox T1OnCdCheckBox;
        private CheckBox T1OnLoCCheckBox;
        private CheckBox UseTrinket1CheckBox;
        private NumericUpDown numTrinket1EnemyHealth;
        private NumericUpDown numTrinket1MyHealth;
        private Label label6;
        private Label label5;
        private NumericUpDown numTrinket2EnemyHealth;
        private NumericUpDown numTrinket2MyHealth;
        private Label label7;
        private Label label8;
        private Label label9;
        private CheckBox T2OnEnemyHealthBelowCheckBox;
        private CheckBox T2OnMyHealthBelowCheckBox;
        private CheckBox T2OnBurstCheckBox;
        private CheckBox T2OnCdCheckBox;
        private CheckBox T2OnLoCCheckBox;
        private CheckBox UseTrinket2CheckBox;
        private GroupBox groupBox10;
        private TabPage Advanced;
        private Button PVPPresetButton;
        private GroupBox groupBox11;
        private GroupBox groupBox12;
        private Label label11;
        private CheckBox checkBox1;
        private CheckBox UseDeathGripInPvPCheckBox;
        private CheckBox UseDeathGripCheckBox;
        private GroupBox groupBox13;
        private CheckBox UseBerserkingCheckBox;
        private CheckBox UseBloodFuryCheckBox;
        private CheckBox BosBerserkingCheckBox;
        private CheckBox BosBloodFuryCheckBox;
        private CheckBox BerserkingOnCdCheckBox;
        private CheckBox BloodFuryOnCdCheckBox;
        private GroupBox groupBox15;
        private GroupBox groupBox14;
        private CheckBox UsePillarofFrostOnCdCheckBox;
        private GroupBox groupBox16;
        private CheckBox UseIceboundFCheckBox;
        private NumericUpDown numIceBoundFortitudeHp;
        private Label label10;
        private GroupBox groupBox17;
        private GroupBox groupBox18;
        private CheckBox useHealingToniccheckBox;
        private Label label12;
        private NumericUpDown numHealingTonicHp;
        private NumericUpDown numHealthstoneUseHp;
        private Label label13;
        private CheckBox UseHealthstonecheckBox;
        private CheckBox DeathSiphoncheckBox;
        private CheckBox DeathStrikecheckBox;
        private NumericUpDown numDeathSiphonHp;
        private Label label15;
        private NumericUpDown numDeathStrikeHp;
        private Label label14;
        private GroupBox groupBox19;
        private NumericUpDown numMFmax;
        private NumericUpDown numMFmin;
        private Label label17;
        private Label label16;
        private CheckBox DelayMFcheckBox;
        private CheckBox UseMFcheckBox;
        private NumericUpDown numStrangulatemax;
        private NumericUpDown numStrangulatemin;
        private Label label18;
        private Label label19;
        private CheckBox DelayStrangulateCheckBox;
        private CheckBox StrangulateCheckBox;
        private CheckBox UseTrinket2OnBoS;
        private CheckBox UseTrinket1OnBoS;
        private TabPage Talents;
        private GroupBox groupBox34;
        private GroupBox groupBox33;
        private GroupBox groupBox32;
        private GroupBox groupBox29;
        private GroupBox groupBox28;
        private GroupBox groupBox26;
        private GroupBox groupBox24;
        private GroupBox groupBox23;
        private GroupBox groupBox22;
        private GroupBox groupBox21;
        private GroupBox groupBox31;
        private GroupBox groupBox30;
        private GroupBox groupBox27;
        private GroupBox groupBox25;
        private GroupBox groupBox20;
        private CheckBox UseLichBornecheckBox;
        private CheckBox UseUnholyBlightcheckBox;
        private CheckBox UsePLagueLeechcheckBox;
        private CheckBox AutoAmsCheckbox;
        private CheckBox UseBoScheckBox;
        private CheckBox UseDefilecheckBox;
        private CheckBox UseDesecratedGroundcheckBox;
        private CheckBox UseRemorselessWintercheckBox;
        private CheckBox UseGorefiendsGraspcheckBox;
        private CheckBox UseConversioncheckBox;
        private CheckBox UseDeathSiphoncheckBox;
        private CheckBox UseDeathPactcheckBox;
        private CheckBox UseBloodTapcheckBox;
        private CheckBox UseAsphyxiatecheckBox;
        private CheckBox UseDeathsAdvancecheckBox;
        private CheckBox UseAMZcheckBox;
        private CheckBox UseArmyoftheDeadcheckBox;
        private TabPage Hotkeys;
        private GroupBox groupBox35;
        private Label label21;
        private Label label20;
        private ComboBox PauseCrModcomboBox;
        private ComboBox PauseCrKeycomboBox;
    }
}
