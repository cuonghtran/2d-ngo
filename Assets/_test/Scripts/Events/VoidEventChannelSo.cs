using System;
using UnityEngine;

namespace BasicNetcode
{
    [CreateAssetMenu(menuName = "Events/Void Event Channel")]
    public class VoidEventChannelSo : ScriptableObject
    {
        public Action OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing is listening to this {name} event.");
                return;
            }
            OnEventRaised.Invoke();
        }
    }
}
