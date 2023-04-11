using Unity.Netcode.Components;
using UnityEngine;

namespace BasicNetcode
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
