using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModelReplacement;
//using ModelReplacement.Scripts;
//using JigglePhysics;
using GameNetcodeStuff;

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
        /*//==========JiggleRig==========//
        void AddJiggleRigTailToGameObject(GameObject obj)
        {

            JiggleRigBuilder builder = obj.AddComponent<JiggleRigBuilder>();
            // Create our settings.
            JiggleSettings settings = ScriptableObject.CreateInstance<JiggleSettings>();
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.Gravity, 1f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.Friction, 0.5f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.AirFriction, 0.2f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.Blend, 1f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.AngleElasticity, 0.9f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.ElasticitySoften, 0.5f);
            settings.SetParameter(JiggleSettingsBase.JiggleSettingParameter.LengthElasticity, 0.6f);
            builder.AddJiggleRig(obj.transform.Find("attach_skin/Visual/Armature/Hips/Spine/Tail"), settings);

        }*/
        
        /*//Miku mod specific scripts. 
        protected override void AddModelScripts()
        {
            //Set dynamic bone options
            replacementModel.GetComponentsInChildren<DynamicBone>().ToList().ForEach(bone =>
            {
                bone.m_UpdateRate = Plugin.UpdateRate.Value;
                bone.m_DistantDisable = Plugin.disablePhysicsAtRange.Value;
                bone.m_DistanceToObject = Plugin.distanceDisablePhysics.Value;
            });
        }*/

    }
}
