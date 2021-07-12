using robotManager.Helpful;
using System.Collections.Generic;
using wManager.Wow.Helpers;

namespace Dungeon.Code
{
    public static class Dungeon
    {
        private static List<int> GetLFDChoiceOrder()
        {
            Lua.LuaDoString("ShowUIPanel(LFDParentFrame);");
            try
            {
                return Lua.LuaDoString<List<int>>("return unpack(GetLFDChoiceOrder());");
            }
            finally
            {
                Lua.LuaDoString("HideUIPanel(LFDParentFrame);");
            }
        }

        private static bool LFGJoinable(int Num)
        {
            var ret = "if not LFGIsIDHeader(" + Num + ") and " +
                            "IsLFGDungeonJoinable(" + Num + ") then " +
                                "return true " +
                            "else " +
                                "return false; " +
                        "end";
            return Lua.LuaDoString<bool>(ret);
        }


        public static void SelectDungeonByName(string DungeonName, bool IsHeroicDungeon, bool EnableDisable)
        {
            try
            {
                foreach (int LFD in GetLFDChoiceOrder())
                {
                    string[] LFGDungeonInfo = Lua.Wow.GetLFGDungeonInfo<string[]>(LFD);
                    if (LFGJoinable(LFD))
                    {
                        if (LFGDungeonInfo[0].ToLower() == DungeonName.ToLower())
                        {
                            if (IsHeroicDungeon)
                            {
                                if (int.Parse(LFGDungeonInfo[1]) == 5)
                                {
                                    Lua.LuaDoString("LFDList_SetDungeonEnabled(" + LFD + "," + EnableDisable.ToString().ToLower() + ");");
                                    break;
                                }
                            }
                            else if (int.Parse(LFGDungeonInfo[1]) == 1)
                            {
                                Lua.LuaDoString("LFDList_SetDungeonEnabled(" + LFD + "," + EnableDisable.ToString().ToLower() + ");");
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Lua.LuaDoString("LFDQueueFrameSpecificList_Update();");
            }
        }
    }
}