using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BasicNetcode
{
    public class WeaponReloadUI : MonoBehaviour
    {
        [SerializeField] private Image reloadFillImage;

        [Header("Events")]
        [SerializeField] private PlayerWeaponsEventChannelSo _onWeaponReloading;

        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _onWeaponReloading.OnEventRaised += ReloadWeaponUI;
        }

        private void OnDisable()
        {
            _onWeaponReloading.OnEventRaised -= ReloadWeaponUI;
        }

        private void ReloadWeaponUI(PlayerWeapons playerWeapons)
        {
            float reloadTime = playerWeapons.GetActiveWeapon().ReloadTime;
            StartCoroutine(UpdateReloadBar(reloadTime));
        }

        IEnumerator UpdateReloadBar(float reloadTime)
        {
            canvasGroup.alpha = 1;

            float elapsed = 0;
            while (elapsed < reloadTime)
            {
                yield return null;
                elapsed += Time.deltaTime;
                reloadFillImage.fillAmount = 1.0f - Mathf.Clamp01(elapsed / reloadTime);
            }

            canvasGroup.alpha = 0;
        }
    }
}
