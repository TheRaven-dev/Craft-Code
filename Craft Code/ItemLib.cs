using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Helpers;

public static class ItemLib
{
    public enum SplitType
    {
        PlaceInbag,
        Vender

    }
    public static void DeleteItem(String ItemName, Int32 leaveAmount = 0, Int32 TimeDelay = 1000)
    {
        Int32 Deleted = 0;
        try
        {
            var ItemQuantity = ItemsManager.GetItemCountByNameLUA(ItemName) - leaveAmount;
            if (string.IsNullOrWhiteSpace(ItemName) || ItemQuantity <= 0)
                return;

            for (Int32 Bags = 0; Bags <= 4; Bags++)
            {
                if (!String.IsNullOrWhiteSpace(GetBagName(Bags)))
                {
                    for (Int32 Slots = 1; Slots <= GetContainerNumSlots(Bags); Slots++)
                    {
                        String[] GetItemInfo = Lua.Wow.GetItemInfo<String[]>(ItemLink(Bags, Slots));
                        if (!String.IsNullOrWhiteSpace(GetItemInfo[0]))
                        {
                            Int32 ItemLeft = ItemQuantity - Deleted;
                            if (GetItemInfo[0].ToLower() == ItemName.ToLower())
                            {
                                if (ItemLeft > 0)
                                {
                                    if (StackCount(Bags, Slots) < 1)
                                    {
                                        Lua.LuaDoString("ClearCursor();");
                                        Lua.LuaDoString("PickupContainerItem(" + Bags + ", " + Slots + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Deleted = Deleted + 1;
                                        Thread.Sleep(TimeDelay);
                                    }
                                    else if (ItemLeft > StackCount(Bags, Slots))
                                    {
                                        Lua.LuaDoString("ClearCursor();");
                                        Lua.LuaDoString("SplitContainerItem(" + Bags + ", " + Slots + ", " + StackCount(Bags, Slots) + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Deleted = Deleted + StackCount(Bags, Slots);
                                        Thread.Sleep(TimeDelay);
                                    }
                                    else
                                    {
                                        Lua.LuaDoString("ClearCursor();");
                                        Lua.LuaDoString("SplitContainerItem(" + Bags + ", " + Slots + ", " + ItemLeft + ");");
                                        Lua.LuaDoString("DeleteCursorItem();");
                                        Deleted = Deleted + ItemLeft;
                                        Thread.Sleep(TimeDelay);
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

    public static void SplitItemStack(String ItemName, bool SellToVender = false, Int32 StackAmount = 5)
    {
        string[] FreeSlot = GetFreeSlots().Split(',');
        try
        {
            for (Int32 B = 0; B <= 4; B++)
            {
                if (!String.IsNullOrWhiteSpace(GetBagName(B)))
                {
                    for (Int32 S = 1; S <= GetContainerNumSlots(B); S++)
                    {
                        String[] GetItemInfo = Lua.Wow.GetItemInfo<String[]>(ItemLink(B, S));
                        if (!String.IsNullOrWhiteSpace(GetItemInfo[0]))
                        {
                            if (GetItemInfo[0].ToLower() == ItemName.ToLower())
                            {
                                Lua.LuaDoString("ClearCursor();");
                                if (StackCount(B, S) > StackAmount)
                                { 
                                    Lua.LuaDoString("SplitContainerItem(" + B + ", " + S + ", " + StackAmount + ");");
                                    Lua.LuaDoString("PickupContainerItem(" + FreeSlot[0] + ", " + FreeSlot[1] + ");");
                                    if(SellToVender)
                                    {
                                        Boolean MerchantFrame = Lua.LuaDoString<Boolean>("return MerchantFrame:IsVisible();");
                                        if(MerchantFrame)
                                        {
                                            Lua.LuaDoString("ShowMerchantSellCursor(1);");
                                            Thread.Sleep(10);
                                            Lua.LuaDoString("UseContainerItem(" + FreeSlot[0] + ", " + FreeSlot[1] + ");");
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch { }
    }

    public static void UseItemByName(String ItemName)
    {
        for (Int32 f = 1; f < 4; f++)
        {
            if (GetBagName(f) != String.Empty)
            {
                for (Int32 I = 0; I < GetContainerNumSlots(f); I++)
                {
                    String[] GetItemInfo = Lua.Wow.GetItemInfo<String[]>(ItemLink(f, I));
                    if (GetItemInfo[0] != String.Empty)
                    {
                        if (GetItemInfo[0].ToLower() == ItemName.ToLower())
                        {
                            if (GetItemCooldown(GetItemInfo[0]))
                            {
                                ItemsManager.UseItem(GetItemInfo[0]);
                            }
                        }
                    }
                }
            }
        }
    }

    private static String GetFreeSlots(Int32 ContainerSlot = 0)
    {
        String GetSlotAvalable = null;
        for (Int32 j = 1; j < 4; j++)
        {
            if (j != ContainerSlot && GetBagName(j) != String.Empty)
            {
                for (Int32 P = 1; P < GetContainerNumSlots(j); P++)
                {
                    Boolean ItemDoesntExist = Lua.LuaDoString<Boolean>("local itemLink = GetContainerItemLink(" + j + "," + P + "); if not itemLink then return true end");
                    if (ItemDoesntExist)
                    {
                        GetSlotAvalable = j.ToString() + "," + P.ToString();
                        break;
                    }
                }
            }
        }
        return GetSlotAvalable;
    }

    private static String GetBagName(Int32 R)
    {
        return Lua.LuaDoString<String>("return GetBagName(" + R + ");");
    }

    private static Int32 GetContainerNumSlots(Int32 C)
    {
        return Lua.LuaDoString<Int32>("return GetContainerNumSlots(" + C + ");");
    }
    private static String ItemLink(Int32 H, Int32 W)
    {
        return Lua.LuaDoString<String>("local itemLink = GetContainerItemLink(" + H + "," + W + "); return itemLink;");
    }

    private static Int32 StackCount(Int32 Q, Int32 X)
    {
        return Lua.LuaDoString<Int32>("local _, stackCount = GetContainerItemInfo(" + Q + ", " + X + "); return stackCount;");
    }

    private static bool GetItemCooldown(string Item)
    {
        return Lua.LuaDoString<bool>("local _, _, enable = GetItemCooldown(" + ItemsManager.GetIdByName(Item) + "); if enable == 1 then return true end");
    }
}