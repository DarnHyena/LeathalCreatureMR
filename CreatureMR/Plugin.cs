using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using System;

//using System.Numerics;

namespace CreatureModelReplacement
{


    [BepInPlugin("CreatureReplacement", "Cackle Crew", "2.0.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]

    
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigFile config;

        // Universal config options  
        public static ConfigEntry<bool> enableModelForAllSuits { get; private set; }
        public static ConfigEntry<bool> enableModelAsDefault { get; private set; }
        public static ConfigEntry<string> suitNamesToEnableModel { get; private set; }
        

        private static void InitConfig()
        {
            enableModelForAllSuits = config.Bind<bool>("Suits to Replace Settings", "Enable Model for all Suits", false, "Enable to replace every suit with Model. Set to false to specify suits");
            enableModelAsDefault = config.Bind<bool>("Suits to Replace Settings", "Enable Model as default", false, "Enable to replace every suit that hasn't been otherwise registered with Model.");
            suitNamesToEnableModel = config.Bind<string>("Suits to Replace Settings", "Suits to enable Model for", "Default,Orange suit,Green suit,Pajama suit,Hazard suit", "Enter a comma separated list of suit names.(Additionally, [Green suit,Pajama suit,Hazard suit])");
            
        }
        private void Awake()
        {
            config = base.Config;
            InitConfig();
            Assets.PopulateAssets();

            // Plugin startup logic

            if (enableModelForAllSuits.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementOverride(typeof(BodyReplacement));

            }
            
            if (enableModelAsDefault.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementDefault(typeof(BodyReplacement));

            }

            var commaSepList = suitNamesToEnableModel.Value.Split(',');
            foreach (var item in commaSepList)
            {
                ModelReplacementAPI.RegisterSuitModelReplacement(item, typeof(BodyReplacement));
            }
            
            
            Harmony harmony = new Harmony("LeCreature");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {"CreaturelReplacement"} is loaded!");
        }
    }
    public static class Assets
    {
        // Replace mbundle with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "lecreature";
        public static AssetBundle MainAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }

}