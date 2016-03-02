/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Threading;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.WoWInternals;

namespace ScourgeBloom.Utilities
{
    internal class SpiritHandler
    {
        private static readonly Random Rnd = new Random();

        public static void ReleaseSpirit()
        {
            if (!StyxWoW.IsInGame)
            {
                return;
            }

            if (!StyxWoW.Me.IsDead) return;
            var delay = Rnd.Next(3000, 7000);
            Logging.Write(Colors.Aqua, "Releasing spirit in 3-7 seconds!");
            Thread.Sleep(delay);
            Lua.DoString($"RunMacroText(\"{"/script RepopMe()"}\")");
        }
    }
}