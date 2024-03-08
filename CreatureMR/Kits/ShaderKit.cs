using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CackleCrew.ThisIsMagical
{
    //A couple of utility functions to handle Shader Options!
    //This is where shader stuff will be added in the future , if needed..!
    public static class ShaderKit
    {
        public static void ClearKeywords(ref Material material, string keyword)
        {
            foreach (var enabledKeyword in material.enabledKeywords)
            {
                if (enabledKeyword.name.Contains(keyword))
                    material.DisableKeyword(enabledKeyword.name);
            }
        }
        public static void SetKeyword(ref Material material, string keyword, string option)
        {
            material.EnableKeyword($"_{keyword}_{option}");
        }
    }
}
