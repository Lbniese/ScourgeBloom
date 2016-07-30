using System;
using System.Collections.Generic;
using ScourgeBloom.Helpers;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;

namespace ScourgeBloom.Managers
{
    internal static class TalentManager
    {
        private static uint _spellCount;
        private static uint _spellBookSignature;

        private static readonly WaitTimer EventRebuildTimer = new WaitTimer(TimeSpan.FromSeconds(1));
        private static readonly WaitTimer SpecChangeTestTimer = new WaitTimer(TimeSpan.FromSeconds(3));

        private static bool _rebuild;
        //public const int TALENT_FLAG_ISEXTRASPEC = 0x10000;

        static TalentManager()
        {
        }

        public static WoWSpec CurrentSpec { get; private set; }

        public static List<Talent> Talents { get; private set; }

        private static int[] TalentId { get; set; }

        private static bool RebuildNeeded
        {
            get { return _rebuild; }
            set
            {
                _rebuild = value;
                EventRebuildTimer.Reset();
            }
        }

        public static void Init()
        {
            Talents = new List<Talent>();
            TalentId = new int[6];

            using (StyxWoW.Memory.AcquireFrame())
            {
                Update();

                Lua.Events.AttachEvent("PLAYER_LEVEL_UP", UpdateTalentManager);
                Lua.Events.AttachEvent("CHARACTER_POINTS_CHANGED", UpdateTalentManager);
                Lua.Events.AttachEvent("GLYPH_UPDATED", UpdateTalentManager);
                Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", UpdateTalentManager);
                Lua.Events.AttachEvent("PLAYER_SPECIALIZATION_CHANGED", UpdateTalentManager);
                Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", UpdateTalentManager);
            }
        }

        /// <summary>
        ///     checks if a specific talent is selected for current character
        /// </summary>
        /// <param name="index">index (base 1) of index</param>
        /// <returns>true if selected, false if not</returns>
        public static bool IsSelected(int index)
        {
            // return Talents.FirstOrDefault(t => t.Index == index).Selected;
            var tier = (index - 1)/3;
            if (tier.Between(0, 6))
                return TalentId[tier] == index;
            return false;
        }

        /// <summary>
        ///     gets talent selected for a specified tier (since mutually exclusive)
        /// </summary>
        /// <returns>true if selected, false if not</returns>
        public static int GetSelectedForTier(int tier)
        {
            tier--;
            if (tier.Between(0, 6))
                return TalentId[tier];
            return -1;
        }

        /// <summary>
        ///     event handler for messages which should cause behaviors to be rebuilt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void UpdateTalentManager(object sender, LuaEventArgs args)
        {
            // Since we hooked this in ctor, make sure we are the selected CC
            if (RoutineManager.Current.Name != ScourgeBloom.GetScourgeBloomRoutineName())
                return;

            var oldSpec = CurrentSpec;
            var oldTalent = TalentId;
            var oldSig = _spellBookSignature;
            var oldSpellCount = _spellCount;

            Logging.WriteDiagnostic("{0} Event Fired!", args.EventName);

            Update();

            if (args.EventName == "PLAYER_SPECIALIZATION_CHANGED")
            {
                SpecChangeTestTimer.Reset();
                Logging.WriteDiagnostic(
                    "TalentManager: receive a {0} event, currently {1} -- queueing check for new spec!", args.EventName,
                    CurrentSpec);
            }

            if (args.EventName == "PLAYER_LEVEL_UP")
            {
                RebuildNeeded = true;
                Logging.Write(Log.LogColor.Hilite, "TalentManager: Your character has leveled up! Now level {0}",
                    args.Args[0]);
            }

            if (CurrentSpec != oldSpec)
            {
                RebuildNeeded = true;
                Logging.Write(Log.LogColor.Hilite, "TalentManager: Your spec has been changed.");
            }

            int i;
            for (i = 0; i < 6; i++)
            {
                if (oldTalent[i] != TalentId[i])
                {
                    RebuildNeeded = true;
                    Logging.Write(Log.LogColor.Hilite, "TalentManager: Your talents have changed.");
                    break;
                }
            }

            if (_spellBookSignature != oldSig || _spellCount != oldSpellCount)
            {
                RebuildNeeded = true;
                Logging.Write(Log.LogColor.Hilite, "TalentManager: Your available Spells have changed.");
            }

            Logging.WriteDiagnostic(Log.LogColor.Hilite, "TalentManager: RebuildNeeded={0}", RebuildNeeded);
        }

        private static uint CalcSpellBookSignature()
        {
            uint sig = 0;
            foreach (var sp in SpellManager.Spells)
            {
                sig ^= (uint) sp.Value.Id;
            }
            return sig;
        }

        /// <summary>
        ///     loads WOW Talent and Spec info into cached list
        /// </summary>
        public static void Update()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                // With Legion, Low levels are an auto selected spec now. We want to keep using Lowbie behaviors pre level 10
                CurrentSpec = StyxWoW.Me.Level < 10 ? WoWSpec.None : StyxWoW.Me.Specialization;

                Talents.Clear();
                TalentId = new int[7];

                // Always 21 talents. 7 rows of 3 talents.
                for (var row = 0; row < 7; row++)
                {
                    for (var col = 0; col < 3; col++)
                    {
                        var selected =
                            Lua.GetReturnVal<bool>(
                                string.Format(
                                    "local t = select(4, GetTalentInfo({0}, {1}, GetActiveSpecGroup())) if t then return 1 end return nil",
                                    row + 1, col + 1), 0);
                        var index = 1 + row*3 + col;
                        var t = new Talent {Index = index, Selected = selected};
                        Talents.Add(t);

                        if (selected)
                            TalentId[row] = index;
                    }
                }

                _spellCount = (uint) SpellManager.Spells.Count;
                _spellBookSignature = CalcSpellBookSignature();
            }
        }

        public static bool Pulse()
        {
            if (SpecChangeTestTimer.IsFinished)
            {
                if (StyxWoW.Me.Level >= 10 && StyxWoW.Me.Specialization != CurrentSpec)
                {
                    CurrentSpec = StyxWoW.Me.Specialization;
                    RebuildNeeded = true;
                    Logging.Write(Log.LogColor.Hilite, "TalentManager: spec is now to {0}", ScourgeBloom.SpecName());
                }
            }

            if (RebuildNeeded && EventRebuildTimer.IsFinished)
            {
                RebuildNeeded = false;
                Logging.Write(Log.LogColor.Hilite, "TalentManager: Rebuilding behaviors due to changes detected.");
                Update(); // reload talents just in case
                ScourgeBloom.DescribeContext();
                //ScourgeBloom.Instance.RebuildBehaviors();
                return true;
            }

            return false;
        }

        #region Nested type: Talent

        public struct Talent
        {
            public bool Selected;
            public int Index;
        }

        #endregion
    }
}