using System;

namespace ScourgeBloom.Helpers
{
    [Flags]
    public enum HealingContext
    {
        None = 0,
        Normal = 0x1,
        Instances = 0x2,
        Battlegrounds = 0x4,
        Raids = 0x8
    }
}