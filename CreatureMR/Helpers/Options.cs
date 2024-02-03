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
            RegisterSuitProfile("Default", "#B76912", "#DBBCA2", "#FF9B31", "#1DD44D", "#FDD9C9", "#E0BB9F", "A", "None");
            RegisterSuitProfile("Orange suit", "#B76912", "#DBBCA2", "#FF9B31", "#1DD44D", "#FDD9C9", "#E0BB9F", "A", "None");
            RegisterSuitProfile("Green suit", "#6D8500", "#F1DCC7", "#D67825", "#FFD6C6", "#8CB02A", "#B9835A", "A", "B");
            RegisterSuitProfile("Hazard suit", "#F3B156", "#F3B156", "#B74A38", "#FF4400", "#C57644", "#4F3427", "C", "C");
            RegisterSuitProfile("Pajama suit", "#68A18D", "#E5D4C1", "#F56C0F", "#FD9E2C", "#E2D9CC", "#7D5435", "A", "A");
            RegisterSuitProfile("Purple Suit", "#8C3EC3", "#8C3EC3", "#CF8224", "#FF9C00", "#D49FA5", "#BF8B64", "B", "D");
        }
        public static void RegisterMoreSuitsMaterials()
        {
            RegisterSuitProfile("CARed", "#B76912", "#DBBCA2", "#FF9B31", "#1DD44D", "#FDD9C9", "#E0BB9F", "A", "None");
            RegisterSuitProfile("CAGreen", "#6D8500", "#F1DCC7", "#D67825", "#FFD6C6", "#8CB02A", "#B9835A", "A", "B");
            RegisterSuitProfile("CAHaz", "#F3B156", "#F3B156", "#B74A38", "#FF4400", "#C57644", "#4F3427", "C", "C");
            RegisterSuitProfile("CAPajam", "#68A18D", "#E5D4C1", "#F56C0F", "#FD9E2C", "#E2D9CC", "#7D5435", "A", "A");
            RegisterSuitProfile("CAPurple", "#8C3EC3", "#8C3EC3", "#CF8224", "#FF9C00", "#D49FA5", "#BF8B64", "B", "D");
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