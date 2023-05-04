using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayerWeapons : NetworkBehaviour
    {
        public enum WeaponSlot
        {
            First = 0,
            Second = 1,
            Third = 2
        }
        [Header("References")]
        public Weapon[] equippedWeapons = new Weapon[3];
        public Weapon weaponToBePickedup;
        [SerializeField] private Transform[] _weaponSlots;
        [SerializeField] private Weapon activeWeapon;
        [SerializeField] private int activeWeaponIndex = -1;
        [SerializeField] private Animator _knifeAnimator;

        [Header("Events")]
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponChanged;
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponPickedup;
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponReloading;
        [SerializeField] private IntEventChannelSo _onAmmoChanged;

        private Weapon _weaponToBePickedUp;
        public Weapon WeaponToBePickedUp 
        { 
            get { return _weaponToBePickedUp; }
            set { _weaponToBePickedUp = value; }
        }

        private string _playerOwner;
        private bool _onCooldown;
        private string _knifeWeaponName = "Knife";
        private float _knifeCooldown = 0.5f;
        private float _weaponCooldownTime = 0f;
        private int _hashKnifeAttack = Animator.StringToHash("Knife_Attack");

        // Start is called before the first frame update
        void Start()
        {
            if (activeWeapon && IsOwner)
            {
                _onWeaponPickedup.RaiseEvent(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsLocalPlayer)
                return;

            ChangeWeapon();
            StartCoroutine(ReloadActiveWeapon());
            FireWeapon();
        }

        void ChangeWeapon()
        {
            if (!activeWeapon.IsReloading)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ChangeActiveWeapon((int)WeaponSlot.First, false);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChangeActiveWeapon((int)WeaponSlot.Second, false);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChangeActiveWeapon((int)WeaponSlot.Third, false);
                }
            }
        }

        private void FireWeapon()
        {
            if (Input.GetMouseButton(0))
            {
                if (activeWeapon && !_onCooldown && !activeWeapon.IsReloading)
                {
                    if (activeWeapon.slot == WeaponSlot.Third && activeWeapon.name == _knifeWeaponName)
                    {
                        _onCooldown = true;
                        RequestToUseKnifeServerRpc();
                    }
                    else
                    {
                        if (Time.time >= _weaponCooldownTime)
                        {
                            _weaponCooldownTime = Time.time + activeWeapon.WeaponCooldown;
                            activeWeapon.ReduceAmmo();
                            _onAmmoChanged.RaiseEvent(activeWeapon.AmmoCount);
                            RequestToShootServerRpc();
                        }
                    }
                }
            }
        }

        [ServerRpc]
        private void RequestToUseKnifeServerRpc()
        {
            UseKnifeClientRpc();
        }

        [ClientRpc]
        private void UseKnifeClientRpc()
        {
            _knifeAnimator.SetTrigger(_hashKnifeAttack);
            StartCoroutine(ReloadKnife());
        }

        [ServerRpc]
        private void RequestToShootServerRpc()
        {
            ShootClientRpc();
        }

        [ClientRpc]
        private void ShootClientRpc()
        {
            activeWeapon.OnFireBullets();
        }

        IEnumerator ReloadActiveWeapon()
        {
            if (activeWeapon && (Input.GetKeyDown(KeyCode.R) || activeWeapon.CheckOutOfAmmo()))
            {
                if (activeWeapon.slot != WeaponSlot.Third) // don't reload third weapons
                {
                    _onWeaponReloading.RaiseEvent(this);
                    yield return StartCoroutine(activeWeapon.Reload());
                    _onAmmoChanged.RaiseEvent(activeWeapon.AmmoCount);
                }
            }
        }

        IEnumerator ReloadKnife()
        {
            float elapsedTime = 0;
            while (elapsedTime <= _knifeCooldown)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _onCooldown = false;
        }

        Weapon GetWeaponByIndex(int index)
        {
            if (index < 0 || index >= equippedWeapons.Length)
                return null;
            return equippedWeapons[index];
        }

        public Weapon GetActiveWeapon()
        {
            return GetWeaponByIndex(activeWeaponIndex);
        }

        // public void Equip(Weapon newWeapon, bool isPickup)
        // {
        //     int weaponSlotIndex = (int)newWeapon.slot;
        //     var weapon = GetWeaponByIndex(weaponSlotIndex);
        //     if (weapon)
        //         return; // do nothing if pickup an already owned weapon

        //     weapon = newWeapon;
        //     weapon.transform.SetParent(_weaponSlots[weaponSlotIndex], false);
        //     weapon.ChangeColorByRarity((int)newWeapon.rarity);
        //     equippedWeapons[weaponSlotIndex] = weapon;

        //     ChangeActiveWeapon(weaponSlotIndex, isPickup);
        // }

        void ChangeActiveWeapon(int newWeaponIndex, bool isPickup)
        {
            ServerRpcParams rpcParams = new ServerRpcParams
            {
                Receive = new ServerRpcReceiveParams
                {
                    SenderClientId = OwnerClientId
                }
            };
            RequestChangeWeaponServerRpc(newWeaponIndex, isPickup, rpcParams);
        }

        [ServerRpc]
        private void RequestChangeWeaponServerRpc(int newWeaponIndex, bool isPickup, ServerRpcParams rpcParams)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { rpcParams.Receive.SenderClientId }
                }
            };

            UpdateClientWeaponClientRpc(newWeaponIndex, isPickup, clientRpcParams);
        }

        [ClientRpc]
        private void UpdateClientWeaponClientRpc(int newWeaponIndex, bool isPickup, ClientRpcParams rpcParams = default)
        {
            if (activeWeaponIndex == newWeaponIndex || equippedWeapons[newWeaponIndex] == null)
                return;

            int holsterIndex = activeWeaponIndex;
            if (holsterIndex != newWeaponIndex) // only switch different weapons
            {
                StartCoroutine(SwitchWeapon(newWeaponIndex, isPickup));
            }
        }

        public void PostPickUpHandler(Weapon newWeapon)
        {
            if (!IsClient) return;

            int newWeaponIndex = (int)newWeapon.slot;
            equippedWeapons[newWeaponIndex] = newWeapon;
            newWeapon.transform.SetParent(_weaponSlots[newWeaponIndex], false);
            newWeapon.playerOwner = _playerOwner;
            newWeapon.SetUpOwner();

            RequestActivatePickedUpWeaponServerRpc(newWeaponIndex);
        }

        [ServerRpc]
        public void RequestActivatePickedUpWeaponServerRpc(int newWeaponIndex)
        {
            if (activeWeaponIndex == newWeaponIndex || equippedWeapons[newWeaponIndex] == null)
                return;

            activeWeaponIndex = newWeaponIndex;
            _weaponCooldownTime = 0;
            UpdatePickedUpWeaponClientRpc(newWeaponIndex);
        }

        [ClientRpc]
        private void UpdatePickedUpWeaponClientRpc(int newWeaponIndex)
        {
            StartCoroutine(SwitchWeapon(newWeaponIndex, true));
        }

        IEnumerator HolsterCurrentWeapon()
        {
            _onCooldown = true;
            foreach (Weapon wp in equippedWeapons)
            {
                if (wp != null) wp.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.05f);
        }

        IEnumerator ActivateWeapon(int index)
        {
            var weapon = GetWeaponByIndex(index);
            if (weapon)
            {
                activeWeapon = weapon;
                weapon.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                _onCooldown = false;
                _onAmmoChanged.RaiseEvent(weapon.AmmoCount);
            }
        }

        IEnumerator SwitchWeapon(int activateIndex, bool isPickup)
        {
            yield return StartCoroutine(HolsterCurrentWeapon());
            yield return StartCoroutine(ActivateWeapon(activateIndex));
            activeWeaponIndex = activateIndex;
            if (IsOwner)
            {
                if (isPickup)
                    _onWeaponPickedup.RaiseEvent(this);
                else 
                    _onWeaponChanged.RaiseEvent(this);
            }
            
        }
    }
}