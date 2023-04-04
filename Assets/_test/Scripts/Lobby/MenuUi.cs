using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace BasicNetcode
{
    public class MenuUi : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField _inputPlayerName;
        [SerializeField] private TMP_InputField _inputIpAddress;
        [SerializeField] private Button _buttonConfirm;
        [SerializeField] private Button _buttonEnter;
        [SerializeField] private GameObject _panelMenu;
        [SerializeField] private GameObject _panelLandingPage;
        [SerializeField] private GameObject _panelEnterIpAddress;
        
        [Header("References")]
        [SerializeField] private NetworkManager _networkManager;

        public string PlayerName { get; private set; }
        private const string PLAYER_NAME_STRING = "PlayerName";

        private void Start()
        {
            SetUpInputField();
        }

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PLAYER_NAME_STRING))
                return;
            string name = PlayerPrefs.GetString(PLAYER_NAME_STRING);
            _inputPlayerName.text = name;
            SetPlayerName(name);
        }

        private void SetPlayerName(string name)
        {
            _buttonConfirm.interactable = !string.IsNullOrEmpty(name);
        }

        public void OnConfirmButtonClicked()
        {
            PlayerName = _inputPlayerName.text;
            PlayerPrefs.SetString(PLAYER_NAME_STRING, PlayerName);

            _panelMenu.SetActive(false);
            _panelLandingPage.SetActive(true);
        }

        public void OnHostGameButtonClicked()
        {
            _networkManager.StartHost();
            _panelLandingPage.SetActive(false);
        }

        public void OnJoinAsClientButtonClicked()
        {
            _panelLandingPage.SetActive(false);
            _panelEnterIpAddress.SetActive(true);
        }

        public void OnEnterLobbyButtonClicked()
        {
            string ipAddress = _inputIpAddress.text.Trim();
            _networkManager.GetComponent<UnityTransport>().ConnectionData.Address = ipAddress;
            _networkManager.StartClient();
            _buttonEnter.interactable = false;
        }
    }
}
