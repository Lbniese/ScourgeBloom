using System;

namespace ScourgeBloom.Helpers
{
    [Flags]
    public enum BehaviorType
    {
        Rest = 0x1,
        PreCombatBuffs = 0x2,
        PullBuffs = 0x4,
        Pull = 0x8,
        Heal = 0x10,
        CombatBuffs = 0x20,
        Combat = 0x40,
        LossOfControl = 0x80,
        Death = 0x100,

        Initialize = 0x200, // initializer method (return is ignored)

        // this is no guarantee that the bot is in combat
        InCombat = Heal | CombatBuffs | Combat,
        // this is no guarantee that the bot is out of combat
        OutOfCombat = Rest | PreCombatBuffs | PullBuffs | Death,

        All = Rest | PreCombatBuffs | PullBuffs | Pull | Heal | CombatBuffs | Combat | LossOfControl | Death
    }
}