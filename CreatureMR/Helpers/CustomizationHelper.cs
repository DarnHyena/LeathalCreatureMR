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
            string ourProfile = $"{ourID}:Config";
            bool isNewProfile = !ProfileKit.TryGetProfile(ourProfile, out _);
            ProfileHelper.TouchPlayerProfile(ourProfile);
            //If the profile is first initiated, Let's put our saved profile in!
            Debug.Log($"IsNewProfile={isNewProfile}");
            Debug.Log($"IsLocalPlayer={controller.IsLocalPlayer}");
            Debug.Log($"OwnerClientId={StartOfRound.Instance.OwnerClientId}");
            Debug.Log($"OwnerClientId={ourID}");
            //Old Method here <-->
            //StartOfRound.Instance.localPlayerController.OwnerClientId == ourID)
            //Have to Test if this works -->
            if (isNewProfile && StartOfRound.Instance.OwnerClientId == ourID)
            {
                Debug.Log("THIS IS A NEW PROFILE!!!!");
                SavedProfileHelper.UpdatePlayerProfile(ourProfile);
                if (SavedProfileHelper.UseOutfits)
                {
                    ProfileKit.SetData(ourProfile, "OUTFIT", "TRUE");
                }
            }
            else
            {
                Debug.Log("SOMETHING WENT WRONG?!");
            }
            string ourModelName = ProfileKit.GetData(ourProfile, "MODEL");
            if (string.IsNullOrWhiteSpace(ourModelName))
            {
                //Default Model Stuff...!
                ProfileKit.SetData(ourProfile, "MODEL", ModelKit.GetDefaultModel());
            }
            //Suit Compatibility
            int suitID = SuitKit.GetCurrentSuitID(controller);
            string suitName = SuitKit.GetSuitName(suitID);
            if (ModelKit.HasModel(suitName))
            {
                ProfileKit.SetData(ourProfile, "MODEL", suitName);
                UnlockableSuit.SwitchSuitForPlayer(controller, SuitKit.GetLastSuitID(controller), false);
                SyncManager.SyncPlayerConfig(controller);
                suitName = SuitKit.GetLastSuitName(controller);
            }
            //Model
            GameObject ourModel = ModelKit.GetModel(ourModelName);
            //Materials
            SkinnedMeshRenderer[] renderers = ourModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            ProfileKit.TryGetData(ourProfile, "OUTFIT", out var outfit);
            string outfitProfile = suitName;
            if (!string.IsNullOrWhiteSpace(outfit) && outfit == "TRUE")
            {
                outfitProfile = ourProfile;
            }
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
            Touch_OutfitProfile(profileName);
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            material.SetColor("_SuitColor", GetColorFromSet(profileName, "PRIMARY"));
            material.SetColor("_PatternColor", GetColorFromSet(profileName, "SECONDARY"));
            material.SetColor("_HoodColor", GetColorFromSet(profileName, "HOOD"));
            material.SetColor("_TankColor", GetColorFromSet(profileName, "TANK"));
            material.SetColor("_LensColor", GetColorFromSet(profileName, "LENS"));
            material.SetColor("_PaintColor", GetColorFromSet(profileName, "PAINTCOLOR"));
            ClearKeywords(ref material, "PATTERN");
            var pattern = GetConfigFromProfile(profileName, "PATTERN");
            SetKeyword(ref material, "PATTERN", pattern);
            ClearKeywords(ref material, "PAINT");
            var paint = GetConfigFromProfile(profileName, "PAINT");
            SetKeyword(ref material, "PAINT", paint);
        }
        public static void ClearKeywords(ref Material material, string keyword)
        {
            foreach (var enabledKeyword in material.enabledKeywords)
            {
                if (enabledKeyword.name.Contains(keyword))
                    material.DisableKeyword(enabledKeyword.name);
            }
        }
        public static void SetKeyword(ref Material material, string keyword, string option)
        {
            material.EnableKeyword($"_{keyword}_{option}");
        }
        public static string GetConfigFromProfile(string profileName, string name)
        {
            if (ProfileKit.TryGetData(profileName, name, out var data))
            {
                return data;
            }
            else
            {
                return ProfileKit.GetData("DEFAULT:Config", name);
            }
        }
        public static Color GetColorFromSet(string profileName, params string[] names)
        {
            foreach (var colorname in names)
            {
                if (GetColorFromProfile(profileName, colorname, out Color color))
                {
                    return color;
                }
            }
            return Color.white;
        }
        public static bool GetColorFromProfile(string profileName, string name, out Color color)
        {
            var foundColor = GetConfigFromProfile(profileName, name);
            if (ColorUtility.TryParseHtmlString(foundColor, out Color parsedColor))
            {
                color = parsedColor;
                return true;
            }
            color = Color.white;
            return false;
        }
        //Checks if the profile exists, otherwise create one using color data.
        public static void Touch_OutfitProfile(string outfitName)
        {
            if (ProfileKit.TryGetProfile(outfitName, out _))
                return;
            int suitID = SuitKit.GetSuitID(outfitName);
            Texture2D suitTexture = SuitKit.GetSuitMaterial(suitID).mainTexture as Texture2D;
            SuitKit.SampleSuitColors(suitTexture, out var bootColor, out var suitColor, out var clothColor, out var tankColor);
            ProfileKit.CloneProfile("DEFAULT:Config", outfitName);
            ProfileKit.SetData(outfitName, "PRIMARY", $"#{ColorUtility.ToHtmlStringRGB(suitColor)}");
            ProfileKit.SetData(outfitName, "HOOD", $"#{ColorUtility.ToHtmlStringRGB(clothColor)}");
            ProfileKit.SetData(outfitName, "TANK", $"#{ColorUtility.ToHtmlStringRGB(tankColor)}");
        }
    }
}
