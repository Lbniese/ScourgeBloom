using System;
using System.Linq;
using ScourgeBloom.Settings;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.DBC;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    internal static class PvP
    {
        private static WoWGuid _lastIsSlowedTarget = WoWGuid.Empty;
        private static bool _lastIsSlowedResult;
        private static int _lastIsSlowedSpellId;

        /// <summary>
        ///     determines if you are inside a battleground/arena prior to start.  this was previously
        ///     known as the preparation phase easily identified by a Preparation or Arena Preparation
        ///     buff, however those auras were removed in MoP
        /// </summary>
        /// <returns>true if in Battleground/Arena prior to start, false otherwise</returns>
        public static bool IsPrepPhase => Battlegrounds.IsInsideBattleground && PrepTimeLeft > 0;

        public static int PrepTimeLeft => Math.Max(0, (int) (BattlegroundStart - DateTime.UtcNow).TotalSeconds);

        public static DateTime BattlegroundStart { get; private set; }

        public static bool IsStunned(this WoWUnit unit)
        {
            return unit.Stunned || unit.HasAuraWithEffect(WoWApplyAuraType.ModStun);
        }

        public static bool IsRooted(this WoWUnit unit)
        {
            return unit.Rooted || unit.HasAuraWithEffect(WoWApplyAuraType.ModRoot);
        }

        public static bool IsSilenced(this WoWUnit unit)
        {
            return unit.Silenced ||
                   unit.HasAuraWithEffect(WoWApplyAuraType.ModSilence, WoWApplyAuraType.ModPacifySilence);
        }

        /// <summary>
        ///     determines if an Aura with any slowing effect matching
        ///     slowedPct or greater is affecting unit
        /// </summary>
        /// <param name="unit">WoWUnit to check</param>
        /// <param name="slowedPct">% slowing required for true</param>
        /// <returns>true: if slowed by slowedPct or more, false: if not slowed as much as specified</returns>
        public static bool IsSlowed(this WoWUnit unit, uint slowedPct = 50)
        {
            if (unit == null)
                return false;

            var slowedCompare = -(int) slowedPct;
            WoWAura foundAura = null;
            SpellEffect foundSe = null;
            var foundSpellId = 0;

            foreach (var aura in unit.GetAllAuras())
            {
                foreach (
                    var se in
                        aura.Spell.SpellEffects.Where(
                            se =>
                                se != null && se.AuraType == WoWApplyAuraType.ModDecreaseSpeed &&
                                se.BasePoints <= slowedCompare))
                {
                    foundAura = aura;
                    foundSe = se;
                    foundSpellId = aura.SpellId;
                    break;
                }
            }

            if (!GeneralSettings.Instance.ExtendedLogging) return foundSe != null;
            if (foundAura != null == _lastIsSlowedResult && _lastIsSlowedTarget == unit.Guid &&
                _lastIsSlowedSpellId == foundSpellId) return foundSe != null;
            _lastIsSlowedResult = foundAura != null;
            _lastIsSlowedTarget = unit.Guid;
            _lastIsSlowedSpellId = foundSpellId;
            if (foundAura != null)
            {
                Logging.WriteDiagnostic("IsSlowed: target {0} slowed {1}% with [{2}] #{3}", unit.SafeName(),
                    foundSe.BasePoints, foundAura.Name, foundSpellId);
            }

            return foundSe != null;
        }

        #region Battleground Start Timer

        private static bool _startTimerAttached;

        public static void AttachStartTimer()
        {
            if (_startTimerAttached)
                return;

            Lua.Events.AttachEvent("START_TIMER", HandleStartTimer);
            ScourgeBloom.OnWoWContextChanged += HandleContextChanged;
            _startTimerAttached = true;
        }

        public static void DetachStartTimer()
        {
            if (!_startTimerAttached)
                return;

            _startTimerAttached = false;
            Lua.Events.DetachEvent("START_TIMER", HandleStartTimer);
        }

        private static void HandleStartTimer(object sender, LuaEventArgs args)
        {
            var secondsUntil = int.Parse(args.Args[1].ToString());
            var prevStart = BattlegroundStart;
            BattlegroundStart = DateTime.UtcNow + TimeSpan.FromSeconds(secondsUntil);

            if (!(BattlegroundStart - prevStart).TotalSeconds.Between(-1, 1))
            {
                Logging.WriteDiagnostic("Start_Timer: Battleground starts in {0} seconds", secondsUntil);
            }
        }

        internal static void HandleContextChanged(object sender, WoWContextEventArg e)
        {
            if (e.CurrentContext != WoWContext.Battlegrounds)
                BattlegroundStart = DateTime.UtcNow;
            else
                BattlegroundStart = DateTime.UtcNow + TimeSpan.FromSeconds(120);
            // just add enough for now... accurate time set by event handler
        }

        #endregion
    }
}