using Domains.Player.Events;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

public class TextureUIIdentifier : MonoBehaviour, MMEventListener<TextureStringEvent>
{
    public TMP_Text textureName;

    void OnEnable()
    {
        this.MMEventStartListening<TextureStringEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<TextureStringEvent>();
    }

    public void OnMMEvent(TextureStringEvent eventType)
    {
        textureName.text = eventType.StringValue;
    }

}
