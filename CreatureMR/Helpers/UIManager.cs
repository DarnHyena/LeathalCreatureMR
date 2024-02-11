using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.IO;
using CackleCrew.ThisIsMagical;
using HarmonyLib;
using CreatureModelReplacement;

namespace CackleCrew.UI
{
    static class UIManager
    {
        public static CackleCrewUIHandler cacklecrewUI;
        public static CackleCrewUITogglerHandler cacklecrewtogglerUI;
        public static QuickMenuManager currentMenuManager;
        public static void Init(QuickMenuManager quickMenuManager)
        {
            currentMenuManager = quickMenuManager;
            if (cacklecrewUI == null)
            {
                var cacklecrewUIPrefab = Assets.CustomizationAssetBundle.LoadAsset<GameObject>("CCUIProfile");
                var cacklecrewUIGameObject = GameObject.Instantiate(cacklecrewUIPrefab, quickMenuManager.menuContainer.transform);
                cacklecrewUI = cacklecrewUIGameObject.AddComponent<CackleCrewUIHandler>();
            }
            if (cacklecrewtogglerUI == null)
            {
                var cacklecrewtogglerUIPrefab = Assets.CustomizationAssetBundle.LoadAsset<GameObject>("CCToggle");
                var cacklecrewtogglerUIGameObject = GameObject.Instantiate(cacklecrewtogglerUIPrefab, quickMenuManager.menuContainer.transform);
                cacklecrewtogglerUI = cacklecrewtogglerUIGameObject.AddComponent<CackleCrewUITogglerHandler>();
                cacklecrewtogglerUI.GetComponent<Button>().onClick.AddListener(ToggleCrewUIButtonClicked);
            }
        }
        public static void ToggleCrewUI(bool open)
        {
            if (currentMenuManager != null)
            {
                if (open)
                {
                    currentMenuManager.mainButtonsPanel.SetActive(false);
                    currentMenuManager.EnableUIPanel(cacklecrewUI.gameObject);
                    cacklecrewUI.UpdateProfileOptions();
                }
                else
                {
                    currentMenuManager.mainButtonsPanel.SetActive(true);
                    currentMenuManager.DisableUIPanel(cacklecrewUI.gameObject);
                    cacklecrewUI.ApplyProfileOptions();
                    ProfileHelper.ForceSyncUpdate();
                }
            }
        }
        public static void ToggleCrewUIButton(bool open)
        {
            if (currentMenuManager != null && cacklecrewtogglerUI != null)
            {
                cacklecrewtogglerUI.gameObject.SetActive(open);
            }
        }
        public static void ToggleCrewUIButtonClicked()
        {
            ToggleCrewUIButton(false);
            ToggleCrewUI(true);
        }
        public static void CrewUIExitButtonClicked()
        {
            ToggleCrewUI(false);
            ToggleCrewUIButton(true);
        }
    }
    [HarmonyPatch]
    static class UIManagerPatches
    {
        [HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
        [HarmonyPostfix]
        public static void OpenQuickMenu_Postfix_Patch(QuickMenuManager __instance)
        {
            UIManager.Init(__instance);
            UIManager.ToggleCrewUI(false);
        }
        [HarmonyPatch(typeof(QuickMenuManager), "EnableUIPanel")]
        [HarmonyPostfix]
        public static void EnableUIPanel_Postfix_Patch(QuickMenuManager __instance, GameObject enablePanel)
        {
            if (enablePanel == __instance.mainButtonsPanel)
            {
                UIManager.ToggleCrewUIButton(true);
            }
            else
            {
                UIManager.ToggleCrewUIButton(false);
            }
        }
        [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenuPanels")]
        [HarmonyPostfix]
        public static void CloseQuickMenuPanels_Postfix_Patch(QuickMenuManager __instance)
        {
            UIManager.ToggleCrewUIButton(true);
        }
        [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
        [HarmonyPostfix]
        public static void CloseQuickMenu_Postfix_Patch(QuickMenuManager __instance)
        {
            UIManager.Init(__instance);
            UIManager.ToggleCrewUI(false);
        }
    }
}
