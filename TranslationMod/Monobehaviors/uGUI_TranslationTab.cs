using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TranslationMod.Handlers;

namespace TranslationMod.Monobehaviors
{
    public class uGUI_TranslationTab : uGUI_PDATab
    {
        public static uGUI_TranslationTab Instance { get; private set; }
        private CanvasGroup content;
        private ScrollRect scrollRect;
        private Transform scrollCanvas;

        new private void Awake()
        {
            content = GetComponentInChildren<CanvasGroup>();
            scrollRect = transform.Find("Content/ScrollView").GetComponent<ScrollRect>();
            scrollCanvas = scrollRect.transform.Find("Viewport/ScrollCanvas");
            scrollRect.enabled = true;
            Instance = this;
        }

        private void Start()
        {
            // VerticalLayoutGroup is required for the scroll canvas to layout its children properly
            if (scrollCanvas.GetComponent<VerticalLayoutGroup>() == null)
            {
                var vLayout = scrollCanvas.gameObject.AddComponent<VerticalLayoutGroup>();
                vLayout.spacing = 5;
                vLayout.childForceExpandHeight = false;
                vLayout.childForceExpandWidth = true;
                vLayout.childControlHeight = true;
                vLayout.childControlWidth = true;
            }

            GameObject label = transform.Find("Content/LogLabel").gameObject;
            label.name = "TranslationTabLabel";
            label.GetComponent<TextMeshProUGUI>().text = Language.main.Get("TranslationTabLabel");
            GetComponentInChildren<RectMask2D>().enabled = true;
            
            RegisterSortButtons();
            RegisterHeaders();
            RegisterWords();
        }

        public override void Open()
        {
            content.SetVisible(true);
        }
        
        public override void Close()
        {
            scrollRect.velocity = Vector2.zero;
            content.SetVisible(false);
        }

        private void RegisterWords()
        {
            foreach (var word in LanguageManager.GetAllWords())
            { 
                // Column for each word
                GameObject row = new GameObject("TranslationRow", typeof(RectTransform), typeof(Image));
                row.transform.SetParent(scrollCanvas, false);
                var image = row.GetComponent<Image>();
                image.sprite = TranslationMod.TranslateTabBackGroundSprite;
                image.type = Image.Type.Sliced;
                var rowLayout = row.AddComponent<HorizontalLayoutGroup>(); 
                rowLayout.spacing = 10; 
                rowLayout.childForceExpandWidth = false; 
                rowLayout.childForceExpandHeight = false; 
                rowLayout.childAlignment = TextAnchor.MiddleCenter; 
                row.AddComponent<LayoutElement>().preferredHeight = 40;

                // Precursor-Label
                GameObject precursorLabel = new GameObject("PrecursorLabel", typeof(RectTransform));
                precursorLabel.transform.SetParent(row.transform, false);
                var precursorTMP = precursorLabel.AddComponent<TextMeshProUGUI>();
                precursorTMP.text = word.Precursor;
                precursorTMP.fontSize = 20;
                precursorTMP.color = Color.white;
                precursorTMP.alignment = TextAlignmentOptions.Center;
                precursorLabel.AddComponent<LayoutElement>().preferredWidth = 200;

                // Translation-Label
                GameObject translationLabel = new GameObject("TranslationLabel", typeof(RectTransform));
                translationLabel.transform.SetParent(row.transform, false);
                var translationTMP = translationLabel.AddComponent<TextMeshProUGUI>();
                translationTMP.text = word.Translation;
                translationTMP.fontSize = 20;
                translationTMP.color = Color.white;
                translationTMP.alignment = TextAlignmentOptions.Center;
                translationLabel.AddComponent<LayoutElement>().preferredWidth = 200;

                // Input Field
                GameObject inputGO = new GameObject("TranslationInput", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                inputGO.transform.SetParent(row.transform, false);
                inputGO.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 36);
                inputGO.AddComponent<LayoutElement>().preferredWidth = 300;
                
                var bgImage = inputGO.GetComponent<Image>();
                bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                
                var inputField = inputGO.AddComponent<TMP_InputField>();
                
                GameObject textGO = new GameObject("Text", typeof(RectTransform));
                textGO.transform.SetParent(inputGO.transform, false);
                var inputText = textGO.AddComponent<TextMeshProUGUI>();
                inputText.fontSize = 20;
                inputText.color = Color.yellow;
                inputText.text = "";
                inputText.enableWordWrapping = false;
                inputText.alignment = TextAlignmentOptions.Left;
                inputText.raycastTarget = true;
                
                GameObject placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
                placeholderGO.transform.SetParent(inputGO.transform, false);
                var placeholder = placeholderGO.AddComponent<TextMeshProUGUI>();
                placeholder.fontSize = 20;
                placeholder.color = new Color(1, 1, 0.5f, 0.5f);
                placeholder.text = "New Translation...";
                placeholder.alignment = TextAlignmentOptions.Left;
                placeholder.raycastTarget = false;
                
                inputField.textComponent = inputText;
                inputField.placeholder = placeholder;
                inputField.text = "";
                inputField.interactable = true;
                
                inputField.onEndEdit.AddListener(newText =>
                {
                    LanguageManager.SetTranslation(word.Precursor, newText);
                    translationTMP.text = newText;
                });
            }
        }

        private void RegisterHeaders()
        {
            // Header Row
            GameObject headerRow = new GameObject("HeaderRow", typeof(RectTransform));
            headerRow.transform.SetParent(scrollCanvas, false);
            var headerLayout = headerRow.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 20;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerRow.AddComponent<LayoutElement>().preferredHeight = 60;

            // Precursor-Header
            GameObject precursorHeader = new GameObject("PrecursorHeader", typeof(RectTransform));
            precursorHeader.transform.SetParent(headerRow.transform, false);
            var precursorHeaderTMP = precursorHeader.AddComponent<TextMeshProUGUI>();
            precursorHeaderTMP.text = "Precursor";
            precursorHeaderTMP.fontSize = 32;
            precursorHeaderTMP.color = Color.cyan;
            precursorHeaderTMP.alignment = TextAlignmentOptions.Center;
            precursorHeader.AddComponent<LayoutElement>().preferredWidth = 200;

            // Translation-Header
            GameObject translationHeader = new GameObject("TranslationHeader", typeof(RectTransform));
            translationHeader.transform.SetParent(headerRow.transform, false);
            var translationHeaderTMP = translationHeader.AddComponent<TextMeshProUGUI>();
            translationHeaderTMP.text = "Translation";
            translationHeaderTMP.fontSize = 32;
            translationHeaderTMP.color = Color.cyan;
            translationHeaderTMP.alignment = TextAlignmentOptions.Center;
            translationHeader.AddComponent<LayoutElement>().preferredWidth = 200;

            // Input Field-Header
            GameObject inputHeader = new GameObject("InputHeader", typeof(RectTransform));
            inputHeader.transform.SetParent(headerRow.transform, false);
            var inputHeaderTMP = inputHeader.AddComponent<TextMeshProUGUI>();
            inputHeaderTMP.text = "Manual Translation";
            inputHeaderTMP.fontSize = 32;
            inputHeaderTMP.color = Color.cyan;
            inputHeaderTMP.alignment = TextAlignmentOptions.Center;
            inputHeader.AddComponent<LayoutElement>().preferredWidth = 300;
        }
        
        private void RegisterSortButtons()
        {
            GameObject sortRow = new GameObject("SortButtonsRow", typeof(RectTransform));
            sortRow.transform.SetParent(scrollCanvas.parent, false);
            var hLayout = sortRow.AddComponent<HorizontalLayoutGroup>();
            hLayout.spacing = 10;
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            sortRow.AddComponent<LayoutElement>().preferredHeight = 40;

            var btnAZ = CreateButton("SortAZButton", "A-Z", sortRow.transform);
            btnAZ.onClick.AddListener(SortByTranslationAZ);

            var btnTrans = CreateButton("SortTranslationButton", "Untranslated First", sortRow.transform);
            btnTrans.onClick.AddListener(SortByMissingTranslationFirst);
        }

        private void SortByTranslationAZ()
        {
            var rows = scrollCanvas.Cast<Transform>()
                .Where(t => t.name == "TranslationRow")
                .OrderBy(t =>
                {
                    var text = t.Find("TranslationLabel")
                        .GetComponent<TextMeshProUGUI>()
                        .text;
                    return string.IsNullOrEmpty(text) ? 1 : 0;
                })
                .ThenBy(t =>
                    t.Find("TranslationLabel")
                        .GetComponent<TextMeshProUGUI>()
                        .text)
                .ToList();
            ReorderRows(rows);
        }

        private void SortByMissingTranslationFirst()
        {
            var rows = scrollCanvas.Cast<Transform>()
                .Where(t => t.name == "TranslationRow")
                .OrderBy(t =>
                {
                    var text = t.Find("TranslationLabel")
                        .GetComponent<TextMeshProUGUI>()
                        .text;
                    return string.IsNullOrEmpty(text) ? 0 : 1;
                })
                .ThenBy(t =>
                    t.Find("TranslationLabel")
                        .GetComponent<TextMeshProUGUI>()
                        .text)
                .ToList();
            ReorderRows(rows);
        }
        
        private Button CreateButton(string name, string label, Transform parent)
        {
            GameObject btnGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnGO.transform.SetParent(parent, false);
            var img = btnGO.GetComponent<Image>();
            img.color = Color.grey;

            GameObject textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(btnGO.transform, false);
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            var rt = textGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            return btnGO.GetComponent<Button>();
        }

        private void ReorderRows(List<Transform> sortedRows)
        {
            var header = scrollCanvas.Find("HeaderRow");
            if (header != null)
                header.SetAsFirstSibling();

            for (int i = 0; i < sortedRows.Count; i++)
                sortedRows[i].SetSiblingIndex(i + 1);
        }
    }
}