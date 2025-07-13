using BepInEx;
using BepInEx.Logging;
using Nautilus.Utility;
using Nautilus.Handlers;
using Nautilus.Utility.ModMessages;
using System.Reflection;
using HarmonyLib;
using TranslationMod.Handlers;
using UnityEngine;

namespace TranslationMod;

[BepInPlugin("com.Alexius25.TranslationMod", "TranslationMod", "1.0.0")]
[BepInDependency("com.snmodding.nautilus")]
internal class TranslationMod : BaseUnityPlugin
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    
    internal static AssetBundle AssetBundle { get; set; } = AssetBundleLoadingUtils.LoadFromAssetsFolder(TranslationMod.Assembly, "translationmod");
    internal static AssetBundle SoundBundle { get; set; } = AssetBundleLoadingUtils.LoadFromAssetsFolder(TranslationMod.Assembly, "translationmodsounds");
    
    internal static string JsonFileName => "precursor_language.json";
    
    internal static ManualLogSource PluginLogger { get; private set; }
    
    internal static PDATab TranslateTab;
    
    internal static Atlas.Sprite TranslateTabSprite;
    internal static Sprite WordEntryBackground;
    internal static Sprite ButtonBackground;
    
    private void Start()
    {
        // Instance project logger
        PluginLogger = base.Logger;
        
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/Alexius25/PrecursorLanguageMod/refs/heads/main/FMUInfo.json");
        
        TranslateTab = EnumHandler.AddEntry<PDATab>("Translation");
        CachePrefabs();
        
        LanguageHandler.SetLanguageLine("TabTranslation", "Translation");
        LanguageHandler.SetLanguageLine("TranslationTabLabel", "Translation");
        
        //MainMenuHandler.Register(this);
        LanguageManager.Load();
        
        Harmony.CreateAndPatchAll(typeof(Patches.uGUI_PDAPatches_TranslationTab), PluginInfo.PLUGIN_GUID);
        PluginLogger.LogInfo("TranslationMod has been loaded successfully.");
    }

    private void CachePrefabs()
    {
        TranslateTabSprite = new Atlas.Sprite(AssetBundle.LoadAsset<Sprite>("TranslationTabSprite"));
        TranslateTabSprite.size = new Vector2(700, 700);
        
        WordEntryBackground = AssetBundle.LoadAsset<Sprite>("FrameSprite2");
        ButtonBackground = AssetBundle.LoadAsset<Sprite>("FrameSprite3");
    }
}
