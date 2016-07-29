/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Windows.Forms;
using ScourgeBloom.Helpers;
using ScourgeBloom.Settings;
using Styx.Common;
using Styx.WoWInternals;
using S = ScourgeBloom.Lists.SpellLists;

namespace ScourgeBloom.Managers
{
    public class HotkeyManager
    {
        #region General

        public static bool KeysRegistered { get; set; }

        public static void RegisterHotKeys()
        {
            if (KeysRegistered) return;

            // Pause
            HotkeysManager.Register("PauseHotkey", KeyPause, ModifKeyPause, ret =>
            {
                PauseHotkey = !PauseHotkey;
                Lua.DoString(PauseHotkey
                    ? @"print('Pause: \124cFF15E61C Enabled!')"
                    : @"print('Pause: \124cFFE61515 Disabled!')");
            });

            Log.WriteLog(LogLevel.Normal, " " + "\r\n");
            Log.WriteLog(LogLevel.Normal, "Pause Key: " + ModifKeyPause + "+ " + KeyPause);


            KeysRegistered = true;
            Log.WriteLog(LogLevel.Normal, " " + "\r\n");
        }

        public static void RemoveHotkeys()
        {
            if (!KeysRegistered) return;

            HotkeysManager.Unregister("PauseHotkey");

            PauseHotkey = false;
            KeysRegistered = false;

            Lua.DoString(@"print('Hotkeys: \124cFFE61515 Removed!')");
            Log.WriteLog(LogLevel.Normal, "Hotkeys: Removed!");
        }

        #endregion General

        #region Pause

        public static bool PauseHotkey { get; set; }

        private static Keys KeyPause => (Keys) Enum.Parse(typeof (Keys), GeneralSettings.Instance.PauseHotkey);

        private static ModifierKeys ModifKeyPause
            => (ModifierKeys) Enum.Parse(typeof (ModifierKeys), GeneralSettings.Instance.ModPauseHotkey);

        #endregion Pause
    }
}