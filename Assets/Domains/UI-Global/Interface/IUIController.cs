using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Interface
{
    public abstract class UIController : MonoBehaviour, MMEventListener<UIEvent>
    {
        protected CanvasGroup CanvasGroup;
        protected bool IsPaused;


        public UIController(bool isPaused)
        {
            IsPaused = isPaused;
        }

        protected virtual void Start()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            CloseUI();
        }

        protected void OnEnable()
        {
            this.MMEventStartListening();
        }

        protected void OnDisable()
        {
            this.MMEventStopListening();
        }

        public abstract void OnMMEvent(UIEvent eventType);


        public void CloseUI()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            IsPaused = false;
            // Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OpenUI()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            IsPaused = true;
            // Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}