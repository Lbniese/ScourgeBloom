/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Linq;
using System.Windows.Media;
using CommonBehaviors.Actions;
using NewMixedMode;
using ScourgeBloom.Helpers;
using ScourgeBloom.Lists;
using ScourgeBloom.Managers;
using ScourgeBloom.Settings;
using ScourgeBloom.Utilities;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.POI;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.Plugins;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom
{
    public class ScourgeBloom : CombatRoutine
    {

        private static readonly WaitTimer WaitForLatencyCheck = new WaitTimer(TimeSpan.FromSeconds(5));

        protected static readonly LocalPlayer Me = StyxWoW.Me;

        public static readonly Version Version = new Version(1, 5, 74);

        private static bool _initialized;

        private static readonly WaitTimer PollInterval = new WaitTimer(TimeSpan.FromSeconds(10));
        private static uint _lastFps;

#pragma warning disable 169
        private bool _contextEventSubscribed;
#pragma warning restore 169

        public static uint Latency { get; set; }

        internal static WoWContext ForcedContext { get; set; }

        internal static bool IsQuestBotActive { get; set; }
        internal static bool IsBgBotActive { get; set; }
        internal static bool IsDungeonBuddyActive { get; set; }
        internal static bool IsPokeBuddyActive { get; set; }
        internal static bool IsManualMovementBotActive { get; set; }
        internal static bool IsGrindBotActive { get; set; }


        public static WoWContext TrainingDummyBehaviors { get; set; }

        public static uint LastMapId { get; set; }

        public override string Name => $"ScourgeBloom - v{Version}.";

        public override WoWClass Class => WoWClass.None;

        public override bool WantButton => true;

        public override CapabilityFlags SupportedCapabilities => CapabilityFlags.All;

        public override Composite CombatBehavior => CreateCombat();

        public override Composite PullBehavior => CreatePull();

        public override Composite RestBehavior => CreateRest();

        public override Composite HealBehavior => CreateHeal();

        public override Composite PreCombatBuffBehavior => CreatePreCombatBuff();

        public override Composite CombatBuffBehavior => CreateCombatBuff();

        public override Composite PullBuffBehavior => CreatePullBuff();

        public override Composite DeathBehavior => CreateDeathBehavior();

        public static bool Paused => HotkeyManager.PauseHotkey;

        #region Pulse

        public override void Pulse()
        {
            try
            {
                if (!StyxWoW.IsInGame || !StyxWoW.IsInWorld || Me == null || !Me.IsValid || !Me.IsAlive)
                    return;

                //Units.EnemyAnnex(8f);

                if (Capabilities.IsPetUsageAllowed && Capabilities.IsPetSummonAllowed &&
                    Me.Specialization == WoWSpec.DeathKnightUnholy)
                {
                    PetManager.Pulse();
                }

                if (Capabilities.IsTargetingAllowed && Me.GotTarget && !Me.CurrentTarget.CanLoot &&
                    !Me.CurrentTarget.Lootable && Me.CurrentTarget.IsDead &&
                    BotPoi.Current.Type != PoiType.Loot && BotPoi.Current.Type != PoiType.Skin &&
                    !ObjectManager.GetObjectsOfType<WoWUnit>()
                        .Any(
                            u =>
                                u.IsDead &&
                                ((CharacterSettings.Instance.LootMobs && u.CanLoot && u.Lootable) ||
                                 (CharacterSettings.Instance.SkinMobs && u.Skinnable && u.CanSkin)) &&
                                u.Distance < CharacterSettings.Instance.LootRadius))
                {
                    Log.WriteLog("[ScourgeBloom] Clearing target, since it is dead and not lootable!");
                    Me.ClearTarget();
                }


                if (WaitForLatencyCheck.IsFinished)
                {
                    Latency = StyxWoW.WoWClient.Latency;
                    WaitForLatencyCheck.Reset();
                }


                UpdateDiagnosticFps();

                if (Paused) return;

                base.Pulse();
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
                throw;
            }
        }

        #endregion Pulse

        public override void Initialize()
        {
            HotkeysManager.Initialize(StyxWoW.Memory.Process.MainWindowHandle);
            GeneralSettings.Instance.Load();
            BotEvents.OnBotStarted += OnBotStartEvent;
            BotEvents.OnBotStopped += OnBotStopEvent;
            TalentManager.InitTalents();

            Lua.Events.AttachEvent("PLAYER_REGEN_DISABLED", OnCombatStarted);
        }

        private static uint GetFps()
        {
            try
            {
                return (uint) Lua.GetReturnVal<float>("return GetFramerate()", 0);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        private static void UpdateDiagnosticFps()
        {
            if (GeneralSettings.Instance.Debug && PollInterval.IsFinished && Me.Combat)
            {
                var currFps = GetFps();
                if (currFps != _lastFps)
                {
                    _lastFps = currFps;
                    Logging.WriteDiagnostic("Combat Performance Monitoring: FPS:{ 0} Latency: { 1}, currFps, Latency");
                }

                PollInterval.Reset();
            }
        }

        private static void OnCombatStarted(object sender, LuaEventArgs e)
        {
            Logging.Write(Colors.GreenYellow, "[ScourgeBloom] Entered Combat");
            Globals.CombatTime.Restart();
        }

        public override void ShutDown()
        {
            BotEvents.OnBotStarted -= OnBotStartEvent;
            BotEvents.OnBotStopped -= OnBotStopEvent;
        }

        private static void OnBotStartEvent(object o)
        {
            HotkeyManager.RegisterHotKeys();
            InitializeOnce();
        }

        private static void OnBotStopEvent(object o)
        {
            HotkeyManager.RemoveHotkeys();
        }

        private static void InitializeOnce()
        {
            if (_initialized)
                return;

            ScourgeBloomSettings.ClassSettings.Initialize();
            StatCounter.StatCount();

            //TalentManager.InitTalents();
            GeneralSettings.Instance.Save();

            _initialized = true;

            Log.WriteLog("ScourgeBloom Loaded", Colors.DarkGreen);
        }

        protected virtual Composite CreateCombat()
        {
            return new HookExecutor("ScourgeBloom_Combat_Root",
                "Root composite for ScourgeBloom combat. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreatePreCombatBuff()
        {
            return new HookExecutor("ScourgeBloom_PreCombatBuffs_Root",
                "Root composite for ScourgeBloom PreCombatBuffs. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreateCombatBuff()
        {
            return new HookExecutor("ScourgeBloom_CombatBuffs_Root",
                "Root composite for ScourgeBloom PreCombatBuffs. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreateDeathBehavior()
        {
            return new HookExecutor("ScourgeBloom_DeathBehavior_Root",
                "Root composite for ScourgeBloom DeathBehavior. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreatePullBuff()
        {
            return new HookExecutor("ScourgeBloom_PullBuffs_Root",
                "Root composite for ScourgeBloom PreCombatBuffs. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreateRest()
        {
            return new HookExecutor("ScourgeBloom_Rest_Root",
                "Root composite for ScourgeBloom Resting. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreatePull()
        {
            return new HookExecutor("ScourgeBloom_Pull_Root",
                "Root composite for ScourgeBloom Pulling. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        protected virtual Composite CreateHeal()
        {
            return new HookExecutor("ScourgeBloom_Heals_Root",
                "Root composite for ScourgeBloom heals. Rotations will be plugged into this hook.",
                new ActionAlwaysFail());
        }

        #region DON'T TOUCH - CORE SHIT

        public static string RaceName()
        {
            return Me.Race.ToString().CamelToSpaced();
        }

        public static string ClassName()
        {
            return Me.Class.ToString().CamelToSpaced();
        }

        public static string SpecName()
        {
            if (TalentManager.CurrentSpec == WoWSpec.None)
                return Me.Level <= 80 ? " Lowbie" : " Non-specialized";

            var spec = TalentManager.CurrentSpec.ToString();
            spec = spec.Substring(Me.Class.ToString().Length);
            return " " + spec; // simulate CamelToSpaced() leading blank
        }

        public static string SpecAndClassName()
        {
            return SpecName() + Me.Class.ToString().CamelToSpaced();
        }

        public static string ClassAndSpecName()
        {
            return Me.Class.ToString().CamelToSpaced() + SpecName();
        }

        public static string GetBotName()
        {
            BotBase bot = null;

            if (TreeRoot.Current != null)
            {
                if (!(TreeRoot.Current is MixedModeEx))
                    bot = TreeRoot.Current;
                else
                {
                    var mmb = (MixedModeEx) TreeRoot.Current;
                    if (mmb != null)
                    {
                        if (mmb.SecondaryBot != null && mmb.SecondaryBot.RequirementsMet)
                            return "Mixed:" + mmb.SecondaryBot.Name;
                        return mmb.PrimaryBot != null ? "Mixed:" + mmb.PrimaryBot.Name : "Mixed:[primary null]";
                    }
                }
            }

            return bot == null ? "(null)" : bot.Name;
        }

        public static BotBase GetCurrentBotBase()
        {
            var bot = TreeRoot.Current;
            var ex = bot as MixedModeEx;
            if (ex == null) return bot;
            var mmb = ex;
            if (mmb.SecondaryBot != null && mmb.SecondaryBot.RequirementsMet)
                bot = mmb.SecondaryBot;
            else
                bot = mmb.PrimaryBot;

            return bot;
        }

        public static bool IsBotInUse(params string[] nameSubstrings)
        {
            var botName = GetBotName().ToUpper();
            return nameSubstrings.Any(s => botName.Contains(s.ToUpper()));
        }

        public static PluginContainer FindPlugin(string pluginName)
        {
#if OLD_PLUGIN_CHECK
            var lowerNames = nameSubstrings.Select(s => s.ToLowerInvariant()).ToList();
            bool res = Styx.Plugins.PluginManager.Plugins.Any(p => p.Enabled && lowerNames.Contains(p.Name.ToLowerInvariant()));
            return res;
#else
            return
                PluginManager.Plugins.FirstOrDefault(
                    pi => pluginName.Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase));
#endif
        }

        public static bool IsPluginEnabled(params string[] nameSubstrings)
        {
            return
                (from s in nameSubstrings select FindPlugin(s) into pi where pi != null select pi.Enabled)
                    .FirstOrDefault();
        }

        public static bool SetPluginEnabled(string s, bool enabled)
        {
            var pi = FindPlugin(s);
            if (pi == null) return false;
            pi.Enabled = enabled;
            return true;
        }

        public static bool InCinematic()
        {
            var inCin = Lua.GetReturnVal<bool>("return InCinematic()", 0);
            return inCin;
        }

        public static string GetScourgeBloomRoutineName()
        {
            return $"ScourgeBloom - v{Version}.";
        }

        public override void OnButtonPress()
        {
            var gui = new ScourgeBloomSettings();
            gui.Show();
        }

        #endregion
    }
}