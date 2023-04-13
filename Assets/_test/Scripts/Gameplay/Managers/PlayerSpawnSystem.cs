using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using BasicNetcode.Networking;

namespace BasicNetcode
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        public static PlayerSpawnSystem Instance => instance;
        private static PlayerSpawnSystem instance;

        [SerializeField] private GameObject _playerPrefab;

        public List<GameObject> _playersList = new List<GameObject>();
        public List<GameObject> PlayersList
        {
            get { return _playersList; }
            private set { }
        }
        private static List<Transform> _spawnPoints = new List<Transform>();
        private int _nextPointIndex = 0;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public static void AddSpawnPoint(Transform pointTransform)
        {
            _spawnPoints.Add(pointTransform);
            _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform pointTransform)
        {
            _spawnPoints.Remove(pointTransform);
        }

        public override void OnNetworkSpawn()
        {
            SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            if (!IsServer)
                return;
            
            foreach (var player in ServerGameNetPortal.Instance.ClientData)
            {
                Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextPointIndex);
                if (spawnPoint == null)
                {
                    Debug.LogError($"Missing spawn point for player {_nextPointIndex}.");
                    return;
                }

                GameObject playerObject = Instantiate(_playerPrefab, _spawnPoints[_nextPointIndex].position, _spawnPoints[_nextPointIndex].rotation);
                _playersList.Add(playerObject);
                var networkObj = playerObject.GetComponent<NetworkObject>();
                networkObj.SpawnAsPlayerObject(player.Value.ClientId, true);
                networkObj.ChangeOwnership(player.Value.ClientId);
                var playerController = playerObject.GetComponent<PlayerUiController>();
                playerController.SetNameClientRpc(player.Value.PlayerName);
                _nextPointIndex++;
            }
        }
    }
}
