using System.Linq;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using BasicNetcode.Networking;

namespace BasicNetcode
{
    public class PlayerUiController : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _textPlayerName;

        [ClientRpc]
        public void SetNameClientRpc(string playerName)
        {
            _textPlayerName.text = playerName;
        }
    }
}
