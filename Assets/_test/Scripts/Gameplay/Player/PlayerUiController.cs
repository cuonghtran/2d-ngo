using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

namespace BasicNetcode
{
    public class PlayerUiController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _textPlayerName;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _armorFillImage;
        
        [Header("Events")]
        [SerializeField] private HitPointsEventChannelSo _onPlayerHitPointsChanged;

        private float _updateSpeed = 0.13f;
        private const float MAX_CHARACTER_HEALTH = 100;
        private const float MAX_CHARACTER_ARMOR = 100;

        private void OnEnable()
        {
            _onPlayerHitPointsChanged.OnEventRaised += UpdatePlayerUi;
        }

        private void OnDisable()
        {
            _onPlayerHitPointsChanged.OnEventRaised -= UpdatePlayerUi;
        }

        private void UpdatePlayerUi(float newHealthValue, float newArmorValue)
        {
            StartCoroutine(UpdateHitPointSequence(newHealthValue, newArmorValue));
        }

        private IEnumerator UpdateHitPointSequence(float newHealthValue, float newArmorValue)
        {
            float preChangeHealthPct = _healthFillImage.fillAmount;
            float newHealthPct = newHealthValue / MAX_CHARACTER_HEALTH;
            float preChangeArmorPct = _armorFillImage.fillAmount;
            float newArmorPct = newArmorValue / MAX_CHARACTER_ARMOR;

            if (newArmorPct != preChangeArmorPct)
            {
                float elapsed = 0;
                while (elapsed < _updateSpeed)
                {
                    elapsed += Time.deltaTime;
                    _armorFillImage.fillAmount = Mathf.Lerp(preChangeArmorPct, newArmorPct, elapsed / _updateSpeed);
                    yield return null;
                }
                _armorFillImage.fillAmount = newArmorPct;
            }

            if (newHealthPct != preChangeHealthPct)
            {
                float elapsed = 0;
                while (elapsed < _updateSpeed)
                {
                    elapsed += Time.deltaTime;
                    _healthFillImage.fillAmount = Mathf.Lerp(preChangeHealthPct, newHealthPct, elapsed / _updateSpeed);
                    yield return null;
                }
                _healthFillImage.fillAmount = newHealthPct;
            }
        }

        [ClientRpc]
        public void SetNameClientRpc(string playerName)
        {
            _textPlayerName.text = playerName;
        }
    }
}
