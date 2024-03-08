using System.Text.RegularExpressions;
using CackleCrew.ThisIsMagical;
using UnityEngine;
using UnityEngine.UI;
namespace CackleCrew.UI
{
    class ClipboardUIHandler : MonoBehaviour
    {
        public const string CLIPBOARD_PROFILE_PREFIX = "CAv1";
        public const string CLIPBOARD_PROFILE_PATTERN = @"^CAv1[a-zA-Z][A-C][01][\dA-F]{36}[0-4]{2}$";
        public CackleCrewUIHandler handler;
        public Button pasteButton;
        public Button copyButton;
        public void Init()
        {
            pasteButton = transform.Find("Paste").GetComponentInChildren<Button>();
            copyButton = transform.Find("Copy").GetComponentInChildren<Button>();
            pasteButton.onClick.AddListener(PasteProfileFromClipboard);
            copyButton.onClick.AddListener(CopyProfileToClipboard);
        }
        void PasteProfileFromClipboard()
        {
            string clipboard = FromClipboard();
            bool isOldProfileData = CompatibilityKit.IsOldProfileData(clipboard);
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            if (!ValidateProfileData(clipboard, out var profileData) && !isOldProfileData) return;
            if (isOldProfileData)
                CompatibilityKit.Deserialize_OldProfileData(profile, clipboard);
            else
                profile.Deserialize(profileData);
            handler.UpdateProfileOptions();
        }
        void CopyProfileToClipboard()
        {
            var profile = ProfileHelper.TouchLocalPlayerProfile(out var player);
            if (profile == null) return;
            handler.ApplyProfileOptions();
            string clipboard = profile.Serialize();
            ToClipboard(CLIPBOARD_PROFILE_PREFIX + clipboard);
        }
        bool ValidateProfileData(string input, out string profileData)
        {
            profileData = null;
            input = input.Trim();
            if (!Regex.IsMatch(input, CLIPBOARD_PROFILE_PATTERN))
                return false;
            profileData = input.Substring(CLIPBOARD_PROFILE_PREFIX.Length);
            return true;
        }
        string FromClipboard()
        {
            return GUIUtility.systemCopyBuffer;
        }
        void ToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}
