/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    internal class TimeToDeath
    {
        // following class should probably be in Unit, but made a separate
        // .. extension class to separate the private property names.
        // --
        // credit to Handnavi.  the following is a wrapping of his code
        public static class TimeToDeathExtension
        {
            private static uint _firstLife; // life of mob when first seen
            private static uint _firstLifeMax; // max life of mob when first seen
            private static int _firstTime; // time mob was first seen
            private static uint _currentLife; // life of mob now
            private static int _currentTime; // time now


            private static readonly DateTime TimeOrigin = new DateTime(2012, 1, 1); // Refernzdatum (festgelegt)
            public static WoWGuid Guid { get; set; } // guid of mob

            /// <summary>
            ///     seconds until the target dies.  first call initializes values. subsequent
            ///     return estimate or indeterminateValue if death can't be calculated
            /// </summary>
            /// <param name="target">unit to monitor</param>
            /// <param name="indeterminateValue">return value if death cannot be calculated ( -1 or int.MaxValue are common)</param>
            /// <returns>number of seconds </returns>
            public static long TimeToDeath(WoWUnit target, long indeterminateValue = -1)
            {
                if (target == null || !target.IsValid || !target.IsAlive)
                {
                    //Logging.Write("TimeToDeath: {0} (GUID: {1}, Entry: {2}) is dead!", target.SafeName(), target.Guid, target.Entry);
                    return 0;
                }

                if (StyxWoW.Me.CurrentTarget.FactionId == 679)
                {
                    return 111; // pick a magic number since training dummies dont die
                }

                //Fill variables on new target or on target switch, this will loose all calculations from last target
                if (Guid != target.Guid || (Guid == target.Guid && target.CurrentHealth == _firstLifeMax))
                {
                    Guid = target.Guid;
                    _firstLife = target.CurrentHealth;
                    _firstLifeMax = target.MaxHealth;
                    _firstTime = ConvDate2Timestam(DateTime.UtcNow);
                    //Lets do a little trick and calculate with seconds / u know Timestamp from unix? we'll do so too
                }
                _currentLife = target.CurrentHealth;
                _currentTime = ConvDate2Timestam(DateTime.UtcNow);
                var timeDiff = _currentTime - _firstTime;
                var hpDiff = _firstLife - _currentLife;
                if (hpDiff > 0)
                {
                    /*
                * Rule of three (Dreisatz):
                * If in a given timespan a certain value of damage is done, what timespan is needed to do 100% damage?
                * The longer the timespan the more precise the prediction
                * time_diff/hp_diff = x/first_life_max
                * x = time_diff*first_life_max/hp_diff
                *
                * For those that forgot, http://mathforum.org/library/drmath/view/60822.html
                */
                    var fullTime = timeDiff*_firstLifeMax/hpDiff;
                    var pastFirstTime = (_firstLifeMax - _firstLife)*timeDiff/hpDiff;
                    var calcTime = _firstTime - pastFirstTime + fullTime - _currentTime;
                    if (calcTime < 1) calcTime = 1;
                    //calc_time is a int value for time to die (seconds) so there's no need to do SecondsToTime(calc_time)
                    var timeToDie = calcTime;
                    //Logging.Write("TimeToDeath: {0} (GUID: {1}, Entry: {2}) dies in {3}, you are dpsing with {4} dps", target.SafeName(), target.Guid, target.Entry, timeToDie, dps);
                    return timeToDie;
                }
                if (hpDiff <= 0)
                {
                    //unit was healed,resetting the initial values
                    Guid = target.Guid;
                    _firstLife = target.CurrentHealth;
                    _firstLifeMax = target.MaxHealth;
                    _firstTime = ConvDate2Timestam(DateTime.UtcNow);
                    //Lets do a little trick and calculate with seconds / u know Timestamp from unix? we'll do so too
                    //Logging.Write("TimeToDeath: {0} (GUID: {1}, Entry: {2}) was healed, resetting data.", target.SafeName(), target.Guid, target.Entry);
                    return indeterminateValue;
                }
                if (_currentLife == _firstLifeMax)
                {
                    //Logging.Write("TimeToDeath: {0} (GUID: {1}, Entry: {2}) is at full health.", target.SafeName(), target.Guid, target.Entry);
                    return indeterminateValue;
                }
                //Logging.Write("TimeToDeath: {0} (GUID: {1}, Entry: {2}) no damage done, nothing to calculate.", target.SafeName(), target.Guid, target.Entry);
                return indeterminateValue;
            }

            private static int ConvDate2Timestam(DateTime time)
            {
#if PREV
                DateTime baseLine = new DateTime(1970, 1, 1); // Refernzdatum (festgelegt)
                DateTime date2 = time; // jetztiges Datum / Uhrzeit
                var ts = new TimeSpan(date2.Ticks - baseLine.Ticks); // das Delta ermitteln
                // Das Delta als gesammtzahl der sekunden ist der Timestamp
                return (Convert.ToInt32(ts.TotalSeconds));
#else
                return (int) (time - TimeOrigin).TotalSeconds;
#endif
            }
        }
    }
}