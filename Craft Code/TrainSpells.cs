using System;
using System.Collections.Generic;
using System.Threading;
using wManager.Wow.Helpers;

namespace Craft_Code
{
    public static class TrainSpells
    {
        public static int PlayerMoney = Lua.LuaDoString<int>("return GetMoney()");
        public static int GetNumTrainerServices()
        {
            return Lua.LuaDoString<int>("return GetNumTrainerServices();");
        }

        public static void SetAvailable()
        {
            Lua.LuaDoString(@"SetTrainerServiceTypeFilter('available', 1);
                            SetTrainerServiceTypeFilter('unavailable', 0);
                            SetTrainerServiceTypeFilter('used', 0);"
            );  
        }


        public static void TrainSpell(Boolean LearnAllSpells = false, String SpellName = null, List<string> SpellList = null)
        {
            try
            {
                for (int s = 0; s <= GetNumTrainerServices(); s++)
                {
                    SetAvailable();
                    int SpellCost = Lua.LuaDoString<int>("local moneyCost = GetTrainerServiceCost(" + s + "); return moneyCost;");
                    string[] GetTrainerServiceInfo = Lua.Wow.GetTrainerServiceInfo<string[]>(s);

                    //Learn Single Spell
                    if (SpellName.Length > 0)
                    {
                        if (GetTrainerServiceInfo[0].ToLower() == SpellName.ToLower().Replace("'", "\'"))
                        {
                            Lua.LuaDoString("SelectTrainerService(" + s + ")");
                            if (SpellCost < PlayerMoney)
                            {
                                Lua.LuaDoString("BuyTrainerService(" + s + ")");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //Learn Every Single Spell in Lists.
                    else if (SpellList.Count > 0)
                    {
                        foreach (string Spell in SpellList)
                        {
                            if (GetTrainerServiceInfo[0].ToLower() == Spell.ToLower().Replace("'", "\'"))
                            {
                                Lua.LuaDoString("BuyTrainerService(" + s + ")");
                            }
                        }
                    }
                    //Learn every single spell.
                    else if (LearnAllSpells)
                    {
                        Lua.LuaDoString("BuyTrainerService(" + s + ")");
                    }
                }
            }
            finally
            {
                Thread.Sleep(Usefuls.LatencyReal + 1000);
                Lua.LuaDoString("CloseTrainer();");
            }
        }
    }
}
