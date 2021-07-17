using System;
using wManager.Wow.Helpers;



namespace BankLib
{
    public static class Bank
    {
        public static void PlaceItemInGuildBankbyName(Int32 Tab, String ItemName)
        {
            SetCurrentGuildBankTab(Tab);
            try
            {
                for(Int32 Bag = 0; Bag <= 4; Bag++)
                {
                    for(Int32 Slot = 1; Slot <= GetContainerNumSlots(Bag); Slot++)
                    {
                        String BagItem = GetContainerItemLink(Bag,Slot)[0];
                        if(BagItem == ItemName)
                        {
                            UseContainerItem(Bag, Slot);
                            break;
                        }
                    }
                } 
            }
            catch { }
        }

        public static void SplitContainerItem(String ItemName, Int32 SplitAMount)
        {
            try
            {
                for (Int32 Bag = 0; Bag <= 4; Bag++)
                {
                    for (Int32 Slot = 1; Slot <= GetContainerNumSlots(Bag); Slot++)
                    {
                        String BagItem = GetContainerItemLink(Bag, Slot)[0];
                        if (BagItem == ItemName)
                        {
                            int Count = int.Parse(GetContainerItemInfo(Bag, Slot)[1]);
                            if(Count > SplitAMount)
                            {
                                SplitContainerItem(Bag, Slot, SplitAMount);
                            }

                        }
                    }
                }
            }
            catch { }
        }

        private static void SetCurrentGuildBankTab(Int32 n)
        {
            try
            {
                for(int BanksLot = 0; BanksLot <= GetNumGuildBankTabs(); BanksLot++ )
                {
                    if(BanksLot == n)
                    {
                        Lua.LuaDoString("SetCurrentGuildBankTab(" + BanksLot + ")");
                    }
                }
            }
            catch { }
        }
        private static void UseContainerItem(Int32 B, Int32 S)
        {
            try
            {
                Lua.LuaDoString("UseContainerItem("+ B +","+ S +")");
            }
            catch { }
        }

        private static int GetNumGuildBankTabs()
        {
            return Lua.LuaDoString<int>("return GetNumGuildBankTabs();");
        }

        private static String[] GetContainerItemLink(Int32 b, Int32 s)
        {
            return Lua.Wow.GetContainerItemLink<String[]>(b, s);
        }

        private static int GetContainerNumSlots(Int32 Num)
        {
            return Lua.LuaDoString<int>("return GetContainerNumSlots(" + Num + ");");
        }
        
        private static String[] GetContainerItemInfo(Int32 b, Int32 s)
        {
            return Lua.Wow.GetContainerItemInfo<String[]>(b, s);
        }
        private static void SplitContainerItem(Int32 bag, Int32 slot, Int32 amount)
        {
            try
            {
                Lua.LuaDoString("SplitContainerItem("+ bag +"," + slot +","+ amount +")");
            }
            finally
            {
                Bag.PutSelectedItemInBag();
            }
        }

    }
}
