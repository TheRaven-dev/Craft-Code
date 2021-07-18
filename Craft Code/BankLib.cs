using System;
using wManager.Wow.Helpers;


namespace BankLib
{
    public static class Bank
    {
        public static void PlaceItemInGuildBankbyName(String ItemName, Int32 Tab = 1)
        {
            Lua.LuaDoString("SetCurrentGuildBankTab(" + Tab + ")");
            try
            {
                for(Int32 Bag = 0; Bag <= 4; Bag++)
                {
                    for(Int32 Slot = 1; Slot <= GetContainerNumSlots(Bag); Slot++)
                    {
                        String BagItem = GetContainerItemLink(Bag,Slot)[0];
                        String[] GetItemInfo = Lua.Wow.GetItemInfo<String[]>(BagItem);
                        if (GetItemInfo[0] == ItemName)
                        {
                            Lua.LuaDoString("UseContainerItem(" + Bag + "," + Slot + ")");
                            break;
                        }
                    }
                } 
            }
            catch { }
        }

        private static String[] GetContainerItemLink(Int32 b, Int32 s)
        {
            return Lua.Wow.GetContainerItemLink<String[]>(b, s);
        }

        private static int GetContainerNumSlots(Int32 Num)
        {
            return Lua.LuaDoString<int>("return GetContainerNumSlots(" + Num + ");");
        }
    }
}
