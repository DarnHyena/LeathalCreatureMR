using CackleCrew.Helpers;
using ModelReplacement;
using UnityEngine;

namespace CreatureModelReplacement
{
    public class BodyReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return CustomizationHelper.GenerateCustomModel(controller);
        }
    }
}