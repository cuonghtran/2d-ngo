using System;
using UnityEngine;

namespace BasicNetcode
{
    [CreateAssetMenu(menuName = "Events/Hit Points Event Channel")]
    public class HitPointsEventChannelSo : ScriptableObject
    {
        public Action<float, float> OnEventRaised;

        public void RaiseEvent(float healthValue, float armorValue)
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing is listening to this {name} event.");
                return;
            }
            OnEventRaised.Invoke(healthValue, armorValue);
        }
    }
}
