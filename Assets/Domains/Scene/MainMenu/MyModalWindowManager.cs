using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Domains.Scene.MainMenu
{
    public class MyModalWindowManager : MonoBehaviour, MMEventListener<MainMenuEvent>
    {
        [Header("Resources")] public TextMeshProUGUI windowTitle;

        public TextMeshProUGUI windowDescription;


        public bool useCustomTexts;
        public string titleText = "Title";
        [TextArea] public string descriptionText = "Description here";

        private CanvasGroup canvasGroup;

        private bool isOn;

        private void Start()
        {
            if (canvasGroup == null)
                canvasGroup = gameObject.GetComponent<CanvasGroup>();

            if (useCustomTexts == false)
            {
                windowTitle.text = titleText;
                windowDescription.text = descriptionText;
            }

            CloseDialog();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MainMenuEvent eventType)
        {
            switch (eventType.EventType)
            {
                case MainMenuEventType.NewGameAttempted:
                    ModalWindowIn();
                    break;
                case MainMenuEventType.ExitDialog:
                    ModalWindowOut();
                    break;
            }
        }

        public void OpenDialog()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void CloseDialog()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void ModalWindowIn()
        {
            StopCoroutine("DisableWindow");
            OpenDialog();

            if (isOn == false) isOn = true;
        }

        public void ModalWindowOut()
        {
            if (isOn) isOn = false;

            StartCoroutine("DisableWindow");
        }

        private IEnumerator DisableWindow()
        {
            yield return new WaitForSeconds(0.5f);
            CloseDialog();
        }
    }
}