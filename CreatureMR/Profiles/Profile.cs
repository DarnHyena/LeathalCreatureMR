using System.Collections.Generic;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //The New Profile Class!
    //Utilizes a Data class to handle internal functionality,
    //Especially when serializing and deserializing.
    //In this way we can make it easier to implement and modify how data gets stored.
    //Atleast with this new interface i can make future changes 
    //without drastically affecting how The profile class is used.
    //
    //TODO Consider if the Profile static accessiblity should be changed to be a class instead :
    //for example, "ProfileManager", and have that be handled manually instead.
    //
    //Public Static Functionality//
    public partial class Profile
    {
        private static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
        //static CreateProfile(profileName)
        //
        //Create a new Profile!
        public static Profile CreateProfile(string profileName)
        {
            if (TryGetProfile(profileName, out var profile))
                return profile;
            profile = new Profile();
            profiles.Add(profileName, profile);
            return profile;
        }
        //static GetProfile(profileName)
        //
        //Gets a Profile!
        public static Profile GetProfile(string profileName)
        {
            TryGetProfile(profileName, out var profile);
            return profile;
        }
        //static TryGetProfile(profileName, out profile)
        //
        //Allows you to get a Profile and check if it exists
        public static bool TryGetProfile(string profileName, out Profile profile)
        {
            return profiles.TryGetValue(profileName, out profile);
        }
        //static HasProfile(profileName)
        //
        //Simple check if Profile exists.
        public static bool HasProfile(string profileName)
        {
            return profiles.ContainsKey(profileName);
        }
        //static CloneProfile(profileName, reference)
        //
        //Allows you to 'clone' data to and from profiles.
        //If the profile provided doesn't exist, it creates a new one before--
        //Copying data from the referenced profile.
        public static Profile CloneProfile(string profileName, string reference)
        {
            if (!TryGetProfile(reference, out var referenceProfile))
            {
                Debug.LogWarning($"Profile.CloneProfile does not contain the reference profile {reference}");
                Debug.LogWarning($"Profile.CloneProfile could not clone to profile {profileName}");
                return null;
            }
            if (!TryGetProfile(profileName, out var profile))
                profile = CreateProfile(profileName);
            foreach (var pair in referenceProfile.Data)
            {
                profile.SetData(pair.Key, pair.Value.Data, true, pair.Value.Type);
            }
            return profile;
        }
        //static ReflectProfile(profileName,reference)
        //
        //Allows you to 'reflect' data to and from existing profiles.
        //Essentially copying data names that exist from the reference to the profile provided.
        public static Profile ReflectProfile(string profileName, string reference)
        {
            if (!TryGetProfile(reference, out var referenceProfile))
            {
                Debug.LogWarning($"Profile.ReflectProfile does not contain the reference profile {reference}");
                Debug.LogWarning($"Profile.ReflectProfile could not reflect to profile {profileName}");
                return null;
            }
            if (!TryGetProfile(profileName, out var profile))
            {
                Debug.LogWarning($"Profile.ReflectProfile does not contain the profile {reference}");
                Debug.LogWarning($"Profile.ReflectProfile could not reflect to profile {profileName}");
                return null;
            }
            foreach (var pair in profile.Data)
            {
                profile.SetData(pair.Key, referenceProfile.GetData(pair.Key, true), true);
            }
            return profile;
        }
        //static DeleteProfile(profileName)
        //
        //Deletes a profile...!
        public static void DeleteProfile(string profileName)
        {
            if (!profiles.TryGetValue(profileName, out var profile))
                return;
            profile.PurgeData();
            profiles.Remove(profileName);
        }
        //static DeleteProfile(keywords)
        //
        //Deletes profiles with the provided keywords.
        public static void FilterDeleteProfiles(string[] keywords)
        {
            Queue<string> profilesToRemove = new Queue<string>();
            foreach (var pair in profiles)
            {
                foreach (var keyword in keywords)
                {
                    if (pair.Key.Contains(keyword))
                    {
                        profilesToRemove.Enqueue(pair.Key);
                        break;
                    }
                }
            }
            while (profilesToRemove.TryDequeue(out var profile))
            {
                DeleteProfile(profile);
            }
        }
        //static DeleteProfile(ignoredKeywords,keywords)
        //
        //Deletes profiles with the provided keywords, ignoring the excluded keywords.
        public static void FilterDeleteProfiles(string[] ignoredKeywords, string[] keywords)
        {
            Queue<string> profilesToRemove = new Queue<string>();
            foreach (var pair in profiles)
            {
                bool ignore = false;
                foreach (var ignoredKeyword in ignoredKeywords)
                {
                    if (pair.Key.Contains(ignoredKeyword))
                    {
                        ignore = true;
                        break;
                    }
                }
                if (ignore)
                    continue;
                foreach (var keyword in keywords)
                {
                    if (pair.Key.Contains(keyword))
                    {
                        profilesToRemove.Enqueue(pair.Key);
                        break;
                    }
                }
            }
            while (profilesToRemove.TryDequeue(out var profile))
            {
                DeleteProfile(profile);
            }
        }
        //static PurgeProfiles(profileName)
        //
        //Purges all the current profiles, clearing them from the dictionary.
        public static void PurgeProfiles(string profileName)
        {
            foreach (var pair in profiles)
            {
                pair.Value.PurgeData();
            }
            profiles.Clear();
        }
    }
    //The Actual Profile Class//
    public partial class Profile
    {
        private Dictionary<string, ProfileData> _data = new Dictionary<string, ProfileData>();
        private List<string> _order = new List<string>();
        public int Count { get { return _order.Count; } }
        public Dictionary<string, ProfileData> Data { get { return _data; } }
        //SetData(key,value,isRaw,type)
        //
        //(isRaw) let's you set the internal value directly.
        //Otherwise it'll automatically get converted to it's 'serialized' counterpart.
        //
        //(type) controls how the data gets encoded/decoded internally.
        //Otherwise it'll be handled as a generic string.
        public void SetData(string key, string value, bool isRaw = false, ProfileDataType type = ProfileDataType.Generic)
        {
            if (!_data.TryGetValue(key, out var data))
            {
                data = new ProfileData()
                {
                    Type = type
                };
                _data.Add(key, data);
                _order.Add(key);
            }
            if (isRaw)
                data.Data = value;
            else
                data.DecodeData(value);
        }
        //GetData(key,isRaw)
        //
        //(isRaw) let's you get the internal value directly.
        public string GetData(string key, bool isRaw = false)
        {
            TryGetData(key, out var value, isRaw);
            return value;
        }
        //TryGetData(key,out value,isRaw)
        //
        //(isRaw) let's you get the internal value directly.
        //
        //This function is especially useful when you want to grab data
        //directly in addition to checking if it exists!
        public bool TryGetData(string key, out string value, bool isRaw = false)
        {
            value = string.Empty;
            if (_data.TryGetValue(key, out var data))
            {
                if (isRaw)
                    value = data.Data;
                else
                    value = data.EncodeData();
            }
            return string.IsNullOrEmpty(value);
        }
        //HasData
        //
        //Simple Check to see if data exists.
        public bool HasData(string key)
        {
            return _data.ContainsKey(key);
        }
        //DeleteData(key)
        //
        //Deletes data from profile
        public void DeleteData(string key)
        {
            if (!HasData(key))
                return;
            _data[key] = null;
            _data.Remove(key);
            _order.Remove(key);
        }
        //PurgeData
        //
        //Removes all the data in profile.
        public void PurgeData()
        {
            _data.Clear();
            _order.Clear();
        }
        //SetDataIndex(key,index)
        //
        //Allows for more control over the position of how data is serialized.
        public void SetDataIndex(string key, int index)
        {
            if (!_order.Contains(key))
                return;
            _order.Remove(key);
            _order.Insert(index, key);
        }
        //GetDataIndex(key)
        //
        //Provides more context to the index of data.
        public int GetDataIndex(string key)
        {
            return _order.IndexOf(key);
        }
        //GetDataSize(key)
        //
        //Provides more context to the size of data.
        public int GetDataSize(string key)
        {
            if (_data.TryGetValue(key, out var data))
                return data.Size;
            return -1;
        }
    }
    //Profile Serialization//
    //
    //The most important part about this Profile Update...!
    //allowing us to put all our data into a minimalized string.
    //eg: FFFFFF01A
    // Color Option Bool Character
    public partial class Profile
    {
        private const char GENERIC_TOKEN = ';';
        //Serialize()
        //
        //Serializes the current Profile into a workable string...!
        public string Serialize()
        {
            List<string> tokens = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                tokens.Add(string.Empty);
            }
            foreach (var pair in _data)
            {
                var data = pair.Value;
                var value = data.Data;
                //Generic Check, Add Special Token To End.
                if (data.Type == ProfileDataType.Generic)
                {
                    value += GENERIC_TOKEN;
                }
                tokens[GetDataIndex(pair.Key)] = value;
            }
            var newProfileData = string.Join(string.Empty, tokens);
            return GenerateChecksum(newProfileData) + newProfileData;
        }
        //Deserialize(profileData)
        //
        //Deserializes the provided profileData string!
        //!!!Keep in mind it will only load data into existing data in the profile.
        //Providing a different profile's code will result in unwanted behavior-->
        //Such as wrong data being read into the incorrect types.
        public void Deserialize(string profileData,bool validate = true)
        {
            //Validation if procided profile data is compatible.
            int expectedSize = 1;
            if(validate)
                foreach (var pair in _data)
                {
                    if (pair.Value.Type == ProfileDataType.Generic)
                        expectedSize++;
                    expectedSize += pair.Value.Size;
                }
            //TODO There seems to be some oversight, with Generic data possibly throwing a issue
            //Possibly ignored here, Possibly handle "generic" data differently,
            //by positioning it at the end of the profileData.
            if (validate && profileData.Length != expectedSize)
            {
                Debug.LogWarning("Profile data provided does not match expected size--");
                Debug.LogWarning("...Ignoring");
                return;
            }
            //Checksum Validation, Data may be incorrect if checksum token is incorrect.
            if (validate && !ValidateChecksum(profileData))
            {
                Debug.LogWarning("Profile data does not match checksum--");
                Debug.LogWarning("data may be incorrect... Ignoring");
                return;
            }
            //Prepare to read profile data.
            int pointer = 1;
            int size;
            foreach (var pair in _data)
            {
                size = pair.Value.Size;
                //Generic Data Check, Get Size To Index Of GENERIC_TOKEN
                if (pair.Value.Type == ProfileDataType.Generic)
                    size = profileData.IndexOf(GENERIC_TOKEN, pointer) - pointer;
                //Grab Our Token Starting From Pointer To Size
                var token = profileData.Substring(pointer, size);
                //Generic Data Check, Move Foward Past GENERIC_TOKEN
                if (pair.Value.Type == ProfileDataType.Generic)
                    pointer++;
                //No Need To Decode Serialized Data.
                pair.Value.Data = token;
                //Move Pointer Over...!
                pointer += size;
            }
        }
        //Compare the Checksum provided in the string, to it's actual checksum.
        private bool ValidateChecksum(string profileData)
        {
            var data_token = profileData[0];
            var token = GenerateChecksum(profileData, 1);
            return data_token == token;
        }
        //Hastily thrown together checksum, where we hash all the characters in a string...!
        private char GenerateChecksum(string profileData, int offset = 0)
        {
            int pointer = offset;
            var token = '#';
            while (pointer < profileData.Length)
            {
                char data = profileData[pointer];
                token ^= data;
                bool flip = (token % 2 == 0);
                token = (char)((token % ('Z' - 'A' + 1)) + 'A');
                token = (flip) ? char.ToUpper(token) : char.ToLower(token);
                pointer++;
            }
            return token;
        }
    }
}
