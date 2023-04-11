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
            {
                _mainCamera = Camera.main;
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleAiming();
        }

        void HandleAiming()
        {
            if (!IsLocalPlayer)
                return;
                
            mousePosition = UtilsClass.GetMouseWorldPosition(_mainCamera);
            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

            // vertically flip the aim
            Vector3 aimLocalScale = Vector3.one;
            if (aimAngle > 90 || aimAngle < -90)
                aimLocalScale.y = -1f;
            else
                aimLocalScale.y = 1f;
            aimTransform.localScale = aimLocalScale;
        }
    }
}