using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Domains.Gameplay.Objectives.ScriptableObjects
{
    [Serializable]
    public class QuestObject : ScriptableObject
    {
        [SerializeField] private string questName;
        [SerializeField] private GameObject infoPanelPrefab;
        [SerializeField] List<ObjectiveObject> objectives;
    }
}