using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CackleCrew.UI
{
    class OptionsPickerUIHandler : MonoBehaviour
    {
        public Dictionary<Image, Image> pairedOptions = new Dictionary<Image, Image>();
        public MenuToggleUIHandler watcher;
        public string currentOption;
        public void Init()
        {
            ScanForOptions();
        }
        public void ToggleUI()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        void ScanForOptions()
        {
            Transform optionsContainer = transform.Find("Options");
            for (int i = 0; i < optionsContainer.childCount; i++)
            {
                var childImage = optionsContainer.GetChild(i).GetComponent<Image>();
                var shapeImage = childImage.transform.Find("Shape").GetComponent<Image>();
                var button = childImage.GetComponent<Button>();
                pairedOptions.Add(childImage, shapeImage);
                button.onClick.AddListener(() => OptionSelected(childImage));
            }
            OptionSelected(pairedOptions.Keys.First(), false);
        }
        void OptionSelected(Image origin, bool toggle = true)
        {
            SetOption(origin.gameObject.name);
            if (toggle)
                ToggleUI();
        }

        public void SetOption(string name)
        {
            foreach (var option in pairedOptions)
            {
                if (option.Key.gameObject.name != name)
                {
                    var color = option.Key.color;
                    color.a = 0;
                    option.Key.color = color;
                }
                else
                {
                    var color = option.Key.color;
                    color.a = 1;
                    option.Key.color = color;
                    if (watcher != null)
                    {
                        watcher.SetOption(name);
                    }
                    currentOption = name;
                }
            }
        }
        public Sprite GetOptionSprite(string name)
        {
            foreach (var option in pairedOptions)
            {
                if (option.Key.gameObject.name == name)
                {
                    return option.Value.sprite;
                }
            }
            return null;
        }
    }
}
