using System;
using JetBrains.Annotations;
using MoreMountains.Tools;

namespace Domains.Player.Events
{
    public enum PointedObjectEventType
    {
        PointedObjectChanged,
        TerrainDetected
    }

    public enum PointedObjectType
    {
        Terrain,
        Interactable,
        Other,
        None
    }

    // Define a class to hold texture/object information for UI display
    [Serializable]
    public class PointedObjectInfo
    {
        public string name;
        public PointedObjectType type; // "Terrain", "Interactable", etc.
        public int textureIndex = -1;
        public bool isInteractable;
    }


    public struct PointedObjectEvent
    {
        private static PointedObjectEvent _e;

        public PointedObjectEventType EventType;
        public string Name;
        public PointedObjectInfo PointedObjectInfo;


        public static void Trigger(PointedObjectEventType pointedObjectEventType, string name,
            PointedObjectInfo pointedObjectInfo = null)
        {
            _e.EventType = pointedObjectEventType;
            _e.Name = name;
            if (pointedObjectInfo != null)
            {
                UnityEngine.Debug.Log("Name: " + pointedObjectInfo.name);
                _e.PointedObjectInfo = pointedObjectInfo;
            }

            MMEventManager.TriggerEvent(_e);
        }
    }
}