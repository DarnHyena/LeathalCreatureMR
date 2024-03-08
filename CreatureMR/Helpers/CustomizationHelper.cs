using CackleCrew.ThisIsMagical;
using CackleCrewMR.Helpers;
using GameNetcodeStuff;
using UnityEngine;

namespace CackleCrew.Helpers
{
    //General Utility Functions to handle the over-all Customization of the model.
    static class CustomizationHelper
    {
        public static GameObject GenerateCustomModel(PlayerControllerB controller)
        {
            //Profile Stuff
            ulong ourID = controller.OwnerClientId;
            string ourProfile = ProfileHelper.GetProfileName(ourID);
            bool isNewProfile = !Profile.HasProfile(ourProfile);
            Profile profile = ProfileHelper.TouchPlayerProfile(ourProfile);
            if (isNewProfile && ProfileHelper.IsLocalPlayer(ourID))
            {
                SavedProfileHelper.UpdatePlayerProfile(ourProfile);
                if (SavedProfileHelper.UseOutfits)
                {
                    profile.SetData("OUTFIT", "TRUE");
                }
            }
            string ourModelName = profile.GetData("MODEL");
            //Suit Compatibility
            int suitID = SuitKit.GetCurrentSuitID(controller);
            string suitName = SuitKit.GetSuitName(suitID);
            //Removed Suit-Model Switching, No Longer Needed..!
            //Model
            GameObject ourModel = ModelKit.GetModel(ourModelName);
            //Materials
            SkinnedMeshRenderer[] renderers = ourModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            bool using_outfits = profile.GetData("OUTFIT").ToUpper() == "TRUE" ? true : false;
            string outfitProfile = (using_outfits) ? ourProfile : $"{ourModelName}:{suitName}";
            if (!MaterialKit.TryGetMaterial($"{ourModelName}:{outfitProfile}", out var material))
            {
                material = new Material(renderers[0].material);
                material.name = $"{ourModelName}:{outfitProfile}(CustomMaterial)";
                MaterialKit.SetMaterial($"{ourModelName}:{outfitProfile}", material);
            }
            ApplyProfileToMaterial(ref material, outfitProfile);
            renderers[0].material = material;
            //Done
            return ourModel;
        }
        public static void ApplyProfileToMaterial(ref Material material, string profileName)
        {
            var profile = Touch_OutfitProfile(profileName);
            material.SetColor("_SuitColor", GetColorFromProfile(profile, "PRIMARY"));
            material.SetColor("_PatternColor", GetColorFromProfile(profile, "SECONDARY"));
            material.SetColor("_HoodColor", GetColorFromProfile(profile, "HOOD"));
            material.SetColor("_TankColor", GetColorFromProfile(profile, "TANK"));
            material.SetColor("_LensColor", GetColorFromProfile(profile, "LENS"));
            material.SetColor("_PaintColor", GetColorFromProfile(profile, "PAINTCOLOR"));
            ApplyConfigFromProfile(ref material, profile, "PATTERN");
            ApplyConfigFromProfile(ref material, profile, "PAINT");
        }
        public static Color GetColorFromProfile(Profile profile, string name)
        {
            var foundColor = (profile != null) ? profile.GetData(name) : string.Empty;
            if (!ColorUtility.TryParseHtmlString(foundColor, out Color color))
                color = Color.white;
            return color;
        }
        public static void ApplyConfigFromProfile(ref Material material, Profile profile, string name)
        {
            ShaderKit.ClearKeywords(ref material, name);
            var paint = (profile != null) ? profile.GetData(name) : "None";
            ShaderKit.SetKeyword(ref material, name, paint);
        }
        //Checks if the profile exists, otherwise create one using color data.
        public static Profile Touch_OutfitProfile(string profileName)
        {
            if (Profile.TryGetProfile(profileName, out var profile))
                return profile;
            var suitName = profileName;
            if (profileName.Contains(':'))
                suitName = profileName.Substring(profileName.IndexOf(':') + 1);
            int suitID = SuitKit.GetSuitID(suitName);
            Texture2D suitTexture = SuitKit.GetSuitMaterial(suitID).mainTexture as Texture2D;
            SuitKit.SampleSuitColors(suitTexture, out var bootColor, out var suitColor, out var clothColor, out var tankColor);
            profile = Profile.CloneProfile(profileName, ProfileHelper.DefaultProfile);
            profile.SetData("PRIMARY", ColorUtility.ToHtmlStringRGB(suitColor), true);
            profile.SetData("HOOD", ColorUtility.ToHtmlStringRGB(clothColor), true);
            profile.SetData("TANK", ColorUtility.ToHtmlStringRGB(tankColor), true);
            return profile;
        }
    }
}
