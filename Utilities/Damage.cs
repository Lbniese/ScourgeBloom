using System;

namespace ScourgeBloom.Utilities
{
    public class Damage
    {
        public Damage(DateTime time, long amt)
        {
            Time = time;
            Amount = amt;
        }

        public DateTime Time { get; set; }
        public long Amount { get; set; }
    }
}