using BepInEx;
using BepInEx.Logging;
using Nautilus.Utility;
using Nautilus.Handlers;
using System.Reflection;
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
    private void Start()
    {
        PluginLogger = base.Logger;

        //MainMenuHandler.Register(this);
        LanguageManager.Load();

        // Example usage
        string translation = LanguageManager.GetTranslation("Test");
        LanguageHandler.SetLanguageLine("EncyPath_Welcome", translation);
    }
}
