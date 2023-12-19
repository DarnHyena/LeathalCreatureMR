using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModelReplacement;
using GameNetcodeStuff;
using LethalCreatureMR.JigglePhysics;
using static LethalCreatureMR.JigglePhysics.JiggleRigBuilder;

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



        //Creature mod specific scripts. 
        protected override void Start()
        {
            base.Start();

                Debug.Log("Oh hai");
            //Tail
            Transform tail = replacementModel.transform.Find("Armature/Hips/TailBase");
            JiggleSettings tailSettings = ScriptableObject.CreateInstance<JiggleSettings>();
            tailSettings.SetData(new JiggleSettingsData()
            {
                gravityMultiplier = 1.0f, // Gravity should be partially expressed in the default shape of the tail, but it absolutely needs to flip and fall depending on orientation.
                friction = 0.5f, // High friction, tails don't wag unless the owner means to generally. It's a limb for balance!
                angleElasticity = 0.5f, // Low angle elasticity, should sweep into position slowly, intentionally. Would want to turn this up for short tails though.
                blend = 1f, // Full blend by default, user can adjust this themselves.
                airDrag = 0.2f, // Tail flaps in the wind! Trails behind the character during movement.
                elasticitySoften = 0.5f, // Tails don't really care about being "perfect".
                lengthElasticity = 0.5f, // With such low angle elasticity, we need a decent amount of length elasticity to make sure the tail doesn't just stretch into oblivion. It still streches a little though!
            });
            List<Transform> ignoredTransforms = new List<Transform>();
            JiggleRigBuilder.JiggleRig tailRig = new JiggleRigBuilder.JiggleRig(tail, tailSettings, ignoredTransforms, new List<Collider>());

            //Ears
            Transform earl = replacementModel.transform.Find("Armature/Hips/Spine/Chest/Neck/Head/Ear.L");
            Transform earr = replacementModel.transform.Find("Armature/Hips/Spine/Chest/Neck/Head/Ear.R");
            JiggleSettings earSettings = ScriptableObject.CreateInstance<JiggleSettings>();
            earSettings.SetData(new JiggleSettingsData()
            {
                gravityMultiplier = 1.0f,
                friction = 0.5f,
                angleElasticity = 0.8f,
                blend = 1f,
                airDrag = 0.2f,
                lengthElasticity = 0.6f,
                elasticitySoften = 0.5f,
            });
            
            JiggleRigBuilder.JiggleRig earlRig = new JiggleRigBuilder.JiggleRig(earl, earSettings, ignoredTransforms, new List<Collider>());
            JiggleRigBuilder.JiggleRig earrRig = new JiggleRigBuilder.JiggleRig(earr, earSettings, ignoredTransforms, new List<Collider>());

            Transform tank = replacementModel.transform.Find("Armature/Hips/Spine/Chest/Tank");
            JiggleSettings tankSettings = ScriptableObject.CreateInstance<JiggleSettings>();
            tankSettings.SetData(new JiggleSettingsData()
            {
                gravityMultiplier = 1.0f,
                friction = 0.9f,
                angleElasticity = 0.8f,
                blend = 1f,
                airDrag = 0.5f,
                lengthElasticity = 1.0f,
                elasticitySoften = 0.16f,
            });

            JiggleRigBuilder.JiggleRig tankRig = new JiggleRigBuilder.JiggleRig(tank, tankSettings, ignoredTransforms, new List<Collider>());

            JiggleRigBuilder builder = replacementModel.GetComponentInChildren<JiggleRigBuilder>();
            builder.jiggleRigs = new List<JiggleRigBuilder.JiggleRig>() { tailRig,earrRig, earlRig, tankRig };
           
        }
        
    }
}