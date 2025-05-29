using System.Collections;
using DG.Tweening;
using INab.Dissolve;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Domains.Gameplay.Vendor.Train
{
    public enum TrainSegmentState
    {
        Docked,
        Outbound,
        HasLeftArea,
        Inbound
    }

    public class TrainSegmentController : MonoBehaviour
    {
        [Header("Feedbacks")] public MMFeedbacks sendoffFeedbacks;

        [SerializeField] private MMFeedbacks reDockFeedbacks;
        [SerializeField] private MMFeedbacks startBackFeedbacks;

        [Header("Animation")] [SerializeField] private float sendoffAnimationDuration = 10f;

        private ModularTrainController _modularTrainController;

        private Dissolver dissolver;
        private bool isDocked;
        private bool isNotBlocked;
        private DOTweenAnimation sendoffAnimation;


        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            isDocked = true;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            dissolver = GetComponent<Dissolver>();

            _modularTrainController = GetComponentInParent<ModularTrainController>();

            if (_modularTrainController == null)
            {
                UnityEngine.Debug.LogError("ModularTrainController not found in parent.");
                return;
            }

            sendoffAnimation = GetComponent<DOTweenAnimation>();
            if (sendoffAnimation == null)
            {
                UnityEngine.Debug.LogError("DOTweenAnimation component not found on this GameObject.");
                return;
            }

            sendoffAnimation.DOPause();
        }

        [Button]
        public IEnumerator SendOff()
        {
            if (isDocked)
            {
                sendoffAnimation.DORestart();
                isDocked = false;

                sendoffFeedbacks?.PlayFeedbacks();


                sendoffAnimation.DOPlay();

                yield return new WaitForSeconds(sendoffAnimationDuration);

                startBackFeedbacks?.PlayFeedbacks();


                Recall();
            }
        }

        private void Recall()
        {
            StartCoroutine(RecallSegment());
        }

        public IEnumerator RecallSegment()
        {
            if (!isDocked)
            {
                dissolver.Dissolve();
                yield return new WaitForSeconds(dissolver.duration);
                dissolver.Materialize();
                yield return new WaitForSeconds(dissolver.duration);
                sendoffAnimation.DOPause();
                sendoffAnimation.DOPlayBackwards();

                yield return new WaitForSeconds(sendoffAnimationDuration);


                isDocked = true;
                // _modularTrainController.EnqueueTrainSegment(this);
                reDockFeedbacks?.PlayFeedbacks();
            }
        }

        // Add this method to your controller if you don't have it already
        public void OnExitArea()
        {
        }
    }
}