using Domains.Items.Events;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using SerializationData = Sirenix.Serialization.SerializationData;

namespace Domains.Gameplay.Vendor.Train
{
    [ShowOdinSerializedPropertiesInInspector]
    public class ModularTrainController : MonoBehaviour,
        ISerializationCallbackReceiver, MMEventListener<InventoryEvent>
    {
        [SerializeField] [HideInInspector] private SerializationData serializationData;

        [SerializeField] private TrainSegmentController trainSegment;

        // [OdinSerialize] private Queue<TrainSegmentController> trainSegments = new();

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
        }

        public void OnMMEvent(InventoryEvent eventType)
        {
            if (eventType.EventType == InventoryEventType.SellAllItems) SendOffHeadOfTrainQueue();
        }

        public void SendOffHeadOfTrainQueue()
        {
            // var trainSegment = trainSegments.Dequeue();
            StartCoroutine(trainSegment.SendOff());
        }

        // public void EnqueueTrainSegment(TrainSegmentController trainSegment)
        // {
        //     trainSegments.Enqueue(trainSegment);
        // }
    }
}