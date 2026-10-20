using BepInEx;
using HarmonyLib;

namespace SmartCraftStorageForSeneaLUI;

[BepInPlugin(Guid, Name, PluginVersion.Value)]
[BepInProcess("valheim.exe")]
[BepInDependency(SmartCraftStorageGuid)]
[BepInDependency(SeneaLUiGuid)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "alkeari.valheim.smartcraftstorageforsenealui";
    public const string Name = "SmartCraftStorage for SeneaL UI";
    private const string SmartCraftStorageGuid = "com.zellds.smartcraftstorage";
    private const string SeneaLUiGuid = "seneaL.valheim.ui";

    private Harmony _harmony;

    private void Awake()
    {
        _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, Guid);
        Logger.LogInfo($"{Name} {PluginVersion.Value} loaded");
    }

    private void OnDestroy() => _harmony.UnpatchSelf();
}
