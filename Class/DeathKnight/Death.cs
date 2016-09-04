using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ScourgeBloom.Helpers;
using ScourgeBloom.Settings;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ScourgeBloom.Class.DeathKnight
{
    public class Death
    {
        private static LocalPlayer Me => StyxWoW.Me;

        public static async Task<bool> DeathBehavor()
        {
            if (Me.IsDead && GeneralSettings.Instance.AutoReleaseSpirit)
            {
                Log.WriteLog(DateTime.Now.ToString("mm:ss:ffffff") + " RepopMe()");
                Lua.DoString("RepopMe()");
            }

            if (Me.IsGhost)
            {
                if (Battlegrounds.IsInsideBattleground || Me.CurrentMap.Name == "Ashran")
                {
                    Log.WriteLog(DateTime.Now.ToString("mm:ss:ffffff") + " Waiting for spirit ressurect");
                    await Coroutine.Sleep(1500);
                }
                else
                {
                    Log.WriteLog(DateTime.Now.ToString("mm:ss:ffffff") + " Moving to corpse!");
                    await CommonCoroutines.MoveTo(Me.CorpsePoint);
                }
            }

            return false;
        }
    }
}