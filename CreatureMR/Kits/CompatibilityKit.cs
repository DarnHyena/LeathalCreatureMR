using BepInEx.Bootstrap;
using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using UnityEngine;
using CackleCrewMR.Helpers;

namespace CackleCrew.ThisIsMagical
{
    // Bepenex Utilities
    partial class CompatibilityKit
    {
        public static bool HasPluginGUID(string pluginGUID)
        {
            bool found = false;
            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID.Equals(pluginGUID))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
    //x754 More Suits Check
    partial class CompatibilityKit
    {
        public static bool Has_x753MoreSuits { get { return HasPluginGUID("x753.More_Suits"); } }
    }
    //Old Share Code Compatibility
    partial class CompatibilityKit
    {
        public static void ValidateSavedProfiles()
        {   
            var tempProfile = Profile.CloneProfile("TEMP:PROFILE:IGNORE",ProfileHelper.DefaultProfile);
            if(IsOldProfileData(SavedProfileHelper.currentConfig.Value))
            {
                Deserialize_OldProfileData(tempProfile, SavedProfileHelper.currentConfig.Value);
                SavedProfileHelper.currentConfig.Value = tempProfile.Serialize();
            }
            foreach(var pair in SavedProfileHelper.savedConfigs)
            {
                tempProfile = Profile.ReflectProfile("TEMP:PROFILE:IGNORE", ProfileHelper.DefaultProfile);
                if (IsOldProfileData(pair.Value.Value))
                {
                    Deserialize_OldProfileData(tempProfile, pair.Value.Value);
                    pair.Value.Value = tempProfile.Serialize();
                }
            }
            Profile.DeleteProfile("TEMP:PROFILE:IGNORE");
        }
        public static bool IsOldProfileData(string profileData)
        {
            return profileData.StartsWith("LCpf");
        }
        public static void Deserialize_OldProfileData(Profile profile, string profileData)
        {
            if (!IsOldProfileData(profileData))
                return;
            try
            {
                string decompressed = DecompressString(profileData.Substring(4));
                string[] split = decompressed.Split(';');
                for (int i = 0; i < split.Length; i += 2)
                {
                    profile.SetData("OUTFIT","FALSE");
                    switch (split[i])
                    {
                        case "MODEL":
                            string model;
                            switch (split[i + 1])
                            {
                                case "Comms":
                                    model = "B";
                                    break;
                                case "Sentry":
                                    model = "C";
                                    break;
                                default:
                                    model = "A";
                                    break;
                            }
                            profile.SetData("MODEL", model);
                            break;
                        default:
                            if (profile.HasData(split[i]))
                                profile.SetData(split[i], split[i + 1]);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Provided tokenString could not be Deserialized or Read.");
                Debug.LogError(e);
            }
        }
        private static string DecompressString(string input)
        {
            byte[] compressedBytes = Convert.FromBase64String(input);
            using (MemoryStream inputStream = new MemoryStream(compressedBytes))
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputStream);
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }
    }
}
