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
            suitPrimaryColor.gameObject.SetActive(!enabled);
            suitSecondaryColor.gameObject.SetActive(!enabled);
            lensColor.gameObject.SetActive(!enabled);
            tankColor.gameObject.SetActive(!enabled);
            patternOption.gameObject.SetActive(!enabled);
            patternColor.gameObject.SetActive(!enabled);
            paintOption.gameObject.SetActive(!enabled);
            paintColor.gameObject.SetActive(!enabled);
            optionHider.SetActive(enabled);
        }
        public void SaveConfig()
        {
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ApplyProfileOptions();
            SavedProfileHelper.UpdateConfig(ourProfile);
            SavedProfileHelper.SaveConfig(profileSaves.currentOption);
        }
        public void LoadConfig()
        {
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ProfileHelper.TouchPlayerProfile(ourProfile);
            SavedProfileHelper.LoadConfig(profileSaves.currentOption);
            SavedProfileHelper.UpdatePlayerProfile(ourProfile);
            UpdateProfileOptions();
        }
        public void LoadDefaultConfig()
        {
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ProfileHelper.TouchPlayerProfile(ourProfile);
            SavedProfileHelper.UpdatePlayerProfile(ourProfile);
            UpdateProfileOptions();
        }
        public void UpdateProfileOptions()
        {
            if (modelPicker == null)
                return;
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ProfileHelper.TouchPlayerProfile(ourProfile);
            useOutfit.isOn = !SavedProfileHelper.UseOutfits;
            ToggleCustomization(useOutfit.isOn);
            if (ProfileKit.TryGetData(ourProfile, "PRIMARY", out var primary) && ColorUtility.TryParseHtmlString(primary, out var primaryColor))
                this.suitPrimaryColor.SetColor(primaryColor);
            if (ProfileKit.TryGetData(ourProfile, "HOOD", out var hood) && ColorUtility.TryParseHtmlString(hood, out var hoodColor))
                this.suitSecondaryColor.SetColor(hoodColor);
            else if (ProfileKit.TryGetData(ourProfile, "SECONDARY", out var secondary) && ColorUtility.TryParseHtmlString(secondary, out var secondaryColor))
                this.suitSecondaryColor.SetColor(secondaryColor);
            if (ProfileKit.TryGetData(ourProfile, "LENS", out var lens) && ColorUtility.TryParseHtmlString(lens, out var lensColor))
                this.lensColor.SetColor(lensColor);
            if (ProfileKit.TryGetData(ourProfile, "TANK", out var tank) && ColorUtility.TryParseHtmlString(tank, out var tankColor))
                this.tankColor.SetColor(tankColor);
            if (ProfileKit.TryGetData(ourProfile, "SECONDARY", out var pattern) && ColorUtility.TryParseHtmlString(pattern, out var patternColor))
                this.patternColor.SetColor(patternColor);
            if (ProfileKit.TryGetData(ourProfile, "PAINTCOLOR", out var paint) && ColorUtility.TryParseHtmlString(paint, out var paintColor))
                this.paintColor.SetColor(paintColor);
            if (ProfileKit.TryGetData(ourProfile, "PATTERN", out var patternOption))
                this.patternOption.SetOption(patternOption);
            if (ProfileKit.TryGetData(ourProfile, "PAINT", out var paintOption))
                this.paintOption.SetOption(paintOption);
            if (ProfileKit.TryGetData(ourProfile, "MODEL", out var modelOption))
                this.modelPicker.ChangeOption(modelOption);
        }
        public void ApplyProfileOptions()
        {
            if (modelPicker == null)
                return;
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ProfileHelper.TouchPlayerProfile(ourProfile);
            ProfileKit.SetData(ourProfile, "PRIMARY", $"#{ColorUtility.ToHtmlStringRGB(suitPrimaryColor.shape.color)}");
            ProfileKit.SetData(ourProfile, "HOOD", $"#{ColorUtility.ToHtmlStringRGB(suitSecondaryColor.shape.color)}");
            ProfileKit.SetData(ourProfile, "LENS", $"#{ColorUtility.ToHtmlStringRGB(lensColor.shape.color)}");
            ProfileKit.SetData(ourProfile, "TANK", $"#{ColorUtility.ToHtmlStringRGB(tankColor.shape.color)}");
            ProfileKit.SetData(ourProfile, "SECONDARY", $"#{ColorUtility.ToHtmlStringRGB(patternColor.shape.color)}");
            ProfileKit.SetData(ourProfile, "PAINTCOLOR", $"#{ColorUtility.ToHtmlStringRGB(paintColor.shape.color)}");
            //Switch
            ProfileKit.SetData(ourProfile, "PATTERN", patternOption.data);
            ProfileKit.SetData(ourProfile, "PAINT", paintOption.data);
            ProfileKit.SetData(ourProfile, "MODEL", modelPicker.selected);
            ProfileKit.ClearData(ourProfile, "OUTFIT");
            SavedProfileHelper.UpdateConfig(ourProfile);
            if (!useOutfit.isOn || (!initialized && !SavedProfileHelper.UseOutfits))
            {
                SavedProfileHelper.UseOutfits = true;
            }
            else
            {
                SavedProfileHelper.UseOutfits = false;
            }
        }
    }

}
