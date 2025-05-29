using UnityEngine;

namespace Domains.Gameplay.Objectives.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ObjectiveObject", menuName = "Scriptable Objects/Objectives/ObjectiveObject",
        order = 1)]
    public class ObjectiveObject : ScriptableObject
    {
        [SerializeField] public string objectiveId;
        [SerializeField] public string objectiveText;
        [SerializeField] public string[] activateWhenCompleted;
        [SerializeField] public int numberOfObjectives = 1;
        
    }
}