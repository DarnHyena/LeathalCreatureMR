using ModelReplacement;
using UnityEngine;

namespace CreatureModelReplacement
{

    public class BodyReplacement : BodyReplacementBase
    {
        
        protected override GameObject LoadAssetsAndReturnModel()
        {
            //Replace with the Asset Name from your unity project 
            string model_name = "CreatureA";
            GameObject model = Assets.MainAssetBundle.LoadAsset<GameObject>(model_name);

            Material replacementMat;

            switch (StartOfRound.Instance.unlockablesList.unlockables[controller.currentSuitID].unlockableName)
            {

                case "Orange suit": //Name of the suit yours will replace
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Red"); // Name of your material
                    break;
                case "CARed": // Duplicate to allow for swapping between default and moresuit
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Red");
                    break;

                case "Green suit":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Green");
                    break;
                case "CAGreen":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Green");
                    break;

                case "Hazard suit":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Haz");
                    break;
                case "CAHaz":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Haz");
                    break;

                case "Pajama suit":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Pajam");
                    break;
                case "CAPajam":
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Pajam");
                    break;

                default:
                    replacementMat = Assets.MainAssetBundle.LoadAsset<Material>("Mat_Red");
                    break;
            }

            SkinnedMeshRenderer[] meshes = model.GetComponentsInChildren<SkinnedMeshRenderer>();

            Debug.Log("Looking for meshes...");
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                Debug.Log("Found a mesh: " + mesh.name);
                //mesh.materials[0].SetTexture("_BaseColorMap", replacementMat);
                mesh.SetMaterial(replacementMat);
            }

            return model;
        }


    }
}