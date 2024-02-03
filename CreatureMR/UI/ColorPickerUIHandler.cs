using UnityEngine;
using UnityEngine.UI;

namespace CackleCrew.UI
{
    class ColorPickerUIHandler : MonoBehaviour
    {
        public Color currentPickedColor;
        public Slider hueSlider;
        public Slider saturationSlider;
        public Slider valueSlider;
        public Image colorPreview;
        public Image saturationPreview;
        public Image saturationOverlayPreview;
        public Image valuePreview;
        public MenuToggleUIHandler watcher;
        public void Init()
        {
            //Attach Variables
            hueSlider = transform.Find("PickerHue").GetComponentInChildren<Slider>();
            saturationSlider = transform.Find("PickerSaturation").GetComponentInChildren<Slider>();
            valueSlider = transform.Find("PickerValue").GetComponentInChildren<Slider>();
            colorPreview = transform.Find("Color").GetComponent<Image>();
            saturationPreview = transform.Find("PickerSaturation").Find("Hue").GetComponent<Image>();
            saturationOverlayPreview = transform.Find("PickerSaturation").Find("Saturation").GetComponent<Image>();
            valuePreview = transform.Find("PickerValue").Find("Hue").GetComponent<Image>();
            //Listeners
            hueSlider.onValueChanged.AddListener(UpdateColor);
            saturationSlider.onValueChanged.AddListener(UpdateColor);
            valueSlider.onValueChanged.AddListener(UpdateColor);
            //Stuff
            SetColor(Color.red);
        }
        public void SetColor(Color color)
        {
            Color.RGBToHSV(color, out var H, out var S, out var V);
            hueSlider.value = H;
            saturationSlider.value = S;
            valueSlider.value = V;
            UpdateColor();
        }
        public void UpdateColor(float _ = default(float))
        {
            currentPickedColor = Color.HSVToRGB(hueSlider.value, saturationSlider.value, valueSlider.value);
            var saturated = Color.HSVToRGB(hueSlider.value, 1, valueSlider.value);
            var saturatedOverlay = Color.HSVToRGB(hueSlider.value, 0, valueSlider.value);
            var valued = Color.HSVToRGB(hueSlider.value, saturationSlider.value, 1);
            colorPreview.color = currentPickedColor;
            saturationPreview.color = saturated;
            saturationOverlayPreview.color = saturatedOverlay;
            valuePreview.color = valued;
            UpdatePreview();
        }
        public void UpdatePreview()
        {
            if (watcher != null)
            {
                watcher.shape.color = currentPickedColor;
            }
        }
    }
}
