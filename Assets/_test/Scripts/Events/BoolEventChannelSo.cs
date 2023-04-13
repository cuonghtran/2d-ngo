using System;
using UnityEngine;

namespace BasicNetcode
{
    public class BoolEventChannelSo : ScriptableObject
    {
        public Action<bool> OnEventRaised;

        public void RaiseEvent(bool value)
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
