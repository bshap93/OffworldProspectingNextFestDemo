using Domains.UI_Global.Events;
using UnityEngine;

public class QuestGiverHelper : MonoBehaviour
{
    public void TriggerOpenQuestDialogueUI()
    {
        UIEvent.Trigger(UIEventType.OpenQuestDialogue);
    }

    public void TriggerCloseQuestDialogueUI()
    {
        UIEvent.Trigger(UIEventType.CloseQuestDialogue);
    }
}