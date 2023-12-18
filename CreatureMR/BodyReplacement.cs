using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModelReplacement;
//using ModelReplacement.Scripts;
using GameNetcodeStuff;
using LethalCreatureMR.JigglePhysics;

namespace CreatureModelReplacement
{

    public class BodyReplacement : BodyReplacementBase
    {
        //Required universally
        protected override GameObject LoadAssetsAndReturnModel()
        {
            //Replace with the Asset Name from your unity project 
            string model_name = "CreatureA";
            return Assets.MainAssetBundle.LoadAsset<GameObject>(model_name);
        }



        //Miku mod specific scripts. 
        /*protected override void AddModelScripts()
        {
            //Set dynamic bone options
            replacementModel.GetComponentsInChildren<JiggleRigBuilder>().ToList().ForEach(bone =>
            {
                Debug.Log(bone.jiggleRigs.Count);
            });
        }*/

    }
}