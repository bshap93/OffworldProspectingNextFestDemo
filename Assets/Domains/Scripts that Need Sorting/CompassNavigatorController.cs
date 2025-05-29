using System.Collections.Generic;
using CompassNavigatorPro;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class CompassNavigatorController : MonoBehaviour
    {
        [SerializeField] private List<CompassProPOI> compassProPOIs = new();
        private CompassPro _compassPro;

        private bool hasUsedMoreInfo;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _compassPro = FindFirstObjectByType<CompassPro>();
            if (_compassPro != null)
            {
                // _compassPro.showOnScreenIndicators = true;
                // _compassPro.showOffScreenIndicators = true;
                // _compassPro.UpdateSettings();
                // UnityEngine.Debug.Log("Compass settings initialized");
            }
            else
            {
                UnityEngine.Debug.LogWarning("CompassPro component not found.");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // if (CustomInputBindings.IsGetMoreInfoPressed())
            // {
            //     if (_compassPro != null)
            //     {
            //         if (!hasUsedMoreInfo)
            //         {
            //             TutorialEvent.Trigger(TutorialEventType.PlayerUsedMoreInfo);
            //             hasUsedMoreInfo = true;
            //         }
            //
            //         _compassPro.showOnScreenIndicators = true;
            //         _compassPro.showOffScreenIndicators = true;
            //         _compassPro.UpdateSettings();
            //     }
            // }
            // else
            // {
            //     if (_compassPro != null)
            //     {
            //         _compassPro.showOnScreenIndicators = false;
            //         _compassPro.showOffScreenIndicators = false;
            //         _compassPro.UpdateSettings();
            //     }
            // }
        }
    }
}