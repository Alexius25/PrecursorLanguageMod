using BepInEx;
using mset;
using Nautilus.Handlers.TitleScreen;
using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using System;
using UnityEngine;

namespace TranslationMod.Handlers
{
    internal class MainMenuHandler
    {
        internal static void Register(BaseUnityPlugin plugin)
        {
            GameObject SpawnGameObject()
            {
                var titleObject = SpawnTitleObjects("Logo", new Vector3(-31.5f, 0.5f, 45f), Quaternion.Euler(352.5f, 180, 0), true, 700f);

                return titleObject;
            }

            var objectTitleAddon = new WorldObjectTitleAddon(SpawnGameObject, 5f);
            var titleData = new TitleScreenHandler.CustomTitleData("ExampleMenuName", objectTitleAddon);

            TitleScreenHandler.RegisterTitleScreenObject(plugin, titleData);
        }

        private static void SkyApplier(GameObject gameObject)
        {
            SkyApplier skyApplier = gameObject.AddComponent<SkyApplier>();
            skyApplier.renderers = gameObject.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Custom;
            skyApplier.SetCustomSky(UnityEngine.Object.FindObjectOfType<Sky>());
            MaterialUtils.ApplySNShaders(gameObject, 4f, 1f, 10f, Array.Empty<MaterialModifier>());
        }

        private static GameObject SpawnTitleObjects(string prefabName, Vector3 position, Quaternion rotation, bool setActive, float scale)
        {
            var gameObject = UnityEngine.Object.Instantiate(TranslationMod.AssetBundle.LoadAsset<GameObject>(prefabName), position, rotation);
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            SkyApplier(gameObject);
            gameObject.SetActive(setActive);
            return gameObject;
        }
    }
}
