using BepInEx.Configuration;
using CackleCrew.ThisIsMagical;
using CreatureModelReplacement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CackleCrewMR.Helpers
{
    public static class SavedProfileHelper
    {
        public static Dictionary<string, ConfigEntry<string>> savedConfigs = new Dictionary<string, ConfigEntry<string>>();
        public static ConfigEntry<string> currentConfig;
        public static ConfigEntry<string> outfitEnabledConfig;
        public static string CurrentConfig
        {
            get { return currentConfig.Value; }
        }
        public static bool UseOutfits
        {
            get { return outfitEnabledConfig.Value == "TRUE"; }
            set { outfitEnabledConfig.Value = (value) ? "TRUE" : "FALSE"; }
        }
        public static void Init()
        {
            CreateDefaultProfileConfig();
            currentConfig = CreateProfileConfig("Current");
            CreateProfileConfig("A");
            CreateProfileConfig("B");
            CreateProfileConfig("C");
            CreateProfileConfig("D");
            //Eventually might force old-profile save data to be converted.
            //For now I'll implement a temperary check to attempt to load old data instead of overriding it.
            //CompatibilityKit.ValidateSavedProfiles();
        }
        public static void CreateDefaultProfileConfig()
        {
            outfitEnabledConfig = Plugin.config.Bind<string>("Saved Profiles", "Use Suit Outfits", "TRUE");
        }
        public static ConfigEntry<string> CreateProfileConfig(string configName)
        {
            if (savedConfigs.ContainsKey(configName))
                return savedConfigs[configName];
            var config = Plugin.config.Bind<string>("Saved Profiles", $"Profile {configName}", "None");
            savedConfigs.Add(configName, config);
            return config;
        }
        public static void UpdatePlayerProfile(Profile profile)
        {
            if(CompatibilityKit.IsOldProfileData(currentConfig.Value))
                CompatibilityKit.Deserialize_OldProfileData(profile, currentConfig.Value);
            else
                profile.Deserialize(currentConfig.Value);
            if (UseOutfits)
            {
                profile.SetData("OUTFIT", "TRUE");
            }
            else
            {
                profile.SetData("OUTFIT", "FALSE");
            }
        }
        public static void UpdatePlayerProfile(string profileName)
        {
            UpdatePlayerProfile(ProfileHelper.TouchPlayerProfile(profileName));
        }
        public static void UpdateConfig(Profile profile)
        {
            var profileData = profile.Serialize();
            if (string.IsNullOrWhiteSpace(profileData))
            {
                currentConfig.Value = "None";
            }
            else
            {
                currentConfig.Value = profileData;
            }
        }
        public static void UpdateConfig(string profileName)
        {
            UpdateConfig(ProfileHelper.TouchPlayerProfile(profileName));
        }
        public static void SaveConfig(string configName)
        {
            if (!savedConfigs.TryGetValue(configName, out var config))
                return;
            config.Value = currentConfig.Value;
        }
        public static void LoadConfig(string configName)
        {
            if (!savedConfigs.TryGetValue(configName, out var config))
                return;
            currentConfig.Value = config.Value;
        }
    }
}
