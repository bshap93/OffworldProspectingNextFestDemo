using System.Numerics;
using MoreMountains.Tools;

namespace Domains.Player.Camera
{
    public enum CameraEffectEventType
    {
        ShakeCameraPosition
    }

    public struct CameraEffectEvent
    {
        private static CameraEffectEvent _e;

        public CameraEffectEventType EventType;

        public float Duration;
        public Vector3 InputVector;


        public static void Trigger(CameraEffectEventType cameraEffectEventType,
            float duration, Vector3 inputVector = new())
        {
            _e.EventType = cameraEffectEventType;
            _e.Duration = duration;
            _e.InputVector = inputVector;
            MMEventManager.TriggerEvent(_e);
        }
    }
}