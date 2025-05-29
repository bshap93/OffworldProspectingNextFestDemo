using UnityEngine;

namespace Domains.Gameplay.Objectives.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ObjectivesList", menuName = "Scriptable Objects/Objectives/ObjectivesList", order = 1)]
    public class ObjectivesList : ScriptableObject
    {
        [SerializeField] public ObjectiveObject[] objectives;
    }
}