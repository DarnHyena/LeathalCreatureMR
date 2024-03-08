using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace CackleCrew.ThisIsMagical
{
    //The SyncDataKit is a utility class to handle sending data via the chat.
    //Changes made to just send chat data through with a invalid player id,
    //This is expected to throw a error in the vanilla Console,
    //This is done on purpose since attempting to access a player id outside of the
    //player array size will naturally throw an error.

    //Interface
    static partial class SyncDataKit
    {
        public const int DATAKIT_ID = 49362;
        //Possibly capsulize this into it's own helper class to handle it's own messages/listeners
        private static Dictionary<string, List<Action<string>>> listeners = new Dictionary<string, List<Action<string>>>();
        private static readonly MethodInfo _AddPlayerChatMessageServerRpc = AccessTools.Method(typeof(HUDManager), "AddPlayerChatMessageServerRpc");
        //TODO Handle Fragmented Data with a Queue...?
        public static void SendChatData(string chatMessage, string tag)
        {
            if (chatMessage.Length > (50 - tag.Length))
            {
                Debug.LogError("CHAT MESSAGE BEING SENT IS LONGER THAN 50 CHARACTERS!!!");
                Debug.LogError("AND WILL PROBABLY NOT BE RECEIVED BY THE OTHER CLIENTS!!!");
                Debug.LogError(tag + chatMessage);
            }
            Debug.LogWarning("== Ignore the Following Error ==");
            Debug.LogWarning("== IndexOutOfRangeException : AddPlayerChatMessageClientRpc ==");
            try
            {
                var data = new object[] { tag + chatMessage, DATAKIT_ID };
                _AddPlayerChatMessageServerRpc.Invoke(HUDManager.Instance, data);
            }
            catch (Exception _) { }
        }
        public static void HandleChatData(string chatMessage, string tag)
        {
            if (!listeners.TryGetValue(tag, out var action_list))
                return;
            foreach (var action in action_list)
            {
                action(chatMessage.Substring(tag.Length));
            }
        }
        public static int ValidateChatData(string chatMessage, out string tag)
        {
            tag = null;
            foreach (var pair in listeners)
            {
                if (chatMessage.StartsWith(pair.Key))
                {
                    tag = pair.Key;
                    return 1;
                }
            }
            //TODO Add in another flag to handle Fragmented Data...
            //An idea I had about fragmenting profile data, was to pass a index number, the starting index.
            //And implment the functionality in the profile class to read data specific to that index.
            //I Suppose it wouldn't really need to be provided a ending index if the profile class--
            //Handles reading the data lengths at index anyways.
            return 0;
        }
        public static void AttachListener(string tag, Action<string> action)
        {
            if (!listeners.TryGetValue(tag, out var action_list))
            {
                action_list = new List<Action<string>>();
                listeners.Add(tag, action_list);
            }
            if (!action_list.Contains(action))
                action_list.Add(action);
        }
        public static void RemoveListener(string tag, Action<string> action)
        {
            if (!listeners.TryGetValue(tag, out var action_list))
                return;
            action_list.Remove(action);
            if (action_list.Count == 0)
                listeners.Remove(tag);
        }
        public static void PurgeListeners()
        {
            listeners.Clear();
        }
    }

    [HarmonyPatch]
    static class ChatDataKitPatches
    {
        //Base game handles chat much in this way, only checking the last-message received to avoid repeats.
        private static string lastMessage;
        private static void HandleChatDataMessage(string chatMessage, string tag)
        {
            if (lastMessage != chatMessage)
            {
                lastMessage = chatMessage;
                SyncDataKit.HandleChatData(chatMessage, tag);
            }
        }
        //AddPlayerChatMessageClientRpc Patch
        //Check if valid Chat Data Message
        [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageClientRpc")]
        [HarmonyPrefix]
        public static bool AddPlayerChatMessageClientRpc_Prefix_Patch(HUDManager __instance, string chatMessage, int playerId)
        {
            if (SyncDataKit.ValidateChatData(chatMessage, out var tag) == 0)
            {
                return true;
            }
            HandleChatDataMessage(chatMessage, tag);
            return ProfileHelper.IsServerHost();
        }
        //AddTextToChatOnServer Patch
        //Make sure a client doesn't send data directly.
        [HarmonyPatch(typeof(HUDManager), "AddTextToChatOnServer")]
        [HarmonyPrefix]
        public static bool AddTextToChatOnServer(HUDManager __instance, string chatMessage, int playerId = -1)
        {
            if (SyncDataKit.ValidateChatData(chatMessage, out _) != 0 && playerId != SyncDataKit.DATAKIT_ID)
            {
                return false;
            }
            return true;
        }
    }
}