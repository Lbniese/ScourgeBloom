using System;
using ScourgeBloom.Helpers;
using Styx;
using Styx.Common;
using Styx.CommonBot.CharacterManagement;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Managers
{
    public class TalentManager
    {
        public static bool

            #region 56 level

            UnholyAllWillServe,
            UnholyBurstingSores,
            UnholyEbonFever,
            FrostShatteringStrikes,
            FrostIcyTalons,
            FrostMurderousEfficiency,

            #endregion

            #region 57 level

            UnholyEpidemic,
            UnholyPestilentPustules,
            UnholyBlightedRuneWeapon,
            FrostFreezingFog,
            FrostFrozenPulse,
            FrostHornOfWinter,

            #endregion

            #region 58 level

            UnholyUnholyFrenzy,
            UnholyCastigator,
            UnholyClawingShadows,
            FrostIcecap,
            FrostHungeringRuneWeapon,
            FrostAvalanche,

            #endregion

            #region 60 level

            UnholySludgeBelcher,
            UnholyAsphyxiate,
            UnholyDebilitatingInfestation,
            FrostAbominationsMight,
            FrostBlindingSleet,
            FrostWinterIsComing,

            #endregion

            #region 75 level

            UnholySpellEater,
            UnholyCorpseShield,
            UnholyLingeringApparition,
            FrostVolatileShielding,
            FrostPermafrost,
            FrostWhiteWalker,

            #endregion

            #region 90 level

            UnholyShadowInfusion,
            UnholyNecrosis,
            UnholyInfectedClaws,
            FrostFrostscythe,
            FrostRunicAttenuation,
            FrostGatheringStorm,

            #endregion

            #region 100 level

            UnholyDarkArbiter,
            UnholyDefile,
            UnholySoulReaper,
            FrostObliteration,
            FrostBreathOfSindragosa,
            FrostGlacialAdvance;

        #endregion

        private static LocalPlayer Me => StyxWoW.Me;

        public static WoWSpec CurrentSpec { get; private set; }

        public static event EventHandler SpecChanged;

        private static void OnSpecChanged(EventArgs args)
        {
            var handler = SpecChanged;
            if (handler != null)
            {
                handler(null, args);
            }
        }

        private static bool GetTalent(int tier, int index)
        {
            try
            {
                tier--;
                index--;
                var tp = Me.GetLearnedTalent(tier);
                if (tp == null)
                {
                    Log.WriteLog("TP " + tier.ToString() + " is null");
                    return false;
                }
                return tp.Index == index;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public static void InitTalents()
        {
            SetTalEvents();

            // don't allow any talents if < 56 level
            if (Me.Level < 56)
            {
                return;
            }

            if (Me.Specialization == WoWSpec.DeathKnightUnholy)
            {
                if (Me.Level >= 56)
                {
                    UnholyAllWillServe = GetTalent(1, 1);
                    UnholyBurstingSores = GetTalent(1, 2);
                    UnholyEbonFever = GetTalent(1, 3);
                }
                if (Me.Level >= 57)
                {
                    UnholyEpidemic = GetTalent(2, 1);
                    UnholyPestilentPustules = GetTalent(2, 2);
                    UnholyBlightedRuneWeapon = GetTalent(2, 3);
                }
                if (Me.Level >= 58)
                {
                    UnholyUnholyFrenzy = GetTalent(3, 1);
                    UnholyCastigator = GetTalent(3, 2);
                    UnholyClawingShadows = GetTalent(3, 3);
                }
                if (Me.Level >= 60)
                {
                    UnholySludgeBelcher = GetTalent(4, 1);
                    UnholyAsphyxiate = GetTalent(4, 2);
                    UnholyDebilitatingInfestation = GetTalent(4, 3);
                }
                if (Me.Level >= 75)
                {
                    UnholySpellEater = GetTalent(5, 1);
                    UnholyCorpseShield = GetTalent(5, 2);
                    UnholyLingeringApparition = GetTalent(5, 3);
                }
                if (Me.Level >= 90)
                {
                    UnholyShadowInfusion = GetTalent(6, 1);
                    UnholyNecrosis = GetTalent(6, 2);
                    UnholyInfectedClaws = GetTalent(6, 3);
                }
                if (Me.Level >= 100)
                {
                    UnholyDarkArbiter = GetTalent(7, 1);
                    UnholyDefile = GetTalent(7, 2);
                    UnholySoulReaper = GetTalent(7, 3);
                }
            }
            if (Me.Specialization == WoWSpec.DeathKnightFrost)
            {
                if (Me.Level >= 56)
                {
                    FrostShatteringStrikes = GetTalent(1, 1);
                    FrostIcyTalons = GetTalent(1, 2);
                    FrostMurderousEfficiency = GetTalent(1, 3);
                }
                if (Me.Level >= 57)
                {
                    FrostFreezingFog = GetTalent(2, 1);
                    FrostFrozenPulse = GetTalent(2, 2);
                    FrostHornOfWinter = GetTalent(2, 3);
                }
                if (Me.Level >= 58)
                {
                    FrostIcecap = GetTalent(3, 1);
                    FrostHungeringRuneWeapon = GetTalent(3, 2);
                    FrostAvalanche = GetTalent(3, 3);
                }
                if (Me.Level >= 60)
                {
                    FrostAbominationsMight = GetTalent(4, 1);
                    FrostBlindingSleet = GetTalent(4, 2);
                    FrostWinterIsComing = GetTalent(4, 3);
                }
                if (Me.Level >= 75)
                {
                    FrostVolatileShielding = GetTalent(5, 1);
                    FrostPermafrost = GetTalent(5, 2);
                    FrostWhiteWalker = GetTalent(5, 3);
                }
                if (Me.Level >= 90)
                {
                    FrostFrostscythe = GetTalent(6, 1);
                    FrostRunicAttenuation = GetTalent(6, 2);
                    FrostGatheringStorm = GetTalent(6, 3);
                }
                if (Me.Level >= 100)
                {
                    FrostObliteration = GetTalent(7, 1);
                    FrostBreathOfSindragosa = GetTalent(7, 2);
                    FrostGlacialAdvance = GetTalent(7, 3);
                }
            }

            PrintTalents();
        }

        public static void SetTalEvents()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                if (Me.Level < 100)
                {
                    Lua.Events.AttachEvent("PLAYER_LEVEL_UP", PlayerLeveledUp);
                }
                Lua.Events.AttachEvent("CHARACTER_POINTS_CHANGED", InitializeTalents);
                Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", InitializeTalents);
                Lua.Events.AttachEvent("PLAYER_SPECIALIZATION_CHANGED", TalentSpecChanged);
                Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", InitializeTalents);
            }
        }

        private static void TalentSpecChanged(object sender, LuaEventArgs args)
        {
            //TODO: Remove after all 3 Spec supported
            Logging.Write("Spec changed to {0}.", Me.Specialization);
            InitTalents();
            //onSpecChanged(args);
        }

        private static void InitializeTalents(object sender, LuaEventArgs args)
        {
            InitTalents();
        }

        private static void PlayerLeveledUp(object sender, LuaEventArgs args)
        {
            Logging.Write(string.Format($"Player leveled up!  Now level {args.Args[0]}"));
            InitTalents();
        }

        private static void PrintTalents()
        {
            Logging.Write("Selected talents for {0}", Me.Specialization.ToString().AddSpaces());
            if (Me.Specialization == WoWSpec.DeathKnightUnholy)
            {
                // tier 1
                if (UnholyAllWillServe)
                {
                    PrintTalent("All Will Serve", 1);
                }
                else if (UnholyBurstingSores)
                {
                    PrintTalent("Bursting Sores", 1);
                }
                else if (UnholyEbonFever)
                {
                    PrintTalent("Ebon Fever", 1);
                }

                // tier 2
                if (UnholyEpidemic)
                {
                    PrintTalent("Epidemic", 2);
                }
                else if (UnholyPestilentPustules)
                {
                    PrintTalent("Pestilent Pustules", 2);
                }
                else if (UnholyBlightedRuneWeapon)
                {
                    PrintTalent("Blighted Rune Weapon", 2);
                }

                // tier 3
                if (UnholyUnholyFrenzy)
                {
                    PrintTalent("Unholy Frenzy", 3);
                }
                else if (UnholyCastigator)
                {
                    PrintTalent("Castigator", 3);
                }
                else if (UnholyClawingShadows)
                {
                    PrintTalent("Clawing Shadows", 3);
                }

                // tier 4
                if (UnholySludgeBelcher)
                {
                    PrintTalent("Sludge Belcher", 4);
                }
                else if (UnholyAsphyxiate)
                {
                    PrintTalent("Asphyxiate", 4);
                }
                else if (UnholyDebilitatingInfestation)
                {
                    PrintTalent("Debilitating Infestation", 4);
                }

                // tier 5
                if (UnholySpellEater)
                {
                    PrintTalent("Spell Eater", 5);
                }
                else if (UnholyCorpseShield)
                {
                    PrintTalent("Corpse Shield", 5);
                }
                else if (UnholyLingeringApparition)
                {
                    PrintTalent("Lingering Apparition", 5);
                }

                // tier 6
                if (UnholyShadowInfusion)
                {
                    PrintTalent("Shadow Infusion", 6);
                }
                else if (UnholyNecrosis)
                {
                    PrintTalent("Necrosis", 6);
                }
                else if (UnholyInfectedClaws)
                {
                    PrintTalent("Infected Claws", 6);
                }

                // tier 7
                if (UnholyDarkArbiter)
                {
                    PrintTalent("Dark Arbiter", 7);
                }
                else if (UnholyDefile)
                {
                    PrintTalent("Defile", 7);
                }
                else if (UnholySoulReaper)
                {
                    PrintTalent("Soul Reaper", 7);
                }
            }

            if (Me.Specialization == WoWSpec.DeathKnightFrost)
            {
                // tier 1
                if (FrostShatteringStrikes)
                {
                    PrintTalent("Shattering Strikes", 1);
                }
                else if (FrostIcyTalons)
                {
                    PrintTalent("Icy Talons", 1);
                }
                else if (FrostMurderousEfficiency)
                {
                    PrintTalent("Murderous Efficiency", 1);
                }

                // tier 2
                if (FrostFreezingFog)
                {
                    PrintTalent("Freezing Fog", 2);
                }
                else if (FrostFrozenPulse)
                {
                    PrintTalent("Frozen Pulse", 2);
                }
                else if (FrostHornOfWinter)
                {
                    PrintTalent("Horn of Winter", 2);
                }

                // tier 3
                if (FrostIcecap)
                {
                    PrintTalent("Icecap", 3);
                }
                else if (FrostHungeringRuneWeapon)
                {
                    PrintTalent("Hungering Rune Weapon", 3);
                }
                else if (FrostAvalanche)
                {
                    PrintTalent("Avalanche", 3);
                }

                // tier 4
                if (FrostAbominationsMight)
                {
                    PrintTalent("Abomination's Might", 4);
                }
                else if (FrostBlindingSleet)
                {
                    PrintTalent("Blinding Sleet", 4);
                }
                else if (FrostWinterIsComing)
                {
                    PrintTalent("Winter is Coming", 4);
                }

                // tier 5
                if (FrostVolatileShielding)
                {
                    PrintTalent("Volatile Shielding", 5);
                }
                else if (FrostPermafrost)
                {
                    PrintTalent("Permafrost", 5);
                }
                else if (FrostWhiteWalker)
                {
                    PrintTalent("White Walker", 5);
                }

                // tier 6
                if (FrostFrostscythe)
                {
                    PrintTalent("Frostscythe", 6);
                }
                else if (FrostRunicAttenuation)
                {
                    PrintTalent("Runic Attenuation", 6);
                }
                else if (FrostGatheringStorm)
                {
                    PrintTalent("Gathering Storm", 6);
                }

                // tier7
                if (FrostObliteration)
                {
                    PrintTalent("Obliteration", 7);
                }
                else if (FrostBreathOfSindragosa)
                {
                    PrintTalent("Breath of Sindragosa)", 7);
                }
                else if (FrostGlacialAdvance)
                {
                    PrintTalent("Glacial Advance", 7);
                }
            }
        }

        private static void PrintTalent(string name, int tier)
        {
            Logging.Write("Tier {0}: {1}", tier, name);
        }

        private static void UpdateTalentManager(object sender, LuaEventArgs args)
        {
            Logging.Write("------------------");
            Logging.Write("Talents changed...");
            InitTalents();
        }

        #region 110 level

        //not implemented yet

        #endregion
    }
}