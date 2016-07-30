/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common.Helpers;
using Styx.WoWInternals;

namespace ScourgeBloom.Helpers
{
    internal class DeadlyBossMods
    {
        private static readonly Dictionary<string, TimerBar> BarCache = new Dictionary<string, TimerBar>();

        public static int NumBars => Lua.GetReturnVal<int>("return DBM.Bars.numBars", 0);

        private static IEnumerable<string> BarIds
        {
            get
            {
                var barIds =
                    Lua.GetReturnVal<string>(
                        "t={} for bar in pairs(DBM.Bars.bars) do table.insert(t, bar.id) end return (table.concat(t,'@!@'))",
                        0);
                return barIds.Split(new[] {"@!@"}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static IEnumerable<TimerBar> Bars
        {
            get
            {
                var barIds =
                    Lua.GetReturnVal<string>(
                        "t={} for bar in pairs(DBM.Bars.bars) do table.insert(t, bar.id) end return (table.concat(t,'@!@'))",
                        0);

                return
                    barIds.Split(new[] {"@!@"}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(barId => new TimerBar(barId));
            }
        }

        public static TimerBar FindBarByPartialId(string id)
        {
            TimerBar bar;
            if (BarCache.TryGetValue(id, out bar))
            {
                // If the bar is dead, then remove it. We no longer want to track it.
                if (bar.Timer.IsFinished)
                {
                    BarCache.Remove(id);
                }
                else
                {
                    return bar;
                }
            }

            var firstMatching = BarIds.FirstOrDefault(bid => bid.Contains(id));
            if (!string.IsNullOrWhiteSpace(firstMatching))
            {
                bar = new TimerBar(firstMatching);
                BarCache.Add(id, bar);
                return bar;
            }

            return null;
        }

        private static string FindBarAndExecute(string id, string doStuff)
        {
            return $"for bar in pairs(DBM.Bars.bars) do if '{id}' == bar.id then {doStuff} end end";
        }

        internal class TimerBar
        {
            private WaitTimer _timer;

            public TimerBar(string barId)
            {
                Id = barId;
            }

            public string Id { get; }

            public bool Dead => Lua.GetReturnVal<bool>(FindBarAndExecute(Id, "return bar.dead"), 0);

            public bool Dummy => Lua.GetReturnVal<bool>(FindBarAndExecute(Id, "return bar.dummy"), 0);

            public bool Flashing => Lua.GetReturnVal<bool>(FindBarAndExecute(Id, "return bar.flashing"), 0);

            public bool Enlarged => Lua.GetReturnVal<bool>(FindBarAndExecute(Id, "return bar.enlarged"), 0);

            public bool FadingIn => Lua.GetReturnVal<bool>(FindBarAndExecute(Id, "return bar.fadingIn"), 0);

            public float LuaTimeLeft => Lua.GetReturnVal<float>(FindBarAndExecute(Id, "return bar.timer"), 0);

            public float LuaTotalTime => Lua.GetReturnVal<float>(FindBarAndExecute(Id, "return bar.totalTime"), 0);

            public TimeSpan TotalTime => TimeSpan.FromSeconds(LuaTotalTime);

            public TimeSpan TimeLeft => TimeSpan.FromSeconds(LuaTimeLeft);

            public WaitTimer Timer => _timer ?? (_timer = new WaitTimer(TimeLeft));

            public void Cancel()
            {
                Lua.DoString(FindBarAndExecute(Id, "bar:Cancel()"));
            }

            public override string ToString()
            {
                return $"Id: {Id}, TotalTime: {TotalTime}, TimeLeft: {TimeLeft}";
            }
        }
    }
}