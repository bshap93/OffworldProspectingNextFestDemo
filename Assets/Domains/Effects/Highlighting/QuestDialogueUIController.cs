using Domains.UI_Global.Events;
using Domains.UI_Global.Interface;
using UnityEngine;

namespace Domains.Effects.Highlighting
{
    public class QuestDialogueUIController : UIController
    {
        private CanvasGroup _canvasGroup;

        public QuestDialogueUIController(bool isPaused) : base(isPaused)
        {
        }


        public override void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenQuestDialogue)
                OpenUI();
            else if (eventType.EventType == UIEventType.CloseQuestDialogue) CloseUI();
        }
    }
}