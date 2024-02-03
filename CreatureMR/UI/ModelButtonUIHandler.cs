using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CackleCrew.UI
{
    class ModelButtonUIHandler : MonoBehaviour
    {
        public Color selectedColor;
        public Color unselectedColor;
        public Image panel;
        public Image image;
        public TMP_Text text;
        public void Init()
        {
            ColorUtility.TryParseHtmlString("#FFE7C9", out selectedColor);
            ColorUtility.TryParseHtmlString("#FD7D3E", out unselectedColor);
            unselectedColor.a = .30f;
            panel = transform.Find("Panel").GetComponent<Image>();
            image = transform.Find("Image").GetComponent<Image>();
            text = transform.Find("Text").GetComponent<TMP_Text>();
        }
        public void ToggleUI(bool selected)
        {
            var ic = image.color;
            var it = text.color;
            if (selected)
            {
                panel.color = selectedColor;
                ic.a = 1;
                it.a = 1;
            }
            else
            {
                panel.color = unselectedColor;
                ic.a = .75f;
                it.a = .75f;
            }
            image.color = ic;
            text.color = it;
        }
    }
}
