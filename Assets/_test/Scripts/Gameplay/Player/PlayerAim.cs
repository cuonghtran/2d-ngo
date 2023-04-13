using UnityEngine;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayerAim : NetworkBehaviour
    {
        [SerializeField] private Transform aimTransform;

        public float aimAngle { get; private set; }
        private Vector3 mousePosition = Vector3.zero;
        private Camera _mainCamera;

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
                _mainCamera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            HandleAiming();
        }

        private void HandleAiming()
        {
            if (!IsLocalPlayer)
                return;
                
            if (_mainCamera == null)
                _mainCamera = Camera.main;
            mousePosition = UtilsClass.GetMouseWorldPosition(_mainCamera);
            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
            Vector3 aimLocalScale = Vector3.one;  // vertically flip the aim
            if (aimAngle > 90 || aimAngle < -90)
                aimLocalScale.y = -1f;
            else
                aimLocalScale.y = 1f;
            aimTransform.localScale = aimLocalScale;

            UpdateClientAimServerRpc(aimAngle);
        }

        [ServerRpc]
        private void UpdateClientAimServerRpc(float aimAngle)
        {
            aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
            Vector3 aimLocalScale = Vector3.one;  // vertically flip the aim
            if (aimAngle > 90 || aimAngle < -90)
                aimLocalScale.y = -1f;
            else
                aimLocalScale.y = 1f;
            aimTransform.localScale = aimLocalScale;
        }
    }
}