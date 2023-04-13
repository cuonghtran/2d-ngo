using System;
using UnityEngine;

namespace BasicNetcode
{
    [CreateAssetMenu(menuName = "Events/Int Event Channel")]
    public class IntEventChannelSo : ScriptableObject
    {
        public Action<int> OnEventRaised;

        public void RaiseEvent(int value)
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
