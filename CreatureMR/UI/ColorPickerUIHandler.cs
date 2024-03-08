using TMPro;
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
        public TMP_InputField hueInput;
        public TMP_InputField saturationInput;
        public TMP_InputField valueInput;
        public TMP_InputField webcolorInput;
        public Button webcolorApply;
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
            hueInput = transform.Find("InputHue").GetComponent<TMP_InputField>();
            saturationInput = transform.Find("InputSaturation").GetComponent<TMP_InputField>();
            valueInput = transform.Find("InputValue").GetComponent<TMP_InputField>();
            webcolorInput = transform.Find("InputWebColor").GetComponent<TMP_InputField>();
            webcolorApply = transform.Find("Apply").GetComponent<Button>();
            //Listeners
            hueSlider.onValueChanged.AddListener(UpdateColor);
            saturationSlider.onValueChanged.AddListener(UpdateColor);
            valueSlider.onValueChanged.AddListener(UpdateColor);
            hueInput.onSubmit.AddListener(CheckInputs);
            saturationInput.onSubmit.AddListener(CheckInputs);
            valueInput.onSubmit.AddListener(CheckInputs);
            webcolorApply.onClick.AddListener(ApplyWebColor);
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
            UpdateInputs();
        }
        public void UpdateInputs()
        {
            Color.RGBToHSV(currentPickedColor, out var H, out var S, out var V);
            hueInput.text = ((int)(H * 100)).ToString();
            saturationInput.text = ((int)(S * 100)).ToString();
            valueInput.text = ((int)(V * 100)).ToString();
            webcolorInput.text = ColorUtility.ToHtmlStringRGB(currentPickedColor);
        }
        public void CheckInputs(string text)
        {
            Color.RGBToHSV(currentPickedColor, out var H, out var S, out var V);
            var H_Input = CheckValueInput(hueInput.text, H);
            var S_Input = CheckValueInput(saturationInput.text, S);
            var V_Input = CheckValueInput(valueInput.text, V);
            SetColor(Color.HSVToRGB(H_Input, S_Input, V_Input));
        }
        public float CheckValueInput(string input, float default_value)
        {
            if (!float.TryParse(input, out var value))
                value = default_value;
            else
                value = Mathf.Clamp(value / 100, 0, 1);
            return value;
        }
        public void ApplyWebColor()
        {
            if (ColorUtility.TryParseHtmlString(CheckHTMLString(webcolorInput.text), out var color))
               SetColor(color);
        }
        public string CheckHTMLString(string text)
        {
            return $"#{text.Replace("#", string.Empty)}";
        }
    }
}
