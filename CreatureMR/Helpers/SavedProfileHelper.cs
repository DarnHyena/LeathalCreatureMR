using CackleCrew.ThisIsMagical;
using CreatureModelReplacement;

namespace CackleCrewMR.Helpers;

public static class SavedProfileHelper
{
    private const string CurrentProfileName = "Current";
    internal static string CurrentProfileData
    {
        get => Plugin.Configuration.ReadProfileData(CurrentProfileName);
        set => Plugin.Configuration.SaveProfileData(CurrentProfileName, value);
    }

    public static void UpdatePlayerProfile(Profile profile)
    {
        if(CompatibilityKit.IsOldProfileData(CurrentProfileData))
            CompatibilityKit.Deserialize_OldProfileData(profile, CurrentProfileData);
        else
            profile.Deserialize(CurrentProfileData);
        profile.SetData("OUTFIT", Plugin.Configuration.OutfitEnabled.ToString().ToUpperInvariant());
    }

    public static void UpdatePlayerProfile(string profileName)
    {
        UpdatePlayerProfile(ProfileHelper.TouchPlayerProfile(profileName));
    }

    public static void UpdateCurrentConfig(Profile profile)
    {
        var profileData = profile.Serialize();
        Plugin.Configuration.SaveProfileData(
            CurrentProfileName,
            string.IsNullOrWhiteSpace(profileData) ? "None" : profileData
        );
    }

    public static void SaveConfig(string configName)
    {
        Plugin.Configuration.SaveProfileData(configName, Plugin.Configuration.ReadProfileData(CurrentProfileName));
    }

    public static void LoadConfig(string configName)
    {
        Plugin.Configuration.SaveProfileData(CurrentProfileName, Plugin.Configuration.ReadProfileData(configName));
    }

    internal static string GetProfileData(ProfileNames name)
    {
        return Plugin.Configuration.ReadProfileData(name.ToString());
    }

    internal static void SaveProfileData(ProfileNames name, string data)
    {
        Plugin.Configuration.SaveProfileData(name.ToString(), data);
    }
}