using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicNetcode
{
    public partial class Weapon : MonoBehaviour
    {
        public enum Rarity
        {
            Normal = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Blank = 4,
        }
    }
}