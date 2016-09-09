/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Media;
using ScourgeBloom.Helpers;
using ScourgeBloom.Settings;
using Styx;
using Styx.Common;
using Styx.WoWInternals;
using S = ScourgeBloom.Lists.SpellLists;

namespace ScourgeBloom.Managers
{
    public class HotkeyManager
    {
        #region General

        public static bool PauseOn { get; set; }
        public static bool CooldownsOn { get; set; }
        public static bool ManualOn { get; set; }
        public static bool KeysRegistered { get; set; }

        public static void RegisterHotKeys()
        {
            if (KeysRegistered) return;
            var converter = TypeDescriptor.GetConverter(typeof (Keys));
            // Pause
            if (!string.IsNullOrEmpty(GeneralSettings.Instance.HotkeyPauseKey) &&
                GeneralSettings.Instance.HotkeyPauseModifier > 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                HotkeysManager.Register("PauseHotkey",
                    (Keys) converter.ConvertFromString(GeneralSettings.Instance.HotkeyPauseKey),
                    (ModifierKeys) GeneralSettings.Instance.HotkeyPauseModifier, ret =>
                    {
                        PauseOn = !PauseOn;
                        Lua.DoString(PauseOn
                            ? @"print('Pause: \124cFF15E61C Enabled!')"
                            : @"print('Pause: \124cFFE61515 Disabled!')");
                    });
            }

            Log.WriteLog(LogLevel.Normal, " " + "\r\n");
            // ReSharper disable once PossibleNullReferenceException
            Log.WriteLog(LogLevel.Normal,
                "Pause Key: " + (ModifierKeys) GeneralSettings.Instance.HotkeyPauseModifier + "+ " +
                (Keys) converter.ConvertFromString(GeneralSettings.Instance.HotkeyPauseKey));


            KeysRegistered = true;
            StyxWoW.Overlay.AddToast("Hotkeys: Registered!", 2000);
            Logging.Write(Colors.Purple, "Hotkeys: Registered!");
        }

        public static void RemoveHotkeys()
        {
            if (!KeysRegistered) return;

            HotkeysManager.Unregister("PauseHotkey");

            PauseOn = false;
            KeysRegistered = false;

            Lua.DoString(@"print('Hotkeys: \124cFFE61515 Removed!')");
            Log.WriteLog(LogLevel.Normal, "Hotkeys: Removed!");
        }

        #endregion General
    }
}