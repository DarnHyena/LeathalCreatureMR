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
            RegisterSuitMaterials();
            RegisterMoreSuitsMaterials();
        }
        public static void RegisterSuitMaterials()
        {
            //Creature
            RegisterProfileData("A:Default", "vA1A93C14C38F68FF8E001DD44DFDD9C9E0BB9F10");
            RegisterProfileData("A:Orange suit", "vA1A93C14C38F68FF8E001DD44DFDD9C9E0BB9F10");
            RegisterProfileData("A:Green suit", "SA1687600F1DCC7D67825FFD6C69A952CB9835A12");
            RegisterProfileData("A:Hazard suit", "YA1E2913DE2913DC35220FF4400B24E1C4F342733");
            RegisterProfileData("A:Pajama suit", "YA164937DE0C3A6F56C0FFF7D01E0C3A6ECA87511");
            RegisterProfileData("A:Purple Suit", "lA1733B90733B90CF7A24FF9C00A15F3CBE866442");
            //Navigator
            RegisterProfileData("B:Default", "rB19A421FCC8E269A948F6DC57DCC9882B2836613");
            RegisterProfileData("B:Orange suit", "rB19A421FCC8E269A948F6DC57DCC9882B2836613");
            RegisterProfileData("B:Green suit", "QB171730E623F13C3930DF8CA89E3A931B2836612");
            RegisterProfileData("B:Hazard suit", "jB1DB8205BE3C11E9B283EC310A9C1A0767350F33");
            RegisterProfileData("B:Pajama suit", "nB1578A73B28366C86025FF8414B99B71B2836611");
            RegisterProfileData("B:Purple Suit", "rB17E43B4CC8E26935B83FFA0009C5B3FB2836624");
            //Brawler
            RegisterProfileData("C:Default", "rC1A44810C36E10FF9D0441CF55D4BA9FE0BB9F11");
            RegisterProfileData("C:Orange suit", "rC1A44810C36E10FF9D0441CF55D4BA9FE0BB9F11");
            RegisterProfileData("C:Green suit", "zC167781DE27E0CBC3026F1C689D4BA9FE0BB9F10");
            RegisterProfileData("C:Hazard suit", "MC1D17919873BECE02E00E02E0072381F542F1532");
            RegisterProfileData("C:Pajama suit", "SC15C9F69FF9E00FF5F04FF5F04DEEE98E0BB9F13");
            RegisterProfileData("C:Purple Suit", "QC17243B9FF8600C9B9A1D42A1BC5884DE0BB9F44");
        }
        public static void RegisterMoreSuitsMaterials()
        {
            //Creature
            Profile.CloneProfile("A:CARed", "A:Default");
            Profile.CloneProfile("A:CAGreen", "A:Green suit");
            Profile.CloneProfile("A:CAHaz", "A:Hazard suit");
            Profile.CloneProfile("A:CAPajam", "A:Pajama suit");
            Profile.CloneProfile("A:CAPurple", "A:Purple Suit");
            //Navigator
            Profile.CloneProfile("B:CARed", "B:Default");
            Profile.CloneProfile("B:CAGreen", "B:Green suit");
            Profile.CloneProfile("B:CAHaz", "B:Hazard suit");
            Profile.CloneProfile("B:CAPajam", "B:Pajama suit");
            Profile.CloneProfile("B:CAPurple", "B:Purple Suit");
            //Brawler
            Profile.CloneProfile("C:CARed", "C:Default");
            Profile.CloneProfile("C:CAGreen", "C:Green suit");
            Profile.CloneProfile("C:CAHaz", "C:Hazard suit");
            Profile.CloneProfile("C:CAPajam", "C:Pajama suit");
            Profile.CloneProfile("C:CAPurple", "C:Purple Suit");
        }
        public static void RegisterVanillaModels()
        {
            ModelKit.RegisterModel("A", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureA"));
            ModelKit.RegisterModel("B", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureB"));
            ModelKit.RegisterModel("C", Assets.MainAssetBundle.LoadAsset<GameObject>("CreatureC"));
        }
        public static void RegisterProfileData(string profileName, string profileData)
        {
            var profile = ProfileHelper.TouchPlayerProfile(profileName);
            profile.Deserialize(profileData);
            profile.SetData("OUTFIT","FALSE");
        }
        public static void SetDefaultProfile()
        {
            var profile = Profile.CreateProfile(ProfileHelper.DefaultProfile);
            profile.SetData("MODEL", "A", false, ProfileDataType.Character);
            profile.SetData("OUTFIT", "FALSE", false, ProfileDataType.Boolean);
            profile.SetData("PRIMARY", "#A93C14", false, ProfileDataType.Color);
            profile.SetData("HOOD", "#C38F68", false, ProfileDataType.Color);
            profile.SetData("TANK", "#FF8E00", false, ProfileDataType.Color);
            profile.SetData("LENS", "#1DD44D", false, ProfileDataType.Color);
            profile.SetData("SECONDARY", "#FDD9C9", false, ProfileDataType.Color);
            profile.SetData("PAINTCOLOR", "#E0BB9F", false, ProfileDataType.Color);
            profile.SetData("PAINT", "A", false, ProfileDataType.ShaderOption);
            profile.SetData("PATTERN", "NONE", false, ProfileDataType.ShaderOption);
        }
    }
    [HarmonyPatch]
    static class OptionsPatch
    {
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(ref StartOfRound __instance)
        {
            Profile.FilterDeleteProfiles(new string[] { ProfileHelper.DEFAULT_PROFILE_ID }, new string[] { ProfileHelper.PROFILE_POSTFIX });
            MaterialKit.ClearMaterials();
        }
    }
}