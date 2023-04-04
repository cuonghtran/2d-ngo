using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

namespace BasicNetcode
{
    public class PlayerHud : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _textPlayerName;
        private NetworkVariable<NetworkString> _playerName = new NetworkVariable<NetworkString>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _playerName.Value = $"Player {OwnerClientId}";
                if (!string.IsNullOrEmpty(_playerName.Value))
                {
                    SetOverlay();
                }
            }
        }

        private void SetOverlay()
        {
            _textPlayerName.text = _playerName.Value;
        }
    }
}
