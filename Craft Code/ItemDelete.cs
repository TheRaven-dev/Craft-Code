using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Helpers;

public static class ItemLib
{
    public static void DeleteItem(string ItemName, int leaveAmount = 0, int TimeDelay = 1)
    {
        var ItemQuantity = ItemsManager.GetItemCountByNameLUA(ItemName) - leaveAmount;
        if (string.IsNullOrWhiteSpace(ItemName) || ItemQuantity <= 0)
            return;
        try
        {
            Int32 Deleted = 0;
            for (Int32 B = 0; B <= 4; B++)
            {
                String GetBagName = Lua.LuaDoString<String>("return GetBagName(" + B + ");");
                if (!String.IsNullOrWhiteSpace(GetBagName))
                {
                    Int32 GetContainerNumSlots = Lua.LuaDoString<Int32>("return GetContainerNumSlots(" + B + ");");
                    for (Int32 S = 1; S <= GetContainerNumSlots; S++)
                    {
                        String ItemLink = Lua.LuaDoString<String>("local itemLink = GetContainerItemLink(" + B + "," + S + "); return itemLink;");
                        String[] GetItemInfo = Lua.Wow.GetItemInfo<String[]>(ItemLink);
                        if (!String.IsNullOrWhiteSpace(GetItemInfo[0]))
                        {
                            Int32 stackCount = Lua.LuaDoString<Int32>("local _, stackCount = GetContainerItemInfo(" + B + ", " + S + "); return stackCount;");
                            Int32 ItemLeft = ItemQuantity - Deleted;
                            if (GetItemInfo[0] == ItemName)
                            {
                                if (ItemLeft > 0)
                                {
                                    if (stackCount < 1)
                                    {
                                        Lua.LuaDoString("PickupContainerItem(" + B + ", " + S + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Thread.Sleep(TimeDelay);
                                        Deleted = Deleted + 1;
                                    }
                                    else if (ItemLeft > stackCount)
                                    {
                                        Lua.LuaDoString("SplitContainerItem(" + B + ", " + S + ", " + stackCount + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Thread.Sleep(TimeDelay);
                                        Deleted = Deleted + stackCount;
                                    }
                                    else
                                    {
                                        Lua.LuaDoString("SplitContainerItem(" + B + ", " + S + ", " + ItemLeft + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Thread.Sleep(TimeDelay);
                                        Deleted = Deleted + ItemLeft;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logging.Write("DeleteItem > " + e);
        }
    }
}