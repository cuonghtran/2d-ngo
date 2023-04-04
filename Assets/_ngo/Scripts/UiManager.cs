using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

namespace BasicNetcode
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startServerButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private TMP_Text _playersInGameText;

        private void Start() 
        {
            _startHostButton.onClick.AddListener(() =>
            {
                if (NetworkManager.Singleton.StartHost())
                {
                    Logger.LogGreen("Host started...");
                }
                else
                {
                    Logger.LogGreen("Host could not be started...");
                }
            });

            _startServerButton.onClick.AddListener(() =>
            {
                if (NetworkManager.Singleton.StartServer())
                {
                    Logger.LogGreen("Server started...");
                }
                else
                {
                    Logger.LogGreen("Server could not be started...");
                }
            });

            _startClientButton.onClick.AddListener(() =>
            {
                if (NetworkManager.Singleton.StartClient())
                {
                    Logger.LogGreen("Client started...");
                }
                else
                {
                    Logger.LogGreen("Client could not be started...");
                }
            });
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                _playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
            }
        }
    }
}
