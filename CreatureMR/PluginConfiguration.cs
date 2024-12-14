using System;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using BepInEx;

namespace CackleCrewMR;

internal class PluginConfiguration
{
    private readonly ConfigFile _baseConfigFile;
    private readonly ConfigFile _profileConfigFile;

    // Universal config options  
    private readonly ConfigEntry<bool> _enableModelForAllSuits;
    private readonly ConfigEntry<bool> _enableModelAsDefault;
    private readonly ConfigEntry<string> _suitNamesToEnableModel;
    private readonly ConfigEntry<bool> _outfitEnabledConfig;
    private readonly Dictionary<string, ConfigEntry<string>> _savedConfigs = new();

    internal bool EnableModelForAllSuits
    {
        get => _enableModelForAllSuits.Value;
        set => _enableModelForAllSuits.Value = value;
    }

    internal bool EnableModelAsDefault
    {
        get => _enableModelAsDefault.Value;
        set => _enableModelAsDefault.Value = value;
    }

    internal string SuitNamesToEnableModel
    {
        get => _suitNamesToEnableModel.Value;
        set => _suitNamesToEnableModel.Value = value;
    }

    internal bool OutfitEnabled
    {
        get => _outfitEnabledConfig.Value;
        set => _outfitEnabledConfig.Value = value;
    }

    internal PluginConfiguration(ConfigFile baseConfigFile)
    {
        _baseConfigFile = baseConfigFile;
        _profileConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "CreatureReplacement.Profiles.cfg"), true);
        _enableModelForAllSuits = baseConfigFile.Bind("Suits to Replace Settings",
            "Enable Model for all Suits",
            false,
            "Enable to replace every suit with Model. Set to false to specify suits");

        _enableModelAsDefault = baseConfigFile.Bind("Suits to Replace Settings",
            "Enable Model as default",
            false,
            "Enable to replace every suit that hasn't been otherwise registered with Model.");

        _suitNamesToEnableModel = baseConfigFile.Bind("Suits to Replace Settings",
            "Suits to enable Model for",
            "Default,Orange suit,Green suit,Pajama suit,Hazard suit,Purple Suit",
            "For use with Moresuits, replace list with: CARed,CAGreen,CAHaz,CAPajam,CAPurp");

        var outfitEnabledConfigDefinition = new ConfigDefinition("Saved Profiles", "Use Suit Outfits");
        _outfitEnabledConfig = _profileConfigFile.Bind(outfitEnabledConfigDefinition, true);
        if (_outfitEnabledConfig.Value)
        {
            var oldOutfitEnabledConfig = _baseConfigFile.Bind(outfitEnabledConfigDefinition, "TRUE");
            _outfitEnabledConfig.Value = oldOutfitEnabledConfig.Value == "TRUE";
        }
        TransferProfilesToNewConfig();
    }

    private void TransferProfilesToNewConfig()
    {
        _ = GetProfileEntry("Current");
        foreach (var name in Enum.GetNames(typeof(ProfileNames)))
        {
            _ = GetProfileEntry(name);
        }
    }

    internal string ReadProfileData(string profileName)
    {
        return GetProfileEntry(profileName).Value;
    }

    internal void SaveProfileData(string profileName, string profileData)
    {
        GetProfileEntry(profileName).Value = profileData;
    }

    private ConfigEntry<string> GetProfileEntry(string profileName)
    {
        if (_savedConfigs.TryGetValue(profileName, out var entry))
            return entry;
        var configDefinition = new ConfigDefinition("Saved Profiles", $"Profile {profileName}");
        var newEntry = _profileConfigFile.Bind(configDefinition, "None");
        if (newEntry.Value == "None")
        {
            var oldEntry = _baseConfigFile.Bind(configDefinition, "None");
            newEntry.Value = oldEntry.Value;
        }
        _savedConfigs.Add(profileName, newEntry);
        return newEntry;
    }
}