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

        private bool _onCooldown;
        private string _knifeWeaponName = "Knife";
        private float _knifeCooldown = 0.5f;
        private float _weaponCooldownTime = 0f;
        private int _hashKnifeAttack = Animator.StringToHash("Knife_Attack");

        public static Action<int> OnAmmoChanged;

        // Start is called before the first frame update
        void Start()
        {
            if (activeWeapon)
            {
                Equip(activeWeapon, false);
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
                    ChangeActiveWeapon(WeaponSlot.First, false);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChangeActiveWeapon(WeaponSlot.Second, false);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChangeActiveWeapon(WeaponSlot.Third, false);
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

        public void Equip(Weapon newWeapon, bool isPickup)
        {
            int weaponSlotIndex = (int)newWeapon.slot;
            var weapon = GetWeaponByIndex(weaponSlotIndex);
            if (weapon)
                return; // do nothing if pickup an already owned weapon

            weapon = newWeapon;
            weapon.transform.SetParent(_weaponSlots[weaponSlotIndex], false);
            weapon.ChangeColorByRarity((int)newWeapon.rarity);
            equippedWeapons[weaponSlotIndex] = weapon;

            ChangeActiveWeapon(newWeapon.slot, isPickup);
        }

        void ChangeActiveWeapon(WeaponSlot weaponSlot, bool isPickup)
        {
            if (equippedWeapons[(int)weaponSlot] == null)
                return;

            int holsterIndex = activeWeaponIndex;
            int activateIndex = (int)weaponSlot;
            if (holsterIndex != activateIndex) // only switch different weapons
            {
                StartCoroutine(SwitchWeapon(holsterIndex, activateIndex, isPickup));
            }
        }

        IEnumerator HolsterWeapon(int index)
        {
            _onCooldown = true;
            var weapon = GetWeaponByIndex(index);
            if (weapon)
            {
                weapon.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }
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

        IEnumerator SwitchWeapon(int currentIndex, int activateIndex, bool isPickup)
        {
            yield return StartCoroutine(HolsterWeapon(currentIndex));
            yield return StartCoroutine(ActivateWeapon(activateIndex));
            activeWeaponIndex = activateIndex;
            if (isPickup)
                _onWeaponPickedup.RaiseEvent(this);
            else _onWeaponChanged.RaiseEvent(this);
        }
    }
}