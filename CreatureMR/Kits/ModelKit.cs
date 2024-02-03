using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    public static class ModelKit
    {
        public static Dictionary<string, GameObject> registeredModels = new Dictionary<string, GameObject>();
        public static GameObject GetModel(string modelName)
        {
            if (registeredModels.Count == 0)
            {
                Debug.LogError("NO MODELS REGISTERED!!!");
                return null;
            }
            if (string.IsNullOrEmpty(modelName))
                modelName = registeredModels.Keys.First();
            if (!registeredModels.TryGetValue(modelName, out var model))
            {
                Debug.LogWarning($"{modelName} Model Doesn't Exist.");
            }
            if (model == null)
            {
                model = registeredModels.Values.First();
            }
            return model;
        }
        public static bool HasModel(string modelName)
        {
            var i = (string.IsNullOrEmpty(modelName)) ? string.Empty : modelName;
            return registeredModels.ContainsKey(i);
        }
        public static void RegisterModel(string modelName, GameObject prefab)
        {
            if (string.IsNullOrWhiteSpace(modelName))
                return;
            if (registeredModels.TryGetValue(modelName, out var registeredPrefab))
            {
                registeredModels.Remove(modelName);
            }
            registeredModels.Add(modelName, prefab);
        }
        public static string GetDefaultModel()
        {
            return registeredModels.Keys.First();
        }
        public static void ClearModels()
        {
            registeredModels.Clear();
        }
    }
}
