using BepInEx;
using BepInEx.Logging;
using Nautilus.Utility;
using Nautilus.Handlers;
using System.Reflection;
using HarmonyLib;
using TranslationMod.Handlers;
using UnityEngine;

namespace TranslationMod;

[BepInPlugin("com.Alexius25.TranslationMod", "TranslationMod", "1.0.0")]
internal class TranslationMod : BaseUnityPlugin
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    internal static AssetBundle AssetBundle { get; set; } = AssetBundleLoadingUtils.LoadFromAssetsFolder(TranslationMod.Assembly, "assets");
    internal static string JsonFileName => "precursor_language.json";
    internal static ManualLogSource PluginLogger { get; private set; }
    internal static PDATab TranslateTab;
    private void Start()
    {
        PluginLogger = base.Logger;
        
        TranslateTab = EnumHandler.AddEntry<PDATab>("Translation");
        
        LanguageHandler.SetLanguageLine("TabTranslation", "Translation");
        LanguageHandler.SetLanguageLine("TranslationTabLabel", "Translation");
        
        //MainMenuHandler.Register(this);
        LanguageManager.Load();

        // Example usage
        string translation = LanguageManager.GetTranslation("Hello");
        PluginLogger.LogInfo($"Translation: {translation}");
        LanguageHandler.SetLanguageLine("EncyPath_Welcome", translation);
        
        Harmony.CreateAndPatchAll(typeof(Patches.uGUI_PDAPatches_TranslationTab), PluginInfo.PLUGIN_GUID);
    }
}
