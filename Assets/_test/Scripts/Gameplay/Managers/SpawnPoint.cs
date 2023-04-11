using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicNetcode
{
    public class SpawnPoint : MonoBehaviour
    {
        private void Awake()
        {
            PlayerSpawnSystem.AddSpawnPoint(transform);
        }

        private void OnDestroy()
        {
            PlayerSpawnSystem.RemoveSpawnPoint(transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}
