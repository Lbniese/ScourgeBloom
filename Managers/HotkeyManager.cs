using System;
using System.Windows.Forms;
using System.Windows.Media;
using ScourgeBloom.Helpers;
using ScourgeBloom.Settings;
using Styx.Common;
using Styx.WoWInternals;

namespace ScourgeBloom.Managers
{
    internal class HotkeyManager
    {
        public static bool PauseHotkey { get; set; }
        public static bool KeysRegistered { get; set; }

        private static Keys KeyPause => (Keys)Enum.Parse(typeof(Keys), GeneralSettings.Instance.PauseHotkey);

        private static ModifierKeys ModifKeyPause => (ModifierKeys)Enum.Parse(typeof(ModifierKeys), GeneralSettings.Instance.ModPauseHotkey);

        public static void RegisterHotKeys()
        {
            if (KeysRegistered) return;

            HotkeysManager.Register("PauseHotkey", KeyPause, ModifKeyPause, ret =>
            {
                PauseHotkey = !PauseHotkey;
                Lua.DoString(PauseHotkey ? @"print('Routine Paused: \124cFF15E61C Enabled!')" : @"print('Routine Paused: \124cFFE61515 Disabled!')");
            });

            KeysRegistered = true;
            Log.WriteLog(LogLevel.Normal, " " + "\r\n");
            Log.WriteLog(LogLevel.Normal, "Pause ScourgeBloom: "+ ModifKeyPause + "+ " + KeyPause);
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
    }
}
