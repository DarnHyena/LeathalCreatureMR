using CackleCrew.ThisIsMagical;
using UnityEngine;
using UnityEngine.UI;
namespace CackleCrew.UI
{
    class ClipboardUIHandler : MonoBehaviour
    {
        public CackleCrewUIHandler handler;
        public Button pasteButton;
        public Button copyButton;
        public void Init()
        {
            pasteButton = transform.Find("Load").GetComponentInChildren<Button>();
            copyButton = transform.Find("Copy").GetComponentInChildren<Button>();
            pasteButton.onClick.AddListener(LoadProfileFromClipboard);
            copyButton.onClick.AddListener(CopyProfileToClipboard);
        }

        void LoadProfileFromClipboard()
        {
            string clipboard = FromClipboard();
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            if (!ProfileKit.TryGetProfile(ourProfile, out _))
            {
                ProfileKit.CloneProfile("DEFAULT:Config", ourProfile);
            }
            ProfileKit.DeSerializeProfile_Tokens(ourProfile,clipboard);
            handler.UpdateProfileOptions();
        }
        void CopyProfileToClipboard()
        {
            var controller = StartOfRound.Instance.localPlayerController;
            string ourProfile = $"{controller.OwnerClientId}:Config";
            ProfileHelper.TouchPlayerProfile(ourProfile);
            handler.ApplyProfileOptions();
            string clipboard = ProfileKit.SerializeProfile_Tokens(ourProfile);
            ToClipboard(clipboard);
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
