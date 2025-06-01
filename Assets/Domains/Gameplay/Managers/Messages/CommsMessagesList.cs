using UnityEngine;

namespace Domains.Gameplay.Managers.Messages
{
    [CreateAssetMenu(fileName = "CommsMessagesList", menuName = "Scriptable Objects/Messages/CommsMessagesList",
        order = 1)]
    public class CommsMessagesList : ScriptableObject
    {
        [SerializeField] public CommsMessageObject[] commsMessages;
    }
}