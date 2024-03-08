using UnityEngine;
using CackleCrew.ThisIsMagical;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;
using CackleCrewMR.Helpers;

namespace CackleCrew.UI
{
    class CackleCrewUIHandler : MonoBehaviour
    {
        public ModelPickerUIHandler modelPicker;
        public ColorPickerUIHandler colorPicker;
        public OptionsPickerUIHandler optionPicker;
        public ClipboardUIHandler clipboardHandler;
        public MenuToggleUIHandler suitPrimaryColor;
        public MenuToggleUIHandler suitSecondaryColor;
        public MenuToggleUIHandler lensColor;
        public MenuToggleUIHandler tankColor;
        public MenuToggleUIHandler patternOption;
        public MenuToggleUIHandler patternColor;
        public MenuToggleUIHandler paintOption;
        public MenuToggleUIHandler paintColor;
        public ProfileSavesUIHandler profileSaves;
        public Toggle useOutfit;
        public Button exitButton;
        public GameObject optionHider;
        public bool initialized = false;
        void Start()
        {
            //PAIR
            modelPicker = transform.Find("CrewSelection").gameObject.AddComponent<ModelPickerUIHandler>();
            colorPicker = transform.Find("Pickers/ColorPicker").gameObject.AddComponent<ColorPickerUIHandler>();
            optionPicker = transform.Find("Pickers/OptionPicker").gameObject.AddComponent<OptionsPickerUIHandler>();
            clipboardHandler = transform.Find("Profiles/ClipboardComponent").gameObject.AddComponent<ClipboardUIHandler>();
            suitPrimaryColor = transform.Find("ColorStuff/SuitOptions/PrimaryColor").gameObject.AddComponent<MenuToggleUIHandler>();
            suitSecondaryColor = transform.Find("ColorStuff/SuitOptions/SecondaryColor").gameObject.AddComponent<MenuToggleUIHandler>();
            lensColor = transform.Find("ColorStuff/GearOptions/LensColor").gameObject.AddComponent<MenuToggleUIHandler>();
            tankColor = transform.Find("ColorStuff/GearOptions/TankColor").gameObject.AddComponent<MenuToggleUIHandler>();
            patternOption = transform.Find("ColorStuff/PatternOptions/PatternOption").gameObject.AddComponent<MenuToggleUIHandler>();
            patternColor = transform.Find("ColorStuff/PatternOptions/PatternColor").gameObject.AddComponent<MenuToggleUIHandler>();
            paintOption = transform.Find("ColorStuff/MarkingOptions/MarkingOption").gameObject.AddComponent<MenuToggleUIHandler>();
            paintColor = transform.Find("ColorStuff/MarkingOptions/MarkingColor").gameObject.AddComponent<MenuToggleUIHandler>();
            profileSaves = transform.Find("Profiles/ProfileList").gameObject.AddComponent<ProfileSavesUIHandler>();
            useOutfit = transform.Find("UseOutfits").GetComponent<Toggle>();
            exitButton = transform.Find("Back").gameObject.GetComponent<Button>();
            optionHider = transform.Find("SuitBGone").gameObject;
            //Targets
            suitPrimaryColor.target = colorPicker.transform;
            suitSecondaryColor.target = colorPicker.transform;
            lensColor.target = colorPicker.transform;
            tankColor.target = colorPicker.transform;
            patternOption.target = optionPicker.transform;
            patternColor.target = colorPicker.transform;
            paintOption.target = optionPicker.transform;
            paintColor.target = colorPicker.transform;
            //
            clipboardHandler.handler = this;
            //INIT
            suitPrimaryColor.Init();
            suitSecondaryColor.Init();
            lensColor.Init();
            tankColor.Init();
            patternOption.Init();
            patternColor.Init();
            paintOption.Init();
            paintColor.Init();
            //
            modelPicker.Init();
            colorPicker.Init();
            optionPicker.Init();
            //
            clipboardHandler.Init();
            //
            profileSaves.Init();
            profileSaves.RegisterProfileOption("A", "ProA");
            profileSaves.RegisterProfileOption("B", "ProB");
            profileSaves.RegisterProfileOption("C", "ProC");
            profileSaves.RegisterProfileOption("D", "ProD");
            profileSaves.OnSaveClicked += SaveConfig;
            profileSaves.OnLoadClicked += LoadConfig;
            //Hide
            colorPicker.gameObject.SetActive(false);
            optionPicker.gameObject.SetActive(false);
            //Exit
            exitButton.onClick.AddListener(UIManager.CrewUIExitButtonClicked);
            useOutfit.onValueChanged.AddListener(ToggleCustomization);
            //Fetch_InitialProfile
            LoadDefaultConfig();
            initialized = true;
        }
        public void ToggleCustomization(bool enabled)
        {
            suitPrimaryColor.gameObject.SetActive(enabled);
            suitSecondaryColor.gameObject.SetActive(enabled);
            lensColor.gameObject.SetActive(enabled);
            tankColor.gameObject.SetActive(enabled);
            patternOption.gameObject.SetActive(enabled);
            patternColor.gameObject.SetActive(enabled);
            paintOption.gameObject.SetActive(enabled);
            paintColor.gameObject.SetActive(enabled);
            optionHider.SetActive(!enabled);
        }
        public void SaveConfig()
        {
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            ApplyProfileOptions();
            SavedProfileHelper.UpdateConfig(profile);
            SavedProfileHelper.SaveConfig(profileSaves.currentOption);
        }
        public void LoadConfig()
        {
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            SavedProfileHelper.LoadConfig(profileSaves.currentOption);
            SavedProfileHelper.UpdatePlayerProfile(profile);
            UpdateProfileOptions();
        }
        public void LoadDefaultConfig()
        {
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            SavedProfileHelper.UpdatePlayerProfile(profile);
            UpdateProfileOptions();
        }
        public void UpdateProfileOptions()
        {
            if (modelPicker == null)
                return;
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            useOutfit.isOn = SavedProfileHelper.UseOutfits;
            ToggleCustomization(useOutfit.isOn);
            if (ColorUtility.TryParseHtmlString(profile.GetData("PRIMARY"), out var primaryColor))
                this.suitPrimaryColor.SetColor(primaryColor);
            if (ColorUtility.TryParseHtmlString(profile.GetData("HOOD"), out var hoodColor))
                this.suitSecondaryColor.SetColor(hoodColor);
            if (ColorUtility.TryParseHtmlString(profile.GetData("LENS"), out var lensColor))
                this.lensColor.SetColor(lensColor);
            if (ColorUtility.TryParseHtmlString(profile.GetData("TANK"), out var tankColor))
                this.tankColor.SetColor(tankColor);
            if (ColorUtility.TryParseHtmlString(profile.GetData("SECONDARY"), out var patternColor))
                this.patternColor.SetColor(patternColor);
            if (ColorUtility.TryParseHtmlString(profile.GetData("PAINTCOLOR"), out var paintColor))
                this.paintColor.SetColor(paintColor);
            this.patternOption.SetOption(profile.GetData("PATTERN"));
            this.paintOption.SetOption(profile.GetData("PAINT"));
            this.modelPicker.ChangeOption(profile.GetData("MODEL"));
        }
        public void ApplyProfileOptions()
        {
            if (modelPicker == null)
                return;
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            profile.SetData("PRIMARY", ColorUtility.ToHtmlStringRGB(suitPrimaryColor.shape.color), true);
            profile.SetData("HOOD", ColorUtility.ToHtmlStringRGB(suitSecondaryColor.shape.color), true);
            profile.SetData("LENS", ColorUtility.ToHtmlStringRGB(lensColor.shape.color), true);
            profile.SetData("TANK", ColorUtility.ToHtmlStringRGB(tankColor.shape.color), true);
            profile.SetData("SECONDARY", ColorUtility.ToHtmlStringRGB(patternColor.shape.color), true);
            profile.SetData("PAINTCOLOR", ColorUtility.ToHtmlStringRGB(paintColor.shape.color),true);
            //Switch
            profile.SetData("PATTERN", patternOption.data);
            profile.SetData("PAINT", paintOption.data);
            profile.SetData("MODEL", modelPicker.selected);
            SavedProfileHelper.UpdateConfig(profile);
            if (useOutfit.isOn || (!initialized && !SavedProfileHelper.UseOutfits))
            {
                SavedProfileHelper.UseOutfits = true;
                profile.SetData("OUTFIT", "TRUE");
            }
            else
            {
                SavedProfileHelper.UseOutfits = false;
                profile.SetData("OUTFIT", "FALSE");
            }
        }
    }

}
