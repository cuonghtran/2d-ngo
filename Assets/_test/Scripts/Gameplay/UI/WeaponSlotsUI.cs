using UnityEngine;
using DG.Tweening;

namespace BasicNetcode
{
    public class WeaponSlotsUI : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponChangedEvent;
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponPickedupEvent;

        float _transitionTime = 0.15f;
        float _activateAlpha = 0.85f;
        float _deactivateAlpha = 0.6f;
        float _originPosY = 0;
        float _posYChange = 3;

        private void OnEnable()
        {
            _onWeaponChangedEvent.OnEventRaised += UpdateActiveSlotUI;
            _onWeaponPickedupEvent.OnEventRaised += UpdateActiveSlotWhenPickupUI;
        }

        private void OnDisable()
        {
            _onWeaponChangedEvent.OnEventRaised -= UpdateActiveSlotUI;
            _onWeaponPickedupEvent.OnEventRaised -= UpdateActiveSlotWhenPickupUI;
        }

        private Weapon[] equippedWeapons = new Weapon[3];

        public void UpdateActiveSlotUI(PlayerWeapons playerWeapons)
        {
            int activeSlot = (int)playerWeapons.GetActiveWeapon().slot;

            foreach (RectTransform rect in transform)
            {
                if (rect.GetSiblingIndex() == activeSlot)
                    rect.DOLocalMoveY(_posYChange, _transitionTime).OnComplete(() => rect.GetComponent<CanvasGroup>().alpha = _activateAlpha);
                else rect.DOLocalMoveY(_originPosY, _transitionTime).OnComplete(() => rect.GetComponent<CanvasGroup>().alpha = _deactivateAlpha);
            }
        }

        public void UpdateActiveSlotWhenPickupUI(PlayerWeapons playerWeapons)
        {
            equippedWeapons = playerWeapons.equippedWeapons;
            int activeSlot = (int)playerWeapons.GetActiveWeapon().slot;

            foreach (RectTransform rect in transform)
            {
                int wpIndex = rect.GetSiblingIndex();
                // update equipped weapons
                if (equippedWeapons[wpIndex] != null)
                {
                    var sprite = equippedWeapons[wpIndex].GetComponent<SpriteRenderer>().sprite;
                    rect.GetComponent<SlotUI>().UpdateSlotImage(sprite, (int)equippedWeapons[wpIndex].rarity);
                }

                if (wpIndex == activeSlot)
                    rect.DOLocalMoveY(_posYChange, _transitionTime).OnComplete(() => rect.GetComponent<CanvasGroup>().alpha = _activateAlpha);
                else rect.DOLocalMoveY(_originPosY, _transitionTime).OnComplete(() => rect.GetComponent<CanvasGroup>().alpha = _deactivateAlpha);
            }

        }
    }
}