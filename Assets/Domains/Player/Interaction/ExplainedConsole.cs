using Domains.Gameplay.Managers.Messages;
using Domains.Input.Scripts;
using UnityEngine;

namespace Domains.Player.Interaction
{
    public class ExplainedConsole : MonoBehaviour
    {
        [SerializeField] protected InfoPanelActivator infoPanelActivator;

        protected void OnTriggerEnter(Collider other)
        {
            var uniqueID = infoPanelActivator.uniqueID;
            var panelWasRead = MessageManager.IsPanelMessageRead(uniqueID);
            if (!panelWasRead)
            {
                MessagePanelEvent.Trigger(uniqueID,
                    MessagePanelEventType.MessagePanelOpened);
                infoPanelActivator?.ShowInfoPanel();
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            infoPanelActivator?.HideInfoPanel();
        }
    }
}