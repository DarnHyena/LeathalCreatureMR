using HarmonyLib;

namespace CackleCrew.ThisIsMagical
{
    static class ProfileHelper
    {
        public static void SwitchModel(string name)
        {
            if (ModelKit.HasModel(name))
            {

                var controller = StartOfRound.Instance?.localPlayerController;
                if (controller == null)
                    return;
                string ourProfile = $"{controller.OwnerClientId}:Config";
                ForceSyncUpdate();
            }
        }
        public static string GetModel()
        {
            var controller = StartOfRound.Instance?.localPlayerController;
            if (controller == null)
                return ModelKit.GetDefaultModel();
            string ourProfile = $"{controller.OwnerClientId}:Config";
            TouchPlayerProfile(ourProfile);
            return ProfileKit.GetData(ourProfile, "MODEL");
        }
        public static void ForceSyncUpdate()
        {
            var controller = StartOfRound.Instance?.localPlayerController;
            if (controller == null)
                return;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            TouchPlayerProfile(ourProfile);
            ModelReplacement.ModelReplacementAPI.ResetPlayerModelReplacement(controller);
            SyncManager.SyncPlayerConfig(controller);
        }
        public static void TouchPlayerProfile(string profileName)
        {
            if (!ProfileKit.TryGetProfile(profileName, out _))
            {
                ProfileKit.CloneProfile("DEFAULT:Config", profileName);
            }
        }
    }
    [HarmonyPatch]
    static class CreatureSwitcher_Patches
    {
        /*[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void ConnectClientToPlayerObject_Postfix(PlayerControllerB __instance)
        {
            CreatureSwitcher.ourTracker = StartOfRound.Instance.StartCoroutine(CreatureSwitcher.StartTrackingSwitching());

        }
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(ref StartOfRound __instance)
        {
            StartOfRound.Instance.StopCoroutine(CreatureSwitcher.ourTracker);
        }*/
    }
}
