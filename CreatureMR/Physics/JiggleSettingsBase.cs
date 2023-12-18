using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LethalCreatureMR.JigglePhysics {
public class JiggleSettingsBase : ScriptableObject {
    public virtual JiggleSettingsData GetData() {
        return new JiggleSettingsData();
    }

    public virtual float GetRadius(float normalizedIndex) {
        return 0f;
    }
}

}