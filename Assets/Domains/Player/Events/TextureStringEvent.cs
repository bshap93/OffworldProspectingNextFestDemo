using System;
using MoreMountains.Tools;

namespace Domains.Player.Events
{
    // [Serializable]
    // public enum AttributeLevelEventType
    // {
    //     LevelUp,
    //     Reset,
    //     Initialize
    // }

    /// <summary>
    ///     Used to change the experience of an attribute
    /// </summary>
    public struct TextureStringEvent
    {
        static TextureStringEvent _e;

        public string StringValue;

        public static void Trigger(string stringValue)
        {
            _e.StringValue = stringValue;

            MMEventManager.TriggerEvent(_e);
        } 
    }
}
