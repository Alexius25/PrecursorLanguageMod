using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
        
        [HarmonyPatch(typeof(uGUI_PDA), nameof(uGUI_PDA.SetTabs))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> SetTabs_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch[] matches = new CodeMatch[]
            {
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "Get"),
                new(i=> i.opcode == OpCodes.Stelem_Ref)
            };

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, matches)
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
                .Insert(Transpilers.EmitDelegate(TryGetTranslationTabSprite));

            return newInstructions.InstructionEnumeration();
        }
        
        private static Atlas.Sprite TryGetTranslationTabSprite(Atlas.Sprite originalSprite, PDATab currentTab)
        {
            if (currentTab != TranslationMod.TranslateTab) return originalSprite;

            return TranslationMod.TranslateTabSprite;
        }
    }
}