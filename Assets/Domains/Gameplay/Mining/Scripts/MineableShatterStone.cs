using UnityEngine;

namespace Domains.Gameplay.Mining.Scripts
{
    public class MineableShatterStone : MonoBehaviour, IInteractable
    {
        private OreNode _oreNode;


        private void OnEnable()
        {
            var myGameObject = gameObject;

            _oreNode = myGameObject.GetComponent<OreNode>();
        }

        public void Interact()
        {
            if (_oreNode != null) _oreNode.MinableMineHit();
        }

        public void ShowInteractablePrompt()
        {
        }

        public void HideInteractablePrompt()
        {
        }
    }
}