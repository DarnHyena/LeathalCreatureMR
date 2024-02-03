using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CackleCrew.UI
{
    class MenuToggleUIHandler : MonoBehaviour
    {
        public Button button;
        public Image shape;
        public Transform target;
        public string data;
        public void Init()
        {
            button = transform.Find("Button").GetComponent<Button>();
            shape = transform.Find("Shape").GetComponent<Image>();
            button.onClick.AddListener(ToggleUI);
        }
        public void ToggleUI()
        {
            if (target != null)
            {
                SetUIActive(!target.gameObject.activeSelf);
                if (target.gameObject.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(target.gameObject);
                }
            }
        }
        public void SetSprite(Sprite sprite)
        {
            shape.sprite = sprite;
        }
        public void SetColor(Color color)
        {
            shape.color = color;
        }
        public void SetOption(string data)
        {
            this.data = data;
            if (target.TryGetComponent(typeof(OptionsPickerUIHandler), out var component))
            {
                var optionpicker = (OptionsPickerUIHandler)component;
                SetSprite(optionpicker.GetOptionSprite(data));
            }
        }
        public void SetUIActive(bool visible)
        {
            if (target != null)
            {
                target.gameObject.SetActive(visible);
                target.position = button.transform.position;
                Component component;
                if (target.TryGetComponent(typeof(ColorPickerUIHandler), out component))
                {
                    var colorpicker = (ColorPickerUIHandler)component;
                    colorpicker.watcher = this;
                    colorpicker.SetColor(shape.color);
                }
                if (target.TryGetComponent(typeof(OptionsPickerUIHandler), out component))
                {
                    var optionpicker = (OptionsPickerUIHandler)component;
                    optionpicker.watcher = this;
                    optionpicker.SetOption(data);
                }
                if (visible)
                {
                    EventSystem.current.SetSelectedGameObject(target.gameObject);
                }
            }
        }
        private void Update()
        {
            if (target != null && target.gameObject.activeSelf)
            {
                if (!IsUIFocused(target.gameObject) && !IsUIFocused(button.gameObject))
                {
                    SetUIActive(false);
                }
            }
        }
        private bool IsUIFocused(GameObject go)
        {
            return EventSystem.current.currentSelectedGameObject != null &&
                   (EventSystem.current.currentSelectedGameObject == go || EventSystem.current.currentSelectedGameObject.transform.IsChildOf(go.transform));
        }
    }
}
