using MoreMountains.Feedbacks;
using UnityEngine;

public class SandwormAnimationFeedbacks : MonoBehaviour
{
    public MMFeedbacks roar01Feedbacks;
    public MMFeedbacks appearFeedbacks;
    public MMFeedbacks spitFeedbacks;
    public MMFeedbacks idleUndergroundFeedbacks;
    public MMFeedbacks idleToJumpUndergroundFeedbacks;
    public MMFeedbacks burrownIntoGroundFeedbacks;
    public MMFeedbacks continuedRoarFeedbacks;

    public void TriggerRoar01Feedbacks()
    {
        roar01Feedbacks?.PlayFeedbacks();
    }

    public void TriggerAppearFeedbacks()
    {
        appearFeedbacks?.PlayFeedbacks();
    }

    public void TriggerSpitFeedbacks()
    {
        spitFeedbacks?.PlayFeedbacks();
    }

    public void TriggerIdleUndergroundFeedbacks()
    {
        idleUndergroundFeedbacks?.PlayFeedbacks();
    }

    public void TriggerIdleToJumpUndergroundFeedbacks()
    {
        idleToJumpUndergroundFeedbacks?.PlayFeedbacks();
    }

    public void TriggerBurrownIntoGroundFeedbacks()
    {
        idleToJumpUndergroundFeedbacks?.PlayFeedbacks();
    }

    public void TriggerContinuedRoarFeedbacks()
    {
        continuedRoarFeedbacks?.PlayFeedbacks();
    }
}