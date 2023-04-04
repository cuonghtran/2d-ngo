using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;

namespace BasicNetcode
{
    public class NetworkRoomPlayer : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject _lobbyUI;
        [SerializeField] private TMP_Text[] _playerNameTexts = new TMP_Text[4];
        [SerializeField] private TMP_Text[] _playerReadyTexts = new TMP_Text[4];
        [SerializeField] private Button _startGameButton;

        [SerializeField] private NetworkVariable<NetworkString> _displayName = new NetworkVariable<NetworkString>("Loading...");
        [SerializeField] private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(false);

        private bool _isLeader;
        public bool IsLeader
        {
            get { return _isLeader; }
            set
            {
                _isLeader = value;
                _startGameButton.gameObject.SetActive(value);
            }
        }
    }
}
