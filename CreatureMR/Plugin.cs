using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using CackleCrew.ThisIsMagical;
using CackleCrewMR;

//using System.Numerics;

namespace CreatureModelReplacement
{


    [BepInPlugin("CreatureReplacement", "Cackle Crew", "3.0.0")] //Name of Config / Name of Mod / Version number
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]


    public class Plugin : BaseUnityPlugin
    {
        internal static PluginConfiguration Configuration { get; private set; }

        private static void InitConfig(ConfigFile file)
        {
            Configuration = new PluginConfiguration(file);
        }

        private void Awake()
        {
            InitConfig(Config);
            Assets.PopulateAssets();

            // Plugin startup logic

            if (Configuration.EnableModelForAllSuits)
            {
                ModelReplacementAPI.RegisterModelReplacementOverride(typeof(BodyReplacement));

            }

            if (Configuration.EnableModelAsDefault)
            {
                ModelReplacementAPI.RegisterModelReplacementDefault(typeof(BodyReplacement));

            }

            var commaSepList = Configuration.SuitNamesToEnableModel.Split(',');
            foreach (var item in commaSepList)
            {
                ModelReplacementAPI.RegisterSuitModelReplacement(item, typeof(BodyReplacement));
            }


            Harmony harmony = new Harmony("LeCreature");
            harmony.PatchAll();
            //Setup Customization...!
            Options.Init();
            Logger.LogInfo($"Plugin {"CreatureReplacement"} is loaded!");
        }
    }
    public static class Assets
    {
        // Replace lecreature with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "lecreature";
        public static string customizationAssetBundleName = "lecustomization";
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundle CustomizationAssetBundle = null;

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
            if (CustomizationAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + customizationAssetBundleName))
                {
                    CustomizationAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }

}