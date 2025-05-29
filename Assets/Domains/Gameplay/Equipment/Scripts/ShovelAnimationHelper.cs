using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Equipment.Scripts
{
    public class ShovelAnimationHelper : MonoBehaviour
    {
        [SerializeField] private MMSpringPosition springPosition;
        [SerializeField] private Vector3 moveToValue;
        [SerializeField] private float shovelAnimationDelay;


        private void Awake()
        {
        }

        public void OnShovelUse()
        {
            if (springPosition != null) StartCoroutine(SpringShovel());
        }

        private IEnumerator SpringShovel()
        {
            springPosition.MoveToSubtractive(moveToValue);
            yield return new WaitForSeconds(shovelAnimationDelay);
            springPosition.MoveToAdditive(moveToValue);
        }

        public void AnimateEquip()
        {
            // StartCoroutine(EquipAnimationCoroutine());
        }


        public void AnimateUnEquip()
        {
        }
    }
}