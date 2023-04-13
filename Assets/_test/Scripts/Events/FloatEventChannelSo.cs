using System;
using UnityEngine;

namespace BasicNetcode
{
    [CreateAssetMenu(menuName = "Events/Float Event Channel")]
    public class FloatEventChannelSo : ScriptableObject
    {
        public Action<float> OnEventRaised;

        public void RaiseEvent(float value)
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing is listening to this {name} event.");
                return;
            }
            OnEventRaised.Invoke(value);
        }
    }
}
