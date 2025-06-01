using JetBrains.Annotations;
using UnityEngine;

namespace Domains.Gameplay.Managers.Messages
{
    [CreateAssetMenu(fileName = "CommsMessagesList", menuName = "Scriptable Objects/Messages/CommsMessageObject",
        order = 1)]
    public class CommsMessageObject : ScriptableObject
    {
        [SerializeField] [CanBeNull] public Sprite commsMessageIcon;
        [SerializeField] public string commsMessageId;
        [SerializeField] [CanBeNull] public Sprite commsMessageImage;

        [TextArea(15, 20)] [SerializeField] public string commsMessageRawBodyText;

        [SerializeField] public string commsMessageRawTitleText;

        [TextArea(4, 5)] [SerializeField] public string senderName;
    }
}