using System;

namespace ScourgeBloom.Helpers
{
    [Flags]
    public enum WatchTargetForCast
    {
        None = 0,
        Current = 0x1, // only currenttarget
        Facing = 0x2, // only those we are safelyfacing
        InRange = 0x4 // regardless of facing
    }
}