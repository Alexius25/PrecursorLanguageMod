using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TranslationMod.Handlers
{
    internal static class Paths
    {
        internal static string PluginPath => AppDomain.CurrentDomain.BaseDirectory;
    }

    public static class LanguageManager
    {
        private static string JsonFilePath => Path.Combine(Paths.PluginPath,"BepInEx", "plugins", "TranslationMod", "Data", TranslationMod.JsonFileName);

        public class PrecursorWord
        {
            [JsonProperty("precursor")]
            internal string Precursor;
            [JsonProperty("translation")]
            internal string Translation;
        }

        public class PrecursorLanguage
        {
            [JsonProperty("words")]
            internal List<PrecursorWord> Words = new();
        }

        private static PrecursorLanguage _languageData = new();

        public static void Load()
        {
            if (!File.Exists(JsonFilePath))
            {
                TranslationMod.PluginLogger.LogWarning($"[Precursor-LanguageManager] JSON file not found at: {JsonFilePath}");
                _languageData = new PrecursorLanguage();
                return;
            }

            string json = File.ReadAllText(JsonFilePath);
            _languageData = JsonConvert.DeserializeObject<PrecursorLanguage>(json);
            TranslationMod.PluginLogger.LogInfo($"[Precursor-LanguageManager] Loaded {_languageData.Words.Count} words from JSON.");
        }

        private static void Save()
        {
            string json = JsonConvert.SerializeObject(_languageData, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(JsonFilePath));
            File.WriteAllText(JsonFilePath, json);
            TranslationMod.PluginLogger.LogInfo("[Precursor-LanguageManager] Saved language data.");
        }

        public static string GetTranslation(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            return entry != null && !string.IsNullOrEmpty(entry.Translation)
                ? entry.Translation
                : precursorWord;
        }

        public static bool IsTranslated(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            return entry != null && !string.IsNullOrEmpty(entry.Translation);
        }

        public static void SetTranslation(string precursorWord, string translation)
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
