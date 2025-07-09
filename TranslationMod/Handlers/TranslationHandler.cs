using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TranslationMod.Handlers
{
    internal static class Paths
    {
        public static string PluginPath => AppDomain.CurrentDomain.BaseDirectory;
    }

    internal static class LanguageManager
    {
        private static string JsonFilePath => Path.Combine(Paths.PluginPath,"BepInEx", "plugins", "TranslationMod", "Data", TranslationMod.JsonFileName);

        internal class PrecursorWord
        {
            [JsonProperty("precursor")]
            internal string Precursor;
            [JsonProperty("translation")]
            internal string Translation;
        }

        internal class PrecursorLanguage
        {
            [JsonProperty("words")]
            internal List<PrecursorWord> Words = new();
        }

        private static PrecursorLanguage _languageData = new();

        internal static void Load()
        {
            if (!File.Exists(JsonFilePath))
            {
                TranslationMod.PluginLogger.LogWarning($"[LanguageManager] JSON file not found at: {JsonFilePath}");
                _languageData = new PrecursorLanguage();
                return;
            }

            string json = File.ReadAllText(JsonFilePath);
            _languageData = JsonConvert.DeserializeObject<PrecursorLanguage>(json);
            TranslationMod.PluginLogger.LogInfo($"[LanguageManager] Loaded {_languageData.Words.Count} words from JSON.");
        }

        private static void Save()
        {
            string json = JsonConvert.SerializeObject(_languageData, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(JsonFilePath));
            File.WriteAllText(JsonFilePath, json);
            TranslationMod.PluginLogger.LogInfo("[LanguageManager] Saved language data.");
        }

        internal static string GetTranslation(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            return entry != null && !string.IsNullOrEmpty(entry.Translation)
                ? entry.Translation
                : precursorWord;
        }

        internal static bool IsTranslated(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            return entry != null && !string.IsNullOrEmpty(entry.Translation);
        }

        internal static void SetTranslation(string precursorWord, string translation)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            if (entry != null)
            {
                entry.Translation = translation;
            }
            else
            {
                _languageData.Words.Add(new PrecursorWord { Precursor = precursorWord, Translation = translation });
            }

            Save();
        }

        internal static List<PrecursorWord> GetAllWords() => _languageData.Words;
    }
}
