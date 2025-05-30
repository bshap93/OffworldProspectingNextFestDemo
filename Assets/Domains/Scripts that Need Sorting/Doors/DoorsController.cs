using System;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public abstract class DoorsController : MonoBehaviour
    {
        public GameObject RDoor;
        public GameObject LDoor;
        
        [CanBeNull] public MMFeedbacks OpenDoorFeedbacks;
        [CanBeNull] public MMFeedbacks CloseDoorFeedbacks;
        protected bool isOpen;

        [SerializeField] protected ConditionalDoor conditionalDoor;

        private void Start()
        {
            if (conditionalDoor != null)
            {
                if (conditionalDoor.startActive) 
                {
                    OpenDoors();
                }

            }
        }


        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {  
                if (conditionalDoor != null && conditionalDoor.GetLockedState())
                {

                    return;
                }
                OpenDoors();
                
                
            }
        }
        
        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                CloseDoors();
        }

        public abstract void OpenDoors();

        public abstract void CloseDoors();
    }
}