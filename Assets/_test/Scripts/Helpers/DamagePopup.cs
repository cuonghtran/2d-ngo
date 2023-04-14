using UnityEngine;

namespace BasicNetcode
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private DamageText _damageText;

        private Vector3 _randomizeIntensity = new Vector3(0.2f, 1f, 0);
        private string _damageColorString = "E31B1B";
        private string _whiteColorString = "E0E0E0";

        void Start()
        {
            transform.localPosition += new Vector3(Random.Range(-_randomizeIntensity.x, _randomizeIntensity.x), _randomizeIntensity.y, 0);
        }

        public void SetUp(float dmg, bool displayOnSelf = false)
        {
            var color = displayOnSelf ? UtilsClass.GetColorFromString(_damageColorString)
                                      : UtilsClass.GetColorFromString(_whiteColorString);
            _damageText.Display(dmg, color);
        }
    }
}
