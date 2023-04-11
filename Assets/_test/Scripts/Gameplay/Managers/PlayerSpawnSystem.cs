using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

namespace BasicNetcode
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        private static List<Transform> _spawnPoints = new List<Transform>();
        private int _nextPointIndex = 0;
        
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
            SpawnPlayerServerRpc(NetworkManager.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc(ulong forClientId)
        {
            Debug.Log("Local client id: " + forClientId);
            Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextPointIndex);
            if (spawnPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {_nextPointIndex}.");
                return;
            }

            GameObject playerObject = Instantiate(_playerPrefab, _spawnPoints[_nextPointIndex].position, _spawnPoints[_nextPointIndex].rotation);
            var networkObj = playerObject.GetComponent<NetworkObject>();
            networkObj.SpawnAsPlayerObject(forClientId, true);
            networkObj.ChangeOwnership(forClientId);
            _nextPointIndex++;
        }
    }
}
