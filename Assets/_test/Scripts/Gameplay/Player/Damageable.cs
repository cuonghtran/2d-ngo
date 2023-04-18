using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BasicNetcode.Message;
using Unity.Netcode;

namespace BasicNetcode
{
    public class Damageable : NetworkBehaviour
    {
        public struct DamageMessage
        {
            public GameObject damager;
            public string sourcePlayer;
            public float amount;
            public Vector3 direction;
            public float knockBackForce;

            public bool stopCamera;
        }

        [Header("References")]
        [SerializeField] private PlayerUiController _playerUiController;
        [SerializeField] private GameObject _damagePopupPrefab;

        [Header("Events")]
        [SerializeField] private HitPointsEventChannelSo _onHitPointsChanged;

        public float maxHealth;
        public float currentHealth { get; private set; }
        public float maxArmor;
        public float currentArmor { get; private set; }
        public bool isInvulnerable;
        private float invulnerabilityTime = 0.25f;
        protected float _timeSinceLastHit = 0.0f;
        protected Collider _collider;

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            ResetDamageable();
        }

        // Update is called once per frame
        // void Update()
        // {
        //     if (isInvulnerable)
        //     {
        //         _timeSinceLastHit += Time.deltaTime;
        //         if (_timeSinceLastHit > invulnerabilityTime)
        //         {
        //             _timeSinceLastHit = 0.0f;
        //             isInvulnerable = false;
        //         }
        //     }
        // }

        public void ResetDamageable()
        {
            currentHealth = maxHealth;
            currentArmor = maxArmor;
            isInvulnerable = false;
            _timeSinceLastHit = 0f;
        }

        public void SetColliderState(bool enabled)
        {
            _collider.enabled = enabled;
        }

        public void ApplyDamage(DamageMessage data)
        {
            // already dead or invulnerable
            if (currentHealth <= 0 || isInvulnerable)
                return;

            CalculateDamageDoneServerRpc(data.amount);
            TargetShowDamagePopupText(data.amount);
        }

        [ServerRpc]
        private void CalculateDamageDoneServerRpc(float amount)
        {
            if (amount <= currentArmor)  // when the damage is less than current armor
            {
                currentArmor = Mathf.Max(currentArmor - amount, 0);
            }
            else  // when the damage is greater than current armor
            {
                float leftAmt = Mathf.Abs(currentArmor - amount);
                currentArmor = 0;
                currentHealth = Mathf.Max(currentHealth - leftAmt, 0);
            }
            _onHitPointsChanged.RaiseEvent(currentHealth, currentArmor);
        }

        [ClientRpc]
        private void UpdateClientHitPointsClientRpc()
        {

        }

        private void TargetShowDamagePopupText(float dmg)
        {
            var topMostPos = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
            Vector3 dmgPos = new Vector3(transform.position.x, transform.position.y + topMostPos, transform.position.z);

            var dmgText = Instantiate(_damagePopupPrefab, dmgPos, Quaternion.identity);
            dmgText.GetComponent<DamagePopup>().SetUp(dmg, true);
        }
    }

}