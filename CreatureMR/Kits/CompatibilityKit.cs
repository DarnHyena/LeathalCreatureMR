using BepInEx.Bootstrap;

namespace CackleCrew.ThisIsMagical
{
    class CompatibilityKit
    {

        public static bool Has_x753MoreSuits { get { return HasPluginGUID("x753.More_Suits"); } }
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
}
