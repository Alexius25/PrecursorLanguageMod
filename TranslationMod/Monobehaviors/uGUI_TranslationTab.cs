using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            GameObject label = transform.Find("Content/LogLabel").gameObject;
            label.name = "TranslationTabLabel";
            label.GetComponent<TextMeshProUGUI>().text = Language.main.Get("TranslationTabLabel");
            GetComponentInChildren<RectMask2D>().enabled = true;
            // Add more UI setup here if desired
        }

        public override void Open()  { content.SetVisible(true); }
        public override void Close() { scrollRect.velocity = Vector2.zero; content.SetVisible(false); }
    }
}