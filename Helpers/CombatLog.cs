/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System.Collections.Generic;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Helpers
{
    internal class CombatLog : LuaEventArgs
    {
        public CombatLog(string eventName, uint fireTimeStamp, object[] args)
            : base(eventName, fireTimeStamp, args)
        {
        }

        public double Timestamp
        {
            get { return (double) Args[0]; }
        }

        public string Event
        {
            get { return Args[1].ToString(); }
        }

        // Is this a string? bool? what? What the hell is it even used for?
        // it's a boolean, and it doesn't look like it has any real impact codewise apart from maybe to break old addons? - exemplar 4.1
        public string HideCaster
        {
            get { return Args[2].ToString(); }
        }

        public WoWGuid SourceGuid
        {
            get { return ArgToGuid(Args[3]); }
        }

        public WoWUnit SourceUnit
        {
            get
            {
                var cachedSourceGuid = SourceGuid.IsValid ? SourceGuid : Helpers.Spell.LastCastTarget.Guid;
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>(true, true).FirstOrDefault(
                        o => o.IsValid && (o.Guid == cachedSourceGuid || o.Guid == cachedSourceGuid));
            }
        }

        public string SourceName
        {
            get { return Args[4].ToString(); }
        }

        public int SourceFlags
        {
            get { return (int) (double) Args[5]; }
        }

        public WoWGuid DestGuid
        {
            get { return ArgToGuid(Args[7]); }
        }

        public WoWUnit DestUnit
        {
            get
            {
                var cachedDestGuid = DestGuid;
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>(true, true).FirstOrDefault(
                        o => o.IsValid && (o.Guid == cachedDestGuid || o.Guid == cachedDestGuid)) ??
                    Helpers.Spell.LastCastTarget;
            }
        }

        public string DestName
        {
            get { return Args[8].ToString(); }
        }

        public int DestFlags
        {
            get { return (int) (double) Args[9]; }
        }

        public int SpellId
        {
            get { return (int) (double) Args[11]; }
        }

        public WoWSpell Spell
        {
            get { return WoWSpell.FromId(SpellId); }
        }

        public string SpellName
        {
            get { return Args[12].ToString(); }
        }

        public WoWSpellSchool SpellSchool
        {
            get { return (WoWSpellSchool) (int) (double) Args[13]; }
        }

        public object[] SuffixParams
        {
            get
            {
                var args = new List<object>();
                for (var i = 11; i < Args.Length; i++)
                {
                    if (Args[i] != null)
                    {
                        args.Add(args[i]);
                    }
                }
                return args.ToArray();
            }
        }

        private static WoWGuid ArgToGuid(object o)
        {
            var s = o.ToString();
            WoWGuid guid;
            if (!WoWGuid.TryParseFriendly(s, out guid))
                guid = WoWGuid.Empty;

            return guid;
        }
    }
}