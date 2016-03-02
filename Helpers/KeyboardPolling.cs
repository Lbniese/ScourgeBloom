/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;
using Styx;

namespace ScourgeBloom.Helpers
{
    [UsedImplicitly]
    internal class KeyboardPolling : ScourgeBloom
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);


        public static bool IsKeyDown(Keys key, bool gameWindowFocused = true)
        {
            if (gameWindowFocused && GetForegroundWindow() != StyxWoW.Memory.Process.MainWindowHandle)
                return false;


            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }
    }
}