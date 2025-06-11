using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Domains.Items
{
    public class ItemPickupAfterSeconds : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(ItemPickupCoroutine());
        }

        private IEnumerator ItemPickupCoroutine()
        {
            yield return new WaitForSeconds(5);
            gameObject.SetActive(true);
        }
    }
}