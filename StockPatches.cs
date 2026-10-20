using HarmonyLib;
using SeneaLUI.Core;
using SmartCraftStorage.Config;
using SmartCraftStorage.CraftingChestAccess;
using SmartCraftStorage.Shared;

namespace SmartCraftStorageForSeneaLUI;

[HarmonyPatch(typeof(Stock))]
internal static class StockPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Stock.Count))]
    private static bool Count(Player p, string name, int quality, ref int __result)
    {
        if (!IsLocal(p))
            return true;
        __result = Stock.Own(p.GetInventory(), name, quality) + ChestTotal(p, name, quality);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Stock.FromChests))]
    private static bool FromChests(Player p, string name, ref int __result)
    {
        if (!IsLocal(p))
            return true;
        __result = ChestTotal(p, name, -1);
        return false;
    }

    // Inventory.HaveItem and Inventory.RemoveItem already reach SmartCraftStorage's chests through its own patches.
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Stock.Have))]
    private static bool Have(Player p, string name, ref bool __result)
    {
        if (!IsLocal(p))
            return true;
        __result = p.GetInventory().HaveItem(name, true);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Stock.Remove))]
    private static bool Remove(Player p, string name, int amount, int quality)
    {
        if (!IsLocal(p))
            return true;
        p.GetInventory().RemoveItem(name, amount, quality, true);
        return false;
    }

    private static bool IsLocal(Player p) => p != null && p == Player.m_localPlayer;

    private static int ChestTotal(Player p, string name, int quality)
    {
        if (!InventoryChestPatches.IsCraftingOrBuildingContext(p.GetInventory(), out var player))
            return 0;
        if (ChestCountCache.TryGet(name, quality, true, out var cached))
            return cached;
        var total = 0;
        foreach (var container in NearbyContainers.Find(player.transform.position, ModConfig.CraftingChestRadius.Value, player))
            total += UnlockedInventory.CountItems(container.GetInventory(), name, quality, true);
        ChestCountCache.Store(name, quality, true, total);
        return total;
    }
}
