using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //A bunch of utility functions to help handle suits in the game. Including extracting suit colors!
    //Borrowed code from Company Issues Protogen for this, but improved on a few things.
    static class SuitKit
    {
        private static Dictionary<int, int> LastSuits = new Dictionary<int, int>();
        private static Dictionary<int, int> CurrentSuits = new Dictionary<int, int>();
        public static string GetSuitName(int suitID)
        {
            return StartOfRound.Instance.unlockablesList.unlockables[suitID].unlockableName;
        }
        public static int GetSuitID(string suitName)
        {
            var suits = StartOfRound.Instance.unlockablesList.unlockables;
            for (int i = 0; i < suits.Count; i++)
            {
                var suit = suits[i];
                if (suit.unlockableName == suitName)
                {
                    return i;
                }
            }
            return 0;
        }
        public static Material GetSuitMaterial(int suitID)
        {
            Material material = null;
            if (suitID >= 0 && suitID < StartOfRound.Instance.unlockablesList.unlockables.Count)
            {
                material = StartOfRound.Instance.unlockablesList.unlockables[suitID].suitMaterial;
            }
            return material;
        }
        public static Material GetSuitMaterial(string suitName)
        {
            return GetSuitMaterial(GetSuitID(suitName));
        }
        public static int GetCurrentSuitID(PlayerControllerB player)
        {
            if (!StartOfRound.Instance.ClientPlayerList.TryGetValue(player.OwnerClientId, out var clientID))
                return 0;
            CurrentSuits.TryGetValue(clientID, out var suitID);
            return suitID;
        }
        public static int GetLastSuitID(PlayerControllerB player)
        {
            if (!StartOfRound.Instance.ClientPlayerList.TryGetValue(player.OwnerClientId, out var clientID))
                return 0;
            LastSuits.TryGetValue(clientID, out var suitID);
            return suitID;
        }
        public static string GetCurrentSuitName(PlayerControllerB player)
        {
            return GetSuitName(GetCurrentSuitID(player));
        }
        public static string GetLastSuitName(PlayerControllerB player)
        {
            return GetSuitName(GetLastSuitID(player));
        }
        public static void SetSuit(PlayerControllerB player, string suitName)
        {
            SetSuit(player, GetSuitID(suitName));
        }
        public static void SetSuit(PlayerControllerB player, int suitID)
        {
            if (!StartOfRound.Instance.ClientPlayerList.TryGetValue(player.OwnerClientId, out var clientID))
                return;
            if (CurrentSuits.ContainsKey(clientID))
            {
                if (suitID != CurrentSuits[clientID])
                {
                    LastSuits[clientID] = CurrentSuits[clientID];
                    CurrentSuits[clientID] = suitID;
                }
            }
            else
            {
                LastSuits.Add(clientID, 0);
                CurrentSuits.Add(clientID, suitID);
            }
        }
        public static void ClearSuits()
        {
            CurrentSuits.Clear();
            LastSuits.Clear();
        }

        public static void SwitchSuitOfPlayer(PlayerControllerB controller, int suitID)
        {
            if (!StartOfRound.Instance.SpawnedShipUnlockables.TryGetValue(suitID, out var suitObj))
            {
                Debug.LogWarning($"SUIT ID : {suitID} DOES NOT EXIST!!! Is it not registered with the Unlockables?!");
                Debug.LogWarning($"Suits need to be registered to utilize the built in client RPC functionality.");
                return;
            }
            var suit = suitObj.GetComponent<UnlockableSuit>();
            UnlockableSuit.SwitchSuitForPlayer(controller, suitID, false);
            suit.SwitchSuitServerRpc((int)controller.OwnerClientId);
        }
        public static void SampleSuitColors(Texture2D suitTexture, out Color bootColor, out Color suitColor, out Color clothColor, out Color tankColor)
        {
            bootColor = Color.black;
            suitColor = Color.white;
            clothColor = Color.gray;
            tankColor = Color.yellow;
            try
            {
                int width = suitTexture.width;
                int height = suitTexture.height;
                bootColor = suitTexture.GetPixel((int)(width * 0.597259f), (int)(height * 0.365158f));
                suitColor = suitTexture.GetPixel((int)(width * 0.62027f), (int)(height * 0.365158f));
                clothColor = suitTexture.GetPixel((int)(width * 0.685685f), (int)(height * 0.365158f));
                tankColor = suitTexture.GetPixel((int)(width * 0.026545f), (int)(height * 0.916508f));
            }
            catch (Exception)
            {
                Debug.LogWarning($"Texture \"{suitTexture.name}\" is not Read/Writtable colors could not be extracted.");
            }
        }
    }
    [HarmonyPatch]
    static class SuitKit_Patches
    {
        [HarmonyPatch(typeof(UnlockableSuit), "SwitchSuitForPlayer")]
        [HarmonyPrefix]
        public static void SwitchSuitForPlayer_Postfix(PlayerControllerB player, int suitID, bool playAudio = true)
        {
            SuitKit.SetSuit(player, suitID);
        }
        [HarmonyPatch(typeof(StartOfRound), "OnDestroy")]
        [HarmonyPrefix]
        public static void OnDestoy_Postfix(ref StartOfRound __instance)
        {
            SuitKit.ClearSuits();
        }
    }
}
