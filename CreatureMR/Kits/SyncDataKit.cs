using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
namespace CackleCrew.ThisIsMagical
{
    //The SyncDataKit is a utility class to handle sending data via the chat.
    //Alternatively we could utilize custom RPCs, but for now this remains vanilla even with a small side-effect --
    //of having vanilla clients see the data directly in their own chats.
    //One issue with this approach is that the chat has a natural maximun character limit.
    //the Ideal solution is to create a constitent data-table that is catalogued by a unique Index.
    //This way we can just send smaller hexidecible based data between clients, removing the need to send data--
    //through the server-chat channel.

    //Interface
    static partial class SyncDataKit
    {
        private static Dictionary<string, Action<string[]>> listeners = new Dictionary<string, Action<string[]>>();
        private const bool AlwaysCompress = true;
        public static void SendChatData(string[] tokens)
        {
            string chatMessage = DeTokenizeChatData(tokens);
            HUDManager.Instance.AddTextToChatOnServer(chatMessage, -1);
        }
        public static void HandleChatData(string chatMessage)
        {
            var tokens = TokenizeChatData(chatMessage);
            if (listeners.TryGetValue(tokens[0], out var action))
            {
                action(tokens);
            }
        }
        public static bool ContainsChatData(string chatMessage)
        {
            return chatMessage.StartsWith(ChatDataTag) || chatMessage.StartsWith(ChatDataTag_Compressed);
        }
        public static void AttachListener(string name, Action<string[]> action)
        {
            RemoveListener(name);
            listeners.Add(name, action);
        }
        public static void RemoveListener(string name)
        {
            if (listeners.ContainsKey(name))
            {
                listeners.Remove(name);
            }
        }
    }
    //Tokenization
    static partial class SyncDataKit
    {
        private const string ChatDataTag = "LCCD";
        private const string ChatDataTag_Compressed = "LCCDc";
        //We convert a string into a array of "tokens"
        private static string[] TokenizeChatData(string chatMessage)
        {
            //Check for Compression
            bool compression = false;
            if (chatMessage.StartsWith(ChatDataTag_Compressed))
            {
                compression = true;
                chatMessage = chatMessage.Substring(ChatDataTag_Compressed.Length);
            }
            else if (chatMessage.StartsWith(ChatDataTag))
            {
                chatMessage = chatMessage.Substring(ChatDataTag.Length);
            }
            else
            {
                return null;
            }
            string[] tokens;
            //Decompress
            if (compression)
            {
                tokens = DecompressString(chatMessage).Split(';');
            }
            else
            {
                tokens = chatMessage.Split(';');
            }
            //Time To Unsantize!
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = UnSanitizeInput(tokens[i]);
            }
            return tokens;
        }
        //We Convert Tokens into a usable string.
        private static string DeTokenizeChatData(string[] chatData)
        {
            string[] tokens = chatData;
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = SanitizeInput(tokens[i]);
            }
            string chatMessage = string.Join(";", tokens);
            if (AlwaysCompress || IsEligibleForCompression(chatMessage))
            {
                chatMessage = CompressString(chatMessage);
                chatMessage = $"{ChatDataTag_Compressed}{chatMessage}";
            }
            else
            {
                chatMessage = $"{ChatDataTag}{chatMessage}";
            }
            return chatMessage;
        }
        //Here we can Un-Sanitize the Input, For now we just use the direct text.
        private static string UnSanitizeInput(string input)
        {
            return input;
        }
        //Here we can Sanitize the Input, For now we just send the direct text.
        private static string SanitizeInput(string input)
        {
            return input;
        }
    }
    //Compression
    static partial class SyncDataKit
    {
        //A Small Check if the data is large enough to even be eligible for compression,
        //might be irrelavent in some cases, and maybe a little performance heavy..
        //will figure out a better method.
        private static bool IsEligibleForCompression(string input)
        {
            int minLength = 3;
            int maxLength = input.Length / 2;
            for (int length = minLength; length <= maxLength; length++)
            {
                for (int i = 0; i <= input.Length - 2 * length; i++)
                {
                    string pattern = input.Substring(i, length);
                    string restOfString = input.Substring(i + length);
                    if (restOfString.Contains(pattern))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //WIP, ideally we want to be able to provide a sort of predetermined table of name-values to send the smallest amount of data
        //But for now we'll only going to compress utilizing gzip+Base64
        private static string CompressString(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(byteArray, 0, byteArray.Length);
                }
                return Convert.ToBase64String(outputStream.ToArray());
            }
        }
        private static string DecompressString(string input)
        {
            byte[] compressedBytes = Convert.FromBase64String(input);

            using (MemoryStream inputStream = new MemoryStream(compressedBytes))
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputStream);
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }
    }

    [HarmonyPatch]
    static class ChatDataKitPatches
    {
        //Base game handles chat much in this way, only checking the last-message received to avoid repeats.
        static string lastMessage;
        private static void HandleChatDataMessage(string chatMessage)
        {
            if (lastMessage != chatMessage)
            {
                lastMessage = chatMessage;
                SyncDataKit.HandleChatData(chatMessage);
            }
        }
        //AddPlayerChatMessageClientRpc Patch
        //Check if valid Chat Data Message
        [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageClientRpc")]
        [HarmonyPrefix]
        public static bool AddPlayerChatMessageClientRpc_Prefix_Patch(HUDManager __instance, string chatMessage, int playerId)
        {
            if (!SyncDataKit.ContainsChatData(chatMessage))
                return true;
            HandleChatDataMessage(chatMessage);
            return false;
        }
        //AddTextToChatOnServer Patch
        //Make sure a client doesn't send data directly.
        [HarmonyPatch(typeof(HUDManager), "AddTextToChatOnServer")]
        [HarmonyPrefix]
        public static bool AddTextToChatOnServer(HUDManager __instance, string chatMessage, int playerId = -1)
        {
            if (SyncDataKit.ContainsChatData(chatMessage) && (playerId != -1 && playerId != 99 && playerId != 49362))
            {
                return false;
            }
            return true;
        }
        //AddChatMessage Patch
        //Check if valid Chat Data Message
        [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
        [HarmonyPrefix]
        public static bool AddChatMessage_Prefix_Patch(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped = "")
        {
            if (!SyncDataKit.ContainsChatData(chatMessage))
                return true;
            HandleChatDataMessage(chatMessage);
            return false;
        }
    }
}