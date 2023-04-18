using BasicNetcode.Networking;
using TMPro;
using UnityEngine;

namespace BasicNetcode
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField displayNameInputField;

        private void Start()
        {
            displayNameInputField.text = PlayerPrefs.GetString("PlayerName", "");
        }

        public void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            GameNetPortal.Instance.StartHost();
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            ClientGameNetPortal.Instance.StartClient();
        }
    }
}

