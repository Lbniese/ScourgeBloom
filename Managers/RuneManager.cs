using System;
using System.Collections.Generic;
using System.Dynamic;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Managers
{
    public abstract class RuneManager : DynamicObject
    {
        public delegate WoWUnit GetUnitDelegate();

        public abstract class UnitBasedProxy : RuneManager
        {
            public readonly GetUnitDelegate GetUnit;

            protected UnitBasedProxy(GetUnitDelegate del)
            {
                GetUnit = del;
            }

            protected bool Equals(UnitBasedProxy other)
            {
                return Equals(GetUnit, other.GetUnit);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((UnitBasedProxy) obj);
            }

            public override int GetHashCode()
            {
                return GetUnit?.GetHashCode() ?? 0;
            }

            public static bool operator ==(WoWUnit h1, UnitBasedProxy val)
            {
                return val != null && h1 == val.GetUnit();
            }

            public static bool operator !=(WoWUnit h1, UnitBasedProxy val)
            {
                return val != null && h1 != val.GetUnit();
            }

            public static bool operator ==(UnitBasedProxy val, WoWUnit h1)
            {
                return val != null && h1 == val.GetUnit();
            }

            public static bool operator !=(UnitBasedProxy val, WoWUnit h1)
            {
                return val != null && h1 != val.GetUnit();
            }
        }

        public abstract class ResourceProxy
        {
            public abstract RuneProxy.MagicValueType GetPercent { get; }
            public abstract RuneProxy.MagicValueType GetMax { get; }
            public abstract RuneProxy.MagicValueType GetCurrent { get; }

            public class ProxyCacheEntry
            {
                private readonly Dictionary<string, CacheValue> _values = new Dictionary<string, CacheValue>();

                public CacheValue this[string name]
                {
                    get
                    {
                        if (!_values.ContainsKey(name)) _values[name] = new CacheValue();
                        return _values[name];
                    }
                }

                public class CacheValue
                {
                    public static int CacheInterval = 1;
                }
            }
        }
    }

    public class RuneProxy : RuneManager.ResourceProxy
    {
        private readonly RuneType _type;

        public RuneProxy(RuneType type)
        {
            _type = type;
        }

        public double Frac
        {
            get
            {
                if (_type == RuneType.Blood)
                {
                    var o1 = get_rune_frac(1);
                    var o2 = get_rune_frac(2);
                    var m = Math.Max(o1, o2);
                    var t = 200 - m;
                    return t/100;
                }
                if (_type == RuneType.Frost)
                {
                    var o1 = get_rune_frac(5);
                    var o2 = get_rune_frac(6);
                    var m = Math.Max(o1, o2);
                    var t = 200 - m;
                    return t/100;
                }
                if (_type == RuneType.Unholy)
                {
                    var o1 = get_rune_frac(3);
                    var o2 = get_rune_frac(4);
                    var m = Math.Max(o1, o2);
                    var t = 200 - m;
                    return t/100;
                }
                return 0;
            }
        }

        public override MagicValueType GetPercent => new MagicValueType(Frac);

        public override MagicValueType GetCurrent => new MagicValueType(StyxWoW.Me.GetRuneCount(_type));

        public override MagicValueType GetMax => new MagicValueType(2);

        private double get_rune_frac(int id)
        {
            //"start, duration, runeReady = GetRuneCooldown("+id+"); if (runeReady) then return 0 else local t= ((duration-(GetTime()-start))*100/duration); if (t > 100) then t = t; end return t end"
            return
                Lua.GetReturnVal<double>(
                    "start, duration, runeReady = GetRuneCooldown(" + id +
                    "); if (runeReady) then return 0 else local t= ((duration-(GetTime()-start))*100/duration); if (t > 100) then t = t; end return t end",
                    0);
        }

        /*public static bool operator false(RuneProxy x)
    {
        return x.frac < 1;
    }

    public static bool operator true(RuneProxy x)
    {
        return x.frac >= 1;
    }*/

        public class Gcd : MagicValueType
        {
            public Gcd(decimal v, decimal max, decimal remains) : base(v)
            {
                Max = new MagicValueType(max);
                Remains = new MagicValueType(remains);
            }

            public MagicValueType Max { get; set; }
            public MagicValueType Remains { get; set; }
        }

        public class MagicValueType
        {
            public static MagicValueType Zero = new MagicValueType(0);
            public static MagicValueType False = new MagicValueType(0);
            public static MagicValueType True = !False;
            public static MagicValueType NaN = new MagicValueType(decimal.MinValue);

            public MagicValueType(bool v)
            {
                Nat = v ? 1 : 0;
            }

            public MagicValueType(decimal v)
            {
                Nat = v;
            }

            public MagicValueType(int v)
            {
                Nat = v;
            }

            public MagicValueType(double v)
            {
                Nat = (decimal) v;
            }

            public MagicValueType(MagicValueType v)
            {
                Nat = v.Nat;
            }

            public decimal Nat { get; }

            public MagicValueType max => this;

            public override string ToString()
            {
                return this == NaN ? "NaN" : Nat + "";
            }

            public bool Equals(MagicValueType other)
            {
                return Nat.Equals(other.Nat);
            }

            public override bool Equals(object obj)
            {
                var d = obj as MagicValueType;
                return d?.Nat == Nat;
            }

            public override int GetHashCode()
            {
                return Nat.GetHashCode();
            }

            //MagicValueType Operators - Both Magic Double
            public static MagicValueType operator <(MagicValueType op1, MagicValueType op2)
            {
                //SimcraftImpl.LogDebug(op1 + " < " + op2);
                return new MagicValueType(op1.Nat < op2.Nat);
            }

            public static MagicValueType operator >(MagicValueType op1, MagicValueType op2)
            {
                //SimcraftImpl.LogDebug(op1+" > "+op2);
                return new MagicValueType(op1.Nat > op2.Nat);
            }

            public static MagicValueType operator <=(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat <= op2.Nat);
            }

            public static MagicValueType operator >=(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat >= op2.Nat);
            }

            public static MagicValueType operator !=(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && op1 != null && op1.Nat != op2.Nat);
            }

            public static MagicValueType operator ==(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && op1 != null && op1.Nat == op2.Nat);
            }

            public static MagicValueType operator -(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat - op2.Nat);
            }

            public static MagicValueType operator +(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat + op2.Nat);
            }

            public static MagicValueType operator *(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat*op2.Nat);
            }

            public static MagicValueType operator /(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat/op2.Nat);
            }

            public static MagicValueType operator %(MagicValueType op1, MagicValueType op2)
            {
                return new MagicValueType(op1.Nat%op2.Nat);
            }

            //MagicValueType Operators - Magic , normal
            public static MagicValueType operator <(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat < (decimal) op2);
            }

            public static MagicValueType operator >(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat > (decimal) op2);
            }

            public static MagicValueType operator <=(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat <= (decimal) op2);
            }

            public static MagicValueType operator >=(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat >= (decimal) op2);
            }

            public static MagicValueType operator !=(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1 != null && op1.Nat != (decimal) op2);
            }

            public static MagicValueType operator ==(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1 != null && op1.Nat == (decimal) op2);
            }

            public static MagicValueType operator -(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat - (decimal) op2);
            }

            public static MagicValueType operator +(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat + (decimal) op2);
            }

            public static MagicValueType operator *(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat*(decimal) op2);
            }

            public static MagicValueType operator /(MagicValueType op1, double op2)
            {
                return new MagicValueType(op1.Nat/(decimal) op2);
            }

            //MagicValueType Operators - Normal, Magic
            public static MagicValueType operator <(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 < op2.Nat);
            }

            public static MagicValueType operator >(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 > op2.Nat);
            }

            public static MagicValueType operator <=(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 <= op2.Nat);
            }

            public static MagicValueType operator >=(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 >= op2.Nat);
            }

            public static MagicValueType operator !=(double op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && (decimal) op1 != op2.Nat);
            }

            public static MagicValueType operator ==(double op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && (decimal) op1 == op2.Nat);
            }

            public static MagicValueType operator -(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 - op2.Nat);
            }

            public static MagicValueType operator +(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1 + op2.Nat);
            }

            public static MagicValueType operator *(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1*op2.Nat);
            }

            public static MagicValueType operator /(double op1, MagicValueType op2)
            {
                return new MagicValueType((decimal) op1/op2.Nat);
            }

            //MagicValueType Operators - Magic , normal
            public static MagicValueType operator <(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat < op2);
            }

            public static MagicValueType operator >(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat > op2);
            }

            public static MagicValueType operator <=(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat <= op2);
            }

            public static MagicValueType operator >=(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat >= op2);
            }

            public static MagicValueType operator !=(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1 != null && op1.Nat != op2);
            }

            public static MagicValueType operator ==(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1 != null && op1.Nat == op2);
            }

            public static MagicValueType operator -(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat - op2);
            }

            public static MagicValueType operator +(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat + op2);
            }

            public static MagicValueType operator *(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat*op2);
            }

            public static MagicValueType operator /(MagicValueType op1, int op2)
            {
                return new MagicValueType(op1.Nat/op2);
            }

            //MagicValueType Operators - Normal, Magic
            public static MagicValueType operator <(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 < op2.Nat);
            }

            public static MagicValueType operator >(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 > op2.Nat);
            }

            public static MagicValueType operator <=(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 <= op2.Nat);
            }

            public static MagicValueType operator >=(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 >= op2.Nat);
            }

            public static MagicValueType operator !=(int op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && op1 != op2.Nat);
            }

            public static MagicValueType operator ==(int op1, MagicValueType op2)
            {
                return new MagicValueType(op2 != null && op1 == op2.Nat);
            }

            public static MagicValueType operator -(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 - op2.Nat);
            }

            public static MagicValueType operator +(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1 + op2.Nat);
            }

            public static MagicValueType operator *(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1*op2.Nat);
            }

            public static MagicValueType operator /(int op1, MagicValueType op2)
            {
                return new MagicValueType(op1/op2.Nat);
            }

            //Bool Tricks

            //MagicValueType Operators - Magic , bool
            public static MagicValueType operator <(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat < (op2 ? 1 : 0));
            }

            public static MagicValueType operator >(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat > (op2 ? 1 : 0));
            }

            public static MagicValueType operator <=(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat <= (op2 ? 1 : 0));
            }

            public static MagicValueType operator >=(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat >= (op2 ? 1 : 0));
            }

            public static MagicValueType operator !=(MagicValueType op1, bool op2)
            {
                return !(op1 == op2);
            }

            public static MagicValueType operator ==(MagicValueType op1, bool op2)
            {
                if (op2 && op1 > 0) return new MagicValueType(true);
                if (!op2 && op1 > 0) return new MagicValueType(false);
                if (!op2 && op1 <= 0) return new MagicValueType(true);
                return new MagicValueType(false);
            }

            public static MagicValueType operator -(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat - (op2 ? 1 : 0));
            }

            public static MagicValueType operator +(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat + (op2 ? 1 : 0));
            }

            public static MagicValueType operator *(MagicValueType op1, bool op2)
            {
                return new MagicValueType(op1.Nat*(op2 ? 1 : 0));
            }

            //MagicValueType Operators - Normal, Magic
            public static MagicValueType operator <(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) < op2.Nat);
            }

            public static MagicValueType operator >(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) > op2.Nat);
            }

            public static MagicValueType operator <=(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) <= op2.Nat);
            }

            public static MagicValueType operator >=(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) >= op2.Nat);
            }

            public static MagicValueType operator !=(bool op1, MagicValueType op2)
            {
                return !(op1 == op2);
            }

            public static MagicValueType operator ==(bool op1, MagicValueType op2)
            {
                if (op1 && op2 > 0) return new MagicValueType(true);
                if (!op1 && op2 > 0) return new MagicValueType(false);
                if (!op1 && op2 <= 0) return new MagicValueType(true);
                return new MagicValueType(false);
            }

            public static MagicValueType operator -(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) - op2.Nat);
            }

            public static MagicValueType operator +(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0) + op2.Nat);
            }

            public static MagicValueType operator *(bool op1, MagicValueType op2)
            {
                return new MagicValueType((op1 ? 1 : 0)*op2.Nat);
            }

            //Implicit Conversions
            public static implicit operator bool(MagicValueType d)
            {
                return d.Nat > 0;
            }

            //Implicit Conversions
            public static implicit operator decimal(MagicValueType d)
            {
                return d.Nat;
            }

            /*public static implicit operator MagicValueType(bool d)
        {
            return new MagicValueType(d);
        }*/

            public static implicit operator double(MagicValueType d)
            {
                return (double) d.Nat;
            }

            public static implicit operator int(MagicValueType d)
            {
                return (int) d.Nat;
            }

            public static MagicValueType operator !(MagicValueType d)
            {
                //var _d = -d.boxee;
                return new MagicValueType(d.Nat > 0 ? 0 : 1);
            }
        }
    }
}