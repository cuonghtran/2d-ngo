using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayersManager : Singleton<PlayersManager>
    {
        private NetworkVariable<int> _playersInGame = new NetworkVariable<int>();
        public int PlayersInGame {get { return _playersInGame.Value; } }

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnClientConnected(ulong id)
        {
            if (IsServer)
            {
                Logger.LogGreen($"{id} just connected!");
                _playersInGame.Value++;
            }
        }

        private void OnClientDisconnected(ulong id)
        {
            if (IsServer)
            {
                Logger.LogGreen($"{id} just disconnected!");
                _playersInGame.Value--;
            }
        }
    }
}
