using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.Code.CH4_S1
{
 
        public class HealthBarPlayer : MonoBehaviour
        {
            public Image fillImage;

            public void SetHealth(float current, float max)
            {
                fillImage.fillAmount = current / max;
            }
        }
    }


