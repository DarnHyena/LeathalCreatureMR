using System.Collections.Generic;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
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
