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

        
    }
}