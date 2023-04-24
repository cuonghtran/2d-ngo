using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

namespace BasicNetcode
{
    public class WeaponPickup : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Weapon weaponToBePickedUp;
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private Weapon.Rarity rarity;
        [SerializeField] private GameObject InteractCanvas;

        [Header("Floating values")]
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 1f;
        private Vector2 tempPos = new Vector2();

        private bool isTriggered = false;
        private bool isLooted = false;
        private Transform triggerTransform;
        private Vector3 topPosition;
        private Vector3 bottomPosition;
        private bool goingForward = true;

        // Start is called before the first frame update
        void Start()
        {
            weaponTransform.GetComponent<SpriteRenderer>().color = CommonClass.RarityColor.ElementAtOrDefault((int)rarity).Value;
            topPosition = weaponTransform.position + new Vector3(0, 0.07f, 0);
            bottomPosition = weaponTransform.position - new Vector3(0, 0.07f, 0);
        }

        // Update is called once per frame
        void Update()
        {
            // make the weapon transform floating
            tempPos = transform.position;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            transform.position = tempPos;

            // a player in the trigger zone
            if (isTriggered && !isLooted)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    isLooted = true;
                    PickUpWeapon(triggerTransform);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                InteractCanvas.SetActive(true);
                isTriggered = true;
                triggerTransform = collision.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                InteractCanvas.SetActive(false);
                isTriggered = false;
                triggerTransform = null;
            }
        }

        private void PickUpWeapon(Transform collision)
        {
            PlayerWeapons playerWeapons = collision.GetComponent<PlayerWeapons>();
            if (playerWeapons)
            {
                Weapon newWeapon = Instantiate(weaponToBePickedUp);
                newWeapon.rarity = this.rarity;
                newWeapon.FillAmmo();
                playerWeapons.Equip(newWeapon, true);
                Destroy(gameObject);
            }
        }

        
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var offset = new Vector3(0, -0.3f, 0);
            Gizmos.color = CommonClass.RarityColor.ElementAtOrDefault((int)rarity).Value;
            Gizmos.DrawCube(transform.position + offset, new Vector3(0.4f, 0.25f, 0));
        }
#endif
    }
}