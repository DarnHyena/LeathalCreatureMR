using GameNetcodeStuff;
using HarmonyLib;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms;

namespace CackleCrew.ThisIsMagical
{
    static class ProfileHelper
    {
        public const string DEFAULT_PROFILE_ID = "DEFAULT";
        public const string PROFILE_POSTFIX = ":Config";
        public static string DefaultProfile
        {
            get
            {
                return DEFAULT_PROFILE_ID + PROFILE_POSTFIX;
            }
        }
        public static string LocalProfile
        {
            get
            {
                return GetProfileName(TryGetLocalPlayer(out var player) ? 999999 : player.OwnerClientId);
            }
        }
        public static string GetProfileName(ulong ownerClientID)
        {
            return ownerClientID + PROFILE_POSTFIX;
        }
        public static void ForceSyncUpdate()
        {
            if (TouchLocalPlayerProfile(out var player) == null)
                return;
            ModelReplacement.ModelReplacementAPI.ResetPlayerModelReplacement(player);
            SyncManager.SyncPlayerConfig(player);
        }
        public static Profile TouchPlayerProfile(ulong ownerClientID, out PlayerControllerB player)
        {
            if (!TryGetPlayer(ownerClientID, out player))
                return null;
            string profileName = GetProfileName(ownerClientID);
            if (!Profile.TryGetProfile(profileName, out var profile))
                profile = Profile.CloneProfile(profileName, DefaultProfile);
            return profile;
        }
        public static Profile TouchPlayerProfile(string profileName)
        {
            if (!Profile.TryGetProfile(profileName, out var profile))
                profile = Profile.CloneProfile(profileName, DefaultProfile);
            return profile;
        }
        public static Profile TouchLocalPlayerProfile(out PlayerControllerB player)
        {
            if (!TryGetLocalPlayer(out player))
                return null;
            string profileName = GetProfileName(player.OwnerClientId);
            return TouchPlayerProfile(profileName);
        }
        public static PlayerControllerB GetLocalPlayer()
        {
            return StartOfRound.Instance?.localPlayerController;
        }
        public static bool TryGetLocalPlayer(out PlayerControllerB player)
        {
            player = StartOfRound.Instance?.localPlayerController;
            return player != null;
        }
        public static PlayerControllerB GetPlayer(ulong ownerClientID)
        {
            if (!StartOfRound.Instance.ClientPlayerList.TryGetValue(ownerClientID, out var controller_index))
                return null;
            return StartOfRound.Instance.allPlayerObjects[controller_index].GetComponent<PlayerControllerB>();
        }
        public static bool TryGetPlayer(ulong ownerClientID, out PlayerControllerB player)
        {
            player = GetPlayer(ownerClientID);
            return player != null;
        }
        public static bool IsLocalPlayer(ulong ownerClientID)
        {
            if (StartOfRound.Instance.localPlayerController != null)
            {
                return StartOfRound.Instance.localPlayerController.OwnerClientId == ownerClientID;
            }
            return StartOfRound.Instance.NetworkObjectId == ownerClientID;
        }
        public static bool IsLocalPlayer(PlayerControllerB player)
        {
            return IsLocalPlayer(player.OwnerClientId);
        }
        public static bool IsServerHost()
        {
            NetworkManager networkManager = HUDManager.Instance.NetworkManager;
            return networkManager.IsServer || networkManager.IsHost;
        }
    }
}
