using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace CackleCrew.ThisIsMagical
{
    static class SyncManager
    {
        public static string SerializationTag = "[cacklecrew]";
        public static void SyncPlayerConfig(PlayerControllerB controller)
        {
            if (StartOfRound.Instance.localPlayerController.OwnerClientId == controller.OwnerClientId)
            {
                SuitKit.SwitchSuitOfPlayer(controller, controller.currentSuitID);
                SendPlayerConfig(controller);
            }
        }
        public static void SendPlayerConfig(PlayerControllerB controller)
        {
            List<string> tokens = new List<string>();
            ulong ourID = controller.OwnerClientId;
            string ourProfile = $"{ourID}:Config";
            if (!ProfileKit.TryGetProfile(ourProfile, out _))
            {
                ProfileKit.CloneProfile("DEFAULT:Config", ourProfile);
            }
            string ourModelName = ProfileKit.GetData(ourProfile, "MODEL");
            tokens.Add(SerializationTag);
            tokens.Add(ourID.ToString());
            ProfileKit.TryGetData(ourProfile, "OUTFIT", out var outfit);
            if (!string.IsNullOrWhiteSpace(outfit) && outfit == "TRUE")
            {
                tokens.Add("CUSTOM");
                var profile = ProfileKit.GetProfile(ourProfile);
                foreach (var entry in profile)
                {
                    string tokenPair = $"{entry.Key}:{entry.Value}";
                    tokens.Add(tokenPair);
                }
            }
            else
            {
                tokens.Add(ourModelName);
            }
            SyncDataKit.SendChatData(tokens.ToArray());
        }
        public static void ReceivePlayerConfig(string[] tokens)
        {
            if (tokens.Length < 3)
                return;
            if (!ulong.TryParse(tokens[1], out var clientID))
                return;
            if (StartOfRound.Instance.localPlayerController.OwnerClientId == clientID)
                return;
            if (!StartOfRound.Instance.ClientPlayerList.TryGetValue(clientID, out var playerID))
                return;
            PlayerControllerB controller = StartOfRound.Instance.allPlayerObjects[playerID].GetComponent<PlayerControllerB>();
            string ourProfile = $"{clientID}:Config";
            if (!ProfileKit.TryGetProfile(ourProfile, out _))
            {
                ProfileKit.CloneProfile("DEFAULT:Config", ourProfile);
            }
            if (tokens[2] == "CUSTOM")
            {
                for (int i = 3; i < tokens.Length; i++)
                {
                    var pair = tokens[i].Split(':');
                    ProfileKit.SetData(ourProfile, pair[0], pair[1]);
                }
            }
            else
            {
                ProfileKit.SetData(ourProfile, "MODEL", tokens[2]);
                ProfileKit.ClearData(ourProfile, "OUTFIT");
            }
            SuitKit.SwitchSuitOfPlayer(controller, controller.currentSuitID);
            ModelReplacement.ModelReplacementAPI.ResetPlayerModelReplacement(controller);
        }
    }
    [HarmonyPatch]
    static class SyncManager_Patches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void ConnectClientToPlayerObject_Postfix(PlayerControllerB __instance)
        {
            SyncManager.SyncPlayerConfig(__instance);
        }
        [HarmonyPatch(typeof(StartOfRound), "SyncShipUnlockablesClientRpc")]
        [HarmonyPostfix]
        private static void SyncShipUnlockablesClientRpc_Postfix(StartOfRound __instance)
        {
            if (__instance.IsServer)
                return;
            var controller = StartOfRound.Instance.localPlayerController;
            SyncManager.SyncPlayerConfig(controller);
        }
        [HarmonyPatch(typeof(StartOfRound), "SyncShipUnlockablesServerRpc")]
        [HarmonyPostfix]
        private static void SyncShipUnlockablesServerRpc_Postfix(StartOfRound __instance)
        {
            var controller = StartOfRound.Instance.localPlayerController;
            SyncManager.SyncPlayerConfig(controller);
        }
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void Start_Postfix(ref StartOfRound __instance)
        {
            SyncDataKit.AttachListener(SyncManager.SerializationTag, SyncManager.ReceivePlayerConfig);
        }
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(ref StartOfRound __instance)
        {
            SyncDataKit.RemoveListener(SyncManager.SerializationTag);
        }
    }
}
