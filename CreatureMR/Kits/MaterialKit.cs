using System.Collections.Generic;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //This class might be redundant. How ever, it helps organize the materials for the different configurations.
    //TODO Consider if it would be worth it to combine the MaterialKit and ModelKit together into a 
    //"Library" class with the ability to have "alt" accessible names. using generic T for the string , data.
    //Much like how the Profile class is a globally accessible static class.
    public static class MaterialKit
    {
        private static Dictionary<string, Material> _MaterialPool = new Dictionary<string, Material>();
        public static bool TryGetMaterial(string name, out Material material)
        {
            return _MaterialPool.TryGetValue(name, out material);
        }
        public static void SetMaterial(string name, Material material)
        {
            if (_MaterialPool.ContainsKey(name))
            {
                _MaterialPool.Remove(name);
            }
            _MaterialPool.Add(name, material);
        }
        public static void ClearMaterials()
        {
            _MaterialPool.Clear();
        }
    }
}
