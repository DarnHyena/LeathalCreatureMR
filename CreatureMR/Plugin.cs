using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using LethalCreatureMR.JigglePhysics;

//using System.Numerics;

namespace CreatureModelReplacement
{


    [BepInPlugin("CreaturelReplacement", "Lethal Creature", "2.0.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigFile config;

        // Universal config options 
        public static ConfigEntry<bool> enableCreatureForAllSuits { get; private set; }
        public static ConfigEntry<bool> enableCreatureAsDefault { get; private set; }
        public static ConfigEntry<string> suitNamesToEnableCreature { get; private set; }
        
        // Model specific config options
        public static ConfigEntry<float> UpdateRate { get; private set; }
        public static ConfigEntry<float> distanceDisablePhysics { get; private set; }
        public static ConfigEntry<bool> disablePhysicsAtRange { get; private set; }

        private static void InitConfig()
        {
            enableCreatureForAllSuits = config.Bind<bool>("Suits to Replace Settings", "Enable Creature for all Suits", false, "Enable to replace every suit with Creature. Set to false to specify suits");
            enableCreatureAsDefault = config.Bind<bool>("Suits to Replace Settings", "Enable Creature as default", false, "Enable to replace every suit that hasn't been otherwise registered with Creature.");
            suitNamesToEnableCreature = config.Bind<string>("Suits to Replace Settings", "Suits to enable Creature for", "LC_Red", "Enter a comma separated list of suit names.(Additionally, [Green suit,Pajama suit,Hazard suit])");

            UpdateRate = config.Bind<float>("Dynamic Bone Settings", "Update rate", 30, "Refreshes dynamic bones more times per second the higher the number");
            disablePhysicsAtRange = config.Bind<bool>("Dynamic Bone Settings", "Disable physics at range", false, "Enable to disable physics past the specified range");
            distanceDisablePhysics = config.Bind<float>("Dynamic Bone Settings", "Distance to disable physics", 20, "If Disable physics at range is enabled, this is the range after which physics is disabled.");
            
        }
        private void Awake()
        {
            config = base.Config;
            InitConfig();
            Assets.PopulateAssets();

            // Plugin startup logic


            if (enableCreatureForAllSuits.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementOverride(typeof(BodyReplacement));

            }
            if (enableCreatureAsDefault.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementDefault(typeof(BodyReplacement));

            }

            var commaSepList = suitNamesToEnableCreature.Value.Split(',');
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