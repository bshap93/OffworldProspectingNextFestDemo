using UnityEngine;

namespace Domains.UI_Global.Reticle
{
    [CreateAssetMenu(fileName = "ReticleState", menuName = "UI/Reticle State")]
    public class ReticleState : ScriptableObject
    {
        [Header("Visual Settings")]
        public Sprite reticleSprite;
        public Color reticleColor = Color.white;
        
        [Header("State Information")]
        public string stateName;
        [TextArea(2, 4)]
        public string stateDescription;
    }
}