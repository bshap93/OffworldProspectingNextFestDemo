using System.Collections;
using Domains.Player.Progression;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Briefings
{
    public class InfoDumpController : MonoBehaviour, MMEventListener<UIEvent>
    {
        private bool _isPaused;
        private CanvasGroup canvasGroup;

        public InfoDumpController(bool isPaused)
        {
            _isPaused = isPaused;
        }

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                UnityEngine.Debug.LogError("InfoDumpController: No CanvasGroup found on this GameObject.");
                return;
            }

            // if (ES3.KeyExists("DataWasReset"))
            // {
            //     var dataWasReset = ES3.Load<bool>("DataWasReset");
            //     if (dataWasReset) StartCoroutine(DelayedShowInfoDump());
            // }

            if (!ProgressionManager.TutorialFinished)
                StartCoroutine(DelayedShowInfoDump());
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.CloseUI)
                // shouldShowInfoDump = false;
                HideInfoDump();
        }

        private IEnumerator DelayedShowInfoDump()
        {
            yield return null; // Wait one frame
            ShowInfoDump();
        }

        private void ShowInfoDump()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                _isPaused = true;
                // Time.timeScale = 0;
                UIEvent.Trigger(UIEventType.OpenInfoDump);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                UnityEngine.Debug.LogError("InfoDumpController: No CanvasGroup found on this GameObject.");
            }
        }

        private void HideInfoDump()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                _isPaused = false;
                // Time.timeScale = 1;
                UIEvent.Trigger(UIEventType.CloseInfoDump);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}