using HarmonyLib;
using UnityEngine;

namespace TranslationMod.Patches
{
    [HarmonyPatch(typeof(uGUI_PDA))]
    internal class uGUI_PDAPatches_TranslationTab
    {
        [HarmonyPatch(nameof(uGUI_PDA.Initialize)), HarmonyPrefix]
        private static void Initialize_Prefix(uGUI_PDA __instance)
        {
            if (uGUI_PDA.regularTabs.Contains(TranslationMod.TranslateTab)) return;
            uGUI_PDA.regularTabs.Add(TranslationMod.TranslateTab);
        }

        [HarmonyPatch(nameof(uGUI_PDA.Initialize)), HarmonyPostfix]
        private static void Initialize_Postfix(uGUI_PDA __instance)
        {
            GameObject logTab = __instance.tabLog.gameObject;
            GameObject myTab = GameObject.Instantiate(logTab, __instance.transform.Find("Content"));
            myTab.name = "Translation";
            GameObject.DestroyImmediate(myTab.GetComponent<uGUI_LogTab>());
            myTab.AddComponent<Monobehaviors.uGUI_TranslationTab>();
            __instance.tabs.Add(TranslationMod.TranslateTab, myTab.GetComponent<uGUI_PDATab>());
        }
    }
}