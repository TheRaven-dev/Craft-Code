﻿using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Skill
{
    internal static class Craft
    {
        private static int CraftCount = 0;
        public static bool HasRecipeByName(string ProfessionName, string RecipeName)
        {
            OpenTradeFrame(ProfessionName);
            try
            {
                for (int i = 0; i <= GetNumTradeSkills(); i++)
                {
                    string[] TradeSkillName = Lua.Wow.GetTradeSkillInfo<string[]>(i);
                    if (TradeSkillName[0].ToLower() == RecipeName.ToLower())
                    {
                        Lua.LuaDoString("SelectTradeSkill(" + i + ");");
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                HideFrame();
            }

        }

        public static void CraftItemByName(string profession, string itemName, int Quantity = 0)
        {
            OpenTradeFrame(profession);
            try
            {
                for (int i = 0; i <= GetNumTradeSkills(); i++)
                {
                    string[] TradeSkillName = Lua.Wow.GetTradeSkillInfo<string[]>(i);
                    if (TradeSkillName[0].ToLower() == itemName.ToLower())
                    {
                        Lua.LuaDoString("SelectTradeSkill(" + i + ");");
                        int CraftQuantity = int.Parse(TradeSkillName[2]);
                        if (CraftQuantity > 0)
                        {
                            if (Quantity <= 0)
                            {
                                CraftCount = CraftQuantity;
                                Lua.LuaDoString("DoTradeSkill(" + i + ", " + CraftQuantity + ");");
                            }
                            else
                            {
                                CraftCount = Quantity;
                                Lua.LuaDoString("DoTradeSkill(" + i + ", " + Quantity + ");");
                            }
                        }
                        else
                        {
                            Logging.Write("You do not have enough mats to craft " + itemName);
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (CraftCount > 0)
                {
                    int Time = (3000 + Usefuls.LatencyReal) * CraftCount;
                    MovementManager.StopMoveTo(false, Time);
                    DateTime time = DateTime.Now.AddMilliseconds(Time);
                    while (DateTime.Now < time && !ObjectManager.Me.InCombatFlagOnly && !ObjectManager.Me.InCombat)
                    {
                        Thread.Sleep(300);
                    }
                }
                HideFrame();
            }
        }

        private static int GetNumTradeSkills()
        {
            return Lua.LuaDoString<int>("return GetNumTradeSkills();");
        }

        private static bool TradeSkillFrame()
        {
            return Lua.LuaDoString<bool>("return TradeSkillFrame:IsVisible();");
        }

        private static void HideFrame()
        {
            try
            {
                if (TradeSkillFrame())
                {
                    Lua.LuaDoString("HideUIPanel(TradeSkillFrame)");
                }
            }
            catch { }
        }

        private static void OpenTradeFrame(string ProfessionName)
        {
            try
            {
                if (!TradeSkillFrame())
                {
                    Lua.LuaDoString("CastSpellByName(\"" + ProfessionName + "\")");
                }
            }
            catch { }
        }
    }
}
