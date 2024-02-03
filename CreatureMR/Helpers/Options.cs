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
            RegisterSuitProfile("Default", "#bb6c14", "#FFFFFF", "#e1d4b6", "#ffcd31", "#77BCA2", "NONE", "A");
            RegisterSuitProfile("Orange suit", "#bb6c14", "#FFFFFF", "#e1d4b6", "#ffcd31", "#77BCA2", "NONE", "A");
            RegisterSuitProfile("Green suit", "#728f14", "#63780e", "#e1d4b6", "#ffcd31", "#FFFFFF", "A", "A");
            RegisterSuitProfile("Hazard suit", "#e7b332", "#FFFFFF", "#e7b332", "#b45c00", "#BD4C00", "NONE", "A");
            RegisterSuitProfile("Pajama suit", "#6bac9c", "#FFFFFF", "#FFFFFF", "#f99a00", "#D7862D", "A", "A");
            RegisterSuitProfile("Purple Suit", "#883fbd", "#FFFFFF", "#a7885d", "#ffcd31", "#e1d4b6", "NONE", "B");
        }
        public static void RegisterMoreSuitsMaterials()
        {
            RegisterSuitProfile("CARed", "#bb6c14", "#FFFFFF", "#e1d4b6", "#ffcd31", "#77BCA2", "NONE", "A");
            RegisterSuitProfile("CAGreen", "#728f14", "#63780e", "#e1d4b6", "#ffcd31", "#FFFFFF", "A", "A");
            RegisterSuitProfile("CAHaz", "#e7b332", "#FFFFFF", "#e7b332", "#b45c00", "#BD4C00", "NONE", "A");
            RegisterSuitProfile("CAPajam", "#6bac9c", "#FFFFFF", "#FFFFFF", "#f99a00", "#D7862D", "A", "A");
            RegisterSuitProfile("CAPurple", "#883fbd", "#FFFFFF", "#a7885d", "#ffcd31", "#e1d4b6", "NONE", "B");
        }
        public static void RegisterVanillaModels()
        {
            ModelKit.RegisterModel("Grunt", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureA"));
            ModelKit.RegisterModel("Comms", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureB"));
            ModelKit.RegisterModel("Sentry", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureC"));
        }
        public static void RegisterSuitProfile(string suitName, string primary, string secondary, string hood, string tank, string lens, string pattern, string paint)
        {
            ProfileKit.CloneProfile("DEFAULT:Config", suitName);
            if (ColorUtility.TryParseHtmlString(primary, out var _))
            {
                ProfileKit.SetData(suitName, "PRIMARY", primary);
            }
            if (ColorUtility.TryParseHtmlString(secondary, out var _))
            {
                ProfileKit.SetData(suitName, "SECONDARY", secondary);
            }
            if (ColorUtility.TryParseHtmlString(hood, out var _))
            {
                ProfileKit.SetData(suitName, "HOOD", hood);
            }
            if (ColorUtility.TryParseHtmlString(tank, out var _))
            {
                ProfileKit.SetData(suitName, "TANK", tank);
            }
            if (ColorUtility.TryParseHtmlString(lens, out var _))
            {
                ProfileKit.SetData(suitName, "LENS", lens);
            }
            ProfileKit.SetData(suitName, "PATTERN", pattern);
            ProfileKit.SetData(suitName, "PAINT", paint);
        }
        public static void SetDefaultProfile()
        {
            var defaultProfile = $"DEFAULT:Config";
            ProfileKit.SetData(defaultProfile, "PRIMARY", "#9D4E0D");
            ProfileKit.SetData(defaultProfile, "SECONDARY", "#FFFFFF");
            ProfileKit.SetData(defaultProfile, "HOOD", "#D6B8A7");
            ProfileKit.SetData(defaultProfile, "TANK", "#ffcd31");
            ProfileKit.SetData(defaultProfile, "LENS", "#77BCA2");
            ProfileKit.SetData(defaultProfile, "PATTERN", "NONE");
            ProfileKit.SetData(defaultProfile, "PAINT", "A");
            ProfileKit.SetData(defaultProfile, "PAINTCOLOR", "#E1D4B6");
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