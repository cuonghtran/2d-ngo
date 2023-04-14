using UnityEngine;
using TMPro;

namespace BasicNetcode
{
    public class DamageText : MonoBehaviour
    {
        public void Display(float dmg, Color color)
        {
            transform.GetComponent<TMP_Text>().color = color;
            transform.GetComponent<TMP_Text>().text = Mathf.RoundToInt(dmg).ToString();
        }

        void DestroyText()
        {
            Destroy(gameObject);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
