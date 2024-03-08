using CackleCrewMR.Helpers;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    static class SyncManager
    {
        public const string CACKLECREW_TAG = "[CA]";
        public static void SyncPlayerConfig(PlayerControllerB controller)
        {
            if (ProfileHelper.IsLocalPlayer(controller.OwnerClientId))
            {
                SuitKit.SwitchSuitOfPlayer(controller, controller.currentSuitID);
                SendPlayerConfig(controller);
            }
        }
        public static void SendPlayerConfig(PlayerControllerB controller)
        {
            List<string> tokens = new List<string>();
            ulong ourID = controller.OwnerClientId;
            string ourProfile = ProfileHelper.GetProfileName(ourID);
            var profile = ProfileHelper.TouchPlayerProfile(ourProfile);
            SavedProfileHelper.UpdatePlayerProfile(ourProfile);
            tokens.Add(ourID.ToString());
            tokens.Add("=");
            if (profile.GetData("OUTFIT") == "FALSE")
                tokens.Add(profile.GetData("MODEL",true));
            else
                tokens.Add(profile.Serialize());
            SyncDataKit.SendChatData(string.Join(null, tokens), CACKLECREW_TAG);
        }
        public static void ReceivePlayerConfig(string chatData)
        {
            int seperator_index = chatData.IndexOf('=');
            if (seperator_index == -1)
            {
                Debug.LogWarning("Chat Data Received is Wrong...!");
                Debug.LogWarning("Ignoring Chat Data...");
                return;
            }
            if (!ulong.TryParse(chatData.Substring(0, seperator_index), out ulong ownerClientID))
            {
                Debug.LogWarning("Unable to Parge ID from chat data...!");
                return;
            }
            if (ProfileHelper.IsLocalPlayer(ownerClientID))
            {
                //Debug.LogWarning("Config Is Local, Ignoring...");
                return;
            }
            Profile profile = ProfileHelper.TouchPlayerProfile(ownerClientID, out var player);
            if (profile == null)
            {
                Debug.LogWarning("Config Profile Does not exist!");
                return;
            }
            var data = chatData.Substring(++seperator_index);
            if (data.Length == 1)
            {
                profile.SetData("OUTFIT", "FALSE");
                profile.SetData("MODEL", data, true);
            }
            else
                profile.Deserialize(data);
            SuitKit.SwitchSuitOfPlayer(player, player.currentSuitID);
            ModelReplacement.ModelReplacementAPI.ResetPlayerModelReplacement(player);
        }
    }
    [HarmonyPatch]
    static class SyncManager_Patches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void ConnectClientToPlayerObject_Postfix(PlayerControllerB __instance)
        {
            ModelReplacement.ModelReplacementAPI.ResetPlayerModelReplacement(__instance);
            SyncManager.SyncPlayerConfig(__instance);
        }
        [HarmonyPatch(typeof(StartOfRound), "SyncShipUnlockablesClientRpc")]
        [HarmonyPostfix]
        private static void SyncShipUnlockablesClientRpc_Postfix(StartOfRound __instance)
        {
            if (ProfileHelper.IsServerHost() || !ProfileHelper.TryGetLocalPlayer(out var controller))
                return;
            SyncManager.SyncPlayerConfig(controller);
        }
        [HarmonyPatch(typeof(StartOfRound), "SyncShipUnlockablesServerRpc")]
        [HarmonyPostfix]
        private static void SyncShipUnlockablesServerRpc_Postfix(StartOfRound __instance)
        {
            if (!ProfileHelper.TryGetLocalPlayer(out var controller))
                return;
            SyncManager.SyncPlayerConfig(controller);
        }
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void Start_Postfix(ref StartOfRound __instance)
        {
            SyncDataKit.AttachListener(SyncManager.CACKLECREW_TAG, SyncManager.ReceivePlayerConfig);
        }
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy_Postfix(ref StartOfRound __instance)
        {
            SyncDataKit.RemoveListener(SyncManager.CACKLECREW_TAG, SyncManager.ReceivePlayerConfig);
        }
    }
}
