using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayerControl : NetworkBehaviour
    {
        [SerializeField] private float _walkSpeed = 3.5f;
        [SerializeField] private Vector2 _defaultPosition = new Vector2(-4, 4);
        [SerializeField] private NetworkVariable<float> _forwardBackPosition = new NetworkVariable<float>();
        [SerializeField] private NetworkVariable<float> _leftRightPosition = new NetworkVariable<float>();

        // client caching
        private float _oldForwardBackPosition;
        private float _oldleftRightPosition;

        private void Start()
        {
            transform.position = new Vector3(Random.Range(_defaultPosition.x, _defaultPosition.y), 0, 
                Random.Range(_defaultPosition.x, _defaultPosition.y));
        }

        private void Update()
        {
            if (IsServer)
            {
                UpdateServer();
            }

            if (IsClient && IsOwner)
            {
                UpdateClient();
            }
        }

        private void UpdateServer()
        {
            transform.position = new Vector3(transform.position.x + _leftRightPosition.Value, transform.position.y,
                transform.position.z + _forwardBackPosition.Value);
        }

        private void UpdateClient()
        {
            float forwardBackward = 0;
            float leftRight = 0;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                forwardBackward += _walkSpeed;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                forwardBackward -= _walkSpeed;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                leftRight -= _walkSpeed;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                leftRight += _walkSpeed;
            }

            if (_oldForwardBackPosition != forwardBackward || _oldleftRightPosition != leftRight)
            {
                _oldForwardBackPosition = forwardBackward;
                _oldleftRightPosition = leftRight;
                UpdateClientPositionServerRpc(forwardBackward, leftRight);
            }
        }

        [ServerRpc]
        private void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
        {
            _forwardBackPosition.Value = forwardBackward;
            _leftRightPosition.Value = leftRight;
        }
    }
}
