using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CackleCrew.ThisIsMagical;
using UnityEngine.UI;

namespace CackleCrew.UI
{
    class ModelPickerUIHandler : MonoBehaviour
    {
        public Dictionary<string, ModelButtonUIHandler> options = new Dictionary<string, ModelButtonUIHandler>();
        public string selected;
        public void Init()
        {
            ScanForOptions();
            ChangeOption(options.Keys.First());
        }
        void ScanForOptions()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                var modelButton = child.AddComponent<ModelButtonUIHandler>();
                modelButton.Init();
                options.Add(child.name, modelButton);
                modelButton.transform.GetComponent<Button>().onClick.AddListener(() => ChangeOption(child.name));
            }
        }
        public void ChangeOption(string name)
        {
            foreach (var option in options)
            {
                if (option.Key != name)
                {
                    option.Value.ToggleUI(false);
                }
                else
                {
                    option.Value.ToggleUI(true);
                }
            }
            selected = name;
        }
        void OnEnable()
        {
            ChangeOption(ProfileHelper.GetModel());
        }
    }
}
