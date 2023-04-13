using System;
using UnityEngine;

namespace BasicNetcode
{
    [CreateAssetMenu(menuName = "Events/Player Weapons Event")]
    public class PlayerWeaponsEventChannelSo : ScriptableObject
    {
        public Action<PlayerWeapons> OnEventRaised;

        public void RaiseEvent(PlayerWeapons playerWeapons)
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"Nothing is listening to this {name} event.");
                return;
            }
            OnEventRaised.Invoke(playerWeapons);
        }
    }
}
