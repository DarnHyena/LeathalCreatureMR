using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //Very Simple Profile System, Actually helps quite alot to manage customization.
    //
    //Interface
    static partial class ProfileKit
    {
        private static Dictionary<string, Dictionary<string, string>> _Profiles = new Dictionary<string, Dictionary<string, string>>();
        public static void SetData(string profileName, string name, string data)
        {
            if (!_Profiles.TryGetValue(profileName, out var profile))
            {
                profile = new Dictionary<string, string>();
                _Profiles.Add(profileName, profile);
            }
            if (profile.ContainsKey(name))
            {
                profile.Remove(name);
            }
            profile.Add(name, data);
        }
        public static string GetData(string profileName, string name)
        {
            if (_Profiles.TryGetValue(profileName, out var profile))
            {
                profile.TryGetValue(name, out var data);
                return data;
            }
            return string.Empty;
        }
        public static bool TryGetData(string profileName, string name, out string data)
        {
            if (_Profiles.TryGetValue(profileName, out var profile))
            {
                profile.TryGetValue(name, out var foundData);
                data = foundData;
                return true;
            }
            data = string.Empty;
            return false;
        }
        public static bool HasData(string profileName, string name)
        {
            if (TryGetProfile(profileName, out var profile))
            {
                return profile.ContainsKey(name);
            }
            return false;
        }
        public static void ClearData(string profileName, string name)
        {
            if (_Profiles.TryGetValue(profileName, out var profile))
            {
                if (profile.ContainsKey(name))
                {
                    profile.Remove(name);
                }
            }
        }
        public static Dictionary<string, string> GetProfile(string profileName)
        {
            _Profiles.TryGetValue(profileName, out var profile);
            return profile;
        }
        public static bool TryGetProfile(string profileName, out Dictionary<string, string> profile)
        {
            return _Profiles.TryGetValue(profileName, out profile);
        }
        public static void CloneProfile(string profileName, string newProfileName)
        {
            if (TryGetProfile(profileName, out var profile))
            {
                Dictionary<string, string> newProfile = new Dictionary<string, string>(profile.Count, profile.Comparer);
                foreach (KeyValuePair<string, string> entry in profile)
                {
                    newProfile.Add(entry.Key, (string)entry.Value.Clone());
                }
                if (_Profiles.ContainsKey(newProfileName))
                {
                    _Profiles.Remove(newProfileName);
                }
                _Profiles.Add(newProfileName, newProfile);
            }
        }
        public static void CopyProfile(string profileName, string referenceProfileName)
        {
            if (TryGetProfile(referenceProfileName, out var ref_profile))
            {
                if (TryGetProfile(profileName, out var profile))
                {
                    foreach (var entry in ref_profile)
                    {
                        if (profile.ContainsKey(entry.Key))
                        {
                            profile.Remove(entry.Key);
                        }
                        profile.Add(entry.Key, (string)entry.Value.Clone());
                    }
                }
            }
        }
        public static void ReflectProfile(string profileName, string referenceProfileName)
        {
            if (TryGetProfile(referenceProfileName, out var ref_profile))
            {
                if (TryGetProfile(profileName, out var profile))
                {
                    foreach (var entry in ref_profile)
                    {
                        if (profile.ContainsKey(entry.Key))
                        {
                            profile.Remove(entry.Key);
                            profile.Add(entry.Key, (string)entry.Value.Clone());
                        }
                    }
                }
            }
        }
        public static void ClearProfile(string profileName)
        {
            if (_Profiles.ContainsKey(profileName))
            {
                _Profiles.Remove(profileName);
            }
        }
        public static void ClearAllProfiles(string profileName)
        {
            foreach (var profile in _Profiles)
            {
                profile.Value.Clear();
            }
            _Profiles.Clear();
        }
        public static void ClearAllProfilesFiltered(string profileName, string ignore)
        {
            Queue<string> profilesToRemove = new Queue<string>();
            foreach (var profile in _Profiles)
            {
                if (profile.Key.Contains(profileName) && (string.IsNullOrWhiteSpace(ignore) || !profile.Key.Contains(ignore)))
                {
                    profile.Value.Clear();
                    profilesToRemove.Enqueue(profile.Key);
                }
            }
            while (profilesToRemove.Count > 0)
            {
                _Profiles.Remove(profilesToRemove.Dequeue());
            }
        }
    }
    //Serialization JSON
    static partial class ProfileKit
    {
        public static string SerializeProfile_JSON(string profileName)
        {
            if (!_Profiles.TryGetValue(profileName, out var profile))
            {
                return string.Empty;
            }
            //Store our data names.
            string jsonString = profileName + JsonUtility.ToJson(profile.Keys.ToArray(), false);
            foreach (var data in profile.Values)
            {
                var dataJson = JsonUtility.ToJson(data, false);
                if (!string.IsNullOrWhiteSpace(dataJson) || data == null)
                    jsonString += $";{dataJson};{data.GetType().FullName}";
                else
                    jsonString += ";;";
            }
            return jsonString;
        }
        public static void DeSerializeProfile_JSON(string jsonString)
        {
            string[] tokens = jsonString.Split(';');
            if (tokens.Length < 2)
            {
                Debug.LogWarning($"PROFILE JSON DATA IS INVALID");
                return;
            }
            string profileName = tokens[0];
            string[] names = JsonUtility.FromJson<string[]>(tokens[1]);
            if (names == null || names.Length == 0)
            {
                Debug.LogWarning($"PROFILE JSON DATA {profileName} does not contain any names.");
            }
            for (int i = 2; i < names.Length; i += 2)
            {
                string jsonDataString = tokens[i];
                string jsonType = tokens[i + 1];
                string name = names[i];
                if (string.IsNullOrWhiteSpace(jsonType))
                {
                    SetData(profileName, name, null);
                }
                else
                {
                    SetData(profileName, names[i], (string)JsonUtility.FromJson(jsonDataString, typeof(string)));
                }
            }
        }
    }
    //Serialization COMPRESSED TOKENS
    static partial class ProfileKit
    {
        public static string SerializeProfile_Tokens(string profileName)
        {
            if (!_Profiles.TryGetValue(profileName, out var profile) || profile.Count == 0)
            {
                return string.Empty;
            }
            //Store our data names.
            string[] builder = new string[profile.Count * 2];
            int i = 0;
            foreach (var data in profile)
            {
                builder[i] = data.Key;
                builder[i + 1] = data.Value;
                i += 2;
            }
            string combined = string.Join(";", builder);
            string compressed = $"LCpf{CompressString(combined)}";
            return compressed;
        }
        public static void DeSerializeProfile_Tokens(string profile, string tokenString)
        {
            try
            {
                if (!tokenString.StartsWith("LCpf"))
                    return;
                string decompressed = DecompressString(tokenString.Substring(4));
                string[] split = decompressed.Split(';');
                for (int i = 0; i < split.Length; i += 2)
                {
                    SetData(profile, split[i], split[i + 1]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Provided tokenString could not be Deserialized or Read.");
                Debug.LogError(e);
            }
        }
    }
    //Compression;
    static partial class ProfileKit
    {
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

}
