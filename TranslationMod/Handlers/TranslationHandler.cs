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
        public class PrecursorWord
        {
            [JsonProperty("id")]
            internal int Id;
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

        private static string _jsonFilePath = string.Empty;
        private static PrecursorLanguage _languageData = new PrecursorLanguage();

        public static void Load(string jsonFilePath)
        {
            _jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "plugins", jsonFilePath);

            if (!File.Exists(_jsonFilePath))
            {
                TranslationMod.PluginLogger.LogWarning($"[Precursor-LanguageManager] JSON file not found at: {_jsonFilePath}");
                _languageData = new PrecursorLanguage();
                return;
            }

            string json = File.ReadAllText(_jsonFilePath);
            _languageData = JsonConvert.DeserializeObject<PrecursorLanguage>(json);
            TranslationMod.PluginLogger.LogInfo($"[Precursor-LanguageManager] Loaded {_languageData.Words.Count} words from JSON.");
        }

        private static void Save()
        {
            if (string.IsNullOrEmpty(_jsonFilePath))
                throw new InvalidOperationException("JsonFilePath is not initialized. Call Load first.");

            string json = JsonConvert.SerializeObject(_languageData, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonFilePath)!);
            File.WriteAllText(_jsonFilePath, json);
            TranslationMod.PluginLogger.LogInfo("[Precursor-LanguageManager] Saved language data.");
        }

        // Using the precursor word or translation
        public static string GetTranslation(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            return entry != null && !string.IsNullOrEmpty(entry.Translation)
                ? entry.Translation
                : precursorWord;
        }
        
        public static string GetPrecursor(string translation)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Translation == translation);
            return entry != null ? entry.Precursor : translation;
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
                int newId = _languageData.Words.Any()
                    ? _languageData.Words.Max(w => w.Id) + 1
                    : 1;
                _languageData.Words.Add(new PrecursorWord
                {
                    Id = newId,
                    Precursor = precursorWord,
                    Translation = translation
                });
            }
            Save();
        }
        
        public static void RemoveTranslation(string precursorWord)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Precursor == precursorWord);
            if (entry != null)
            {
                entry.Translation = string.Empty;
                Save();
            }
        }
        
        // Using the ID of the word
        public static string GetTranslationByID(int id)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Id == id);
            if (entry == null)
                return string.Empty;

            return !string.IsNullOrEmpty(entry.Translation)
                ? entry.Translation
                : entry.Precursor;
        }
        
        public static string GetPrecursorByID(int id)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Id == id);
            return entry != null ? entry.Precursor : string.Empty;
        }
        
        public static bool IsTranslatedByID(int id)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Id == id);
            return entry != null && !string.IsNullOrEmpty(entry.Translation);
        }
        
        public static void SetTranslationByID(int id, string translation)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Id == id);
            if (entry != null)
            {
                entry.Translation = translation;
                Save();
            }
        }
        
        public static void RemoveTranslationByID(int id)
        {
            var entry = _languageData.Words.FirstOrDefault(w => w.Id == id);
            if (entry != null)
            {
                entry.Translation = string.Empty;
                Save();
            }
        }

        public static List<PrecursorWord> GetAllWords() => _languageData.Words;
    }
}
