using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CackleCrew.UI
{
    class ProfileSavesUIHandler : MonoBehaviour
    {
        public delegate void profileSelected(string profileName);
        public event profileSelected OnProfileSelected;
        public delegate void buttonClicked();
        public event buttonClicked OnSaveClicked;
        public event buttonClicked OnLoadClicked;
        struct UIOption
        {
            public Button button;
            public Image image;
        }
        Dictionary<string, UIOption> options = new Dictionary<string, UIOption>();
        public string currentOption = string.Empty;
        Color selectedColor;
        Color deselectedColor;
        Button saveButton;
        Button loadButton;

        public void Init()
        {
            saveButton = transform.Find("Save").GetComponent<Button>();
            loadButton = transform.Find("Load").GetComponent<Button>();
            //
            ColorUtility.TryParseHtmlString("#FFE7C9", out selectedColor);
            ColorUtility.TryParseHtmlString("#A04C23", out deselectedColor);
            //
            saveButton.onClick.AddListener(() => OnSaveClicked?.Invoke());
            loadButton.onClick.AddListener(() => OnLoadClicked?.Invoke());
        }
        public void RegisterProfileOption(string profileName, string child)
        {
            Debug.Log($"Looking for {child}");
            var childTransform = transform.Find(child);
            if (!childTransform) {
                Debug.LogWarning($"Could Find! {child}");
                return;
            }
            Debug.Log($"Found {child}!");
            UIOption newOption = new UIOption
            {
                button = childTransform.GetComponent<Button>(),
                image = childTransform.GetComponent<Image>()
            };
            newOption.button.onClick.AddListener(() => Option_OnClick(profileName));
            options.Add(profileName,newOption);
        }
        public void Option_OnClick(string profileName)
        {
            OnProfileSelected?.Invoke(profileName);
            currentOption = profileName;
            UpdateVisuals();
        }
        public void UpdateVisuals()
        {
            foreach (var option in options)
            {
                if (option.Key != currentOption)
                {
                    option.Value.image.color = deselectedColor;
                }
                else
                {
                    option.Value.image.color = selectedColor;
                }
            }
        }
    }
}
