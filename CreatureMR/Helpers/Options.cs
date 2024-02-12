using CackleCrewMR.Helpers;
using CreatureModelReplacement;
using HarmonyLib;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //Here is the center over-all Customization for the Creature itself.
    //
    //Initialization
    static partial class Options
    {
        public static void Init()
        {
            SetDefaultProfile();
            RegisterAssets();
            SavedProfileHelper.Init();
        }
        public static void RegisterAssets()
        {
            RegisterVanillaModels();
            RegisterMoreSuitsMaterials();
            RegisterVanillaMaterials();
        }
        public static void RegisterVanillaMaterials()
        {
            //RegisterSuitProfile("SUIT", "#PRIMARY", "#SECONDARY", "#TANK", "#LENS", "#PATTERNCOLOR", "#PAINTCOLOR", "PAINT SET", "PATTERN SET");
            RegisterSuitProfile("Default", "#843b08", "#9e6944", "#FF9B31", "#1DD44D", "#b77a54", "#E0BB9F", "A", "None");
            RegisterSuitProfile("Orange suit", "#843b08", "#9e6944", "#FF9B31", "#1DD44D", "#b77a54", "#E0BB9F", "A", "None");
            RegisterSuitProfile("Green suit", "#4f4b00", "#ae7b54", "#D67825", "#FFD6C6", "#656312", "#B9835A", "A", "B");
            RegisterSuitProfile("Hazard suit", "#f3b156", "#af6324", "#B74A38", "#FF4400", "#8e421d", "#4F3427", "C", "C");
            RegisterSuitProfile("Pajama suit", "#4b5a3b", "#a57751", "#F56C0F", "#FD9E2C", "#a37a56", "#7D5435", "A", "A");
            RegisterSuitProfile("Purple Suit", "#652352", "#652352", "#CF8224", "#FF9C00", "#995945", "#BF8B64", "B", "D");
        }
        public static void RegisterMoreSuitsMaterials()
        {
            RegisterSuitProfile("CARed", "#843b08", "#9e6944", "#FF9B31", "#1DD44D", "#b77a54", "#E0BB9F", "A", "None");
            RegisterSuitProfile("CAGreen", "#4f4b00", "#ae7b54", "#D67825", "#FFD6C6", "#656312", "#B9835A", "A", "B");
            RegisterSuitProfile("CAHaz", "#f3b156", "#af6324", "#B74A38", "#FF4400", "#8e421d", "#4F3427", "C", "C");
            RegisterSuitProfile("CAPajam", "#4b5a3b", "#a57751", "#F56C0F", "#FD9E2C", "#a37a56", "#7D5435", "A", "A");
            RegisterSuitProfile("CAPurple", "#652352", "#652352", "#CF8224", "#FF9C00", "#995945", "#BF8B64", "B", "D");
        }
        public static void RegisterVanillaModels()
        {
            ModelKit.RegisterModel("Grunt", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureA"));
            ModelKit.RegisterModel("Comms", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureB"));
            ModelKit.RegisterModel("Sentry", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureC"));
        }
        public static void RegisterSuitProfile(string suitName, string primary, string secondary, string tank, string lens, string patterncolor, string paintcolor, string paint, string pattern)
        {
            ProfileKit.CloneProfile("DEFAULT:Config", suitName);
            if (ColorUtility.TryParseHtmlString(primary, out var _))
            {
                ProfileKit.SetData(suitName, "PRIMARY", primary); //Suit Color
            }
            if (ColorUtility.TryParseHtmlString(secondary, out var _))
            {
                ProfileKit.SetData(suitName, "HOOD", secondary); //Secondary Color [Hood,Flowers,Sash]
            }
            if (ColorUtility.TryParseHtmlString(tank, out var _))
            {
                ProfileKit.SetData(suitName, "TANK", tank); //Tank Color
            }
            if (ColorUtility.TryParseHtmlString(lens, out var _))
            {
                ProfileKit.SetData(suitName, "LENS", lens); //Lens Color
            }
            if (ColorUtility.TryParseHtmlString(patterncolor, out var _))
            {
                ProfileKit.SetData(suitName, "SECONDARY", patterncolor); //Pattern Color
            }
            if (ColorUtility.TryParseHtmlString(paintcolor, out var _))
            {
                ProfileKit.SetData(suitName, "PAINTCOLOR", paintcolor); //Paint Color
            }
            ProfileKit.SetData(suitName, "PAINT", paint); //Paint Set
            ProfileKit.SetData(suitName, "PATTERN", pattern); //Pattern Set
        }
        public static void SetDefaultProfile()
        {
            var defaultProfile = $"DEFAULT:Config";
            ProfileKit.SetData(defaultProfile, "PRIMARY", "#B76912");
            ProfileKit.SetData(defaultProfile, "HOOD", "#DBBCA2");
            ProfileKit.SetData(defaultProfile, "TANK", "#FF9B31");
            ProfileKit.SetData(defaultProfile, "LENS", "#1DD44D");
            ProfileKit.SetData(defaultProfile, "SECONDARY", "#FDD9C9");
            ProfileKit.SetData(defaultProfile, "PAINTCOLOR", "#E0BB9F");
            ProfileKit.SetData(defaultProfile, "PAINT", "A");
            ProfileKit.SetData(defaultProfile, "PATTERN", "NONE");
        }
    }
    [HarmonyPatch]
    static class OptionsPatch
    {
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(ref StartOfRound __instance)
        {
            ProfileKit.ClearAllProfilesFiltered(":Config", "DEFAULT");
            MaterialKit.ClearMaterials();
        }
    }
}