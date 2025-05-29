using System;
using System.Collections;
using System.Collections.Generic;
using Domains.Gameplay.Objectives.Events;
using Domains.Gameplay.Objectives.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesListHUD : MonoBehaviour, MMEventListener<ObjectiveEvent>
{
    public GameObject ActiveObjectivesList;
    public GameObject CompletedObjectivesList;

    public GameObject ActiveObjectiveElementPrefab;
    public GameObject CompletedObjectiveElementPrefab;

    public List<GameObject> ActiveObjectiveElements = new();
    public List<GameObject> CompletedObjectiveElements = new();

    public Color ActiveObjectiveTextColor;
    public Color CompletedObjectiveTextColor;
    public int numCompletedObjectivesToShow;

    private ObjectivesManager objectivesManager;

    private void Start()
    {
        objectivesManager = FindFirstObjectByType<ObjectivesManager>();

        RefreshActiveObjectivesList();
        RefreshCompletedObjectivesList();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(ObjectiveEvent eventType)
    {
        if (eventType.type == ObjectiveEventType.ObjectiveActivated)
            StartCoroutine(DelayedRefresh(() => RefreshActiveObjectivesList()));
        else if (eventType.type == ObjectiveEventType.ObjectiveCompleted)
            StartCoroutine(DelayedRefresh(() =>
            {
                RefreshCompletedObjectivesList();
                RefreshActiveObjectivesList();
            }));
    }

    private IEnumerator DelayedRefresh(Action refreshAction)
    {
        yield return null; // Wait one frame
        refreshAction();
        yield return null;

    }

    public void RefreshActiveObjectivesList()
    {
        foreach (var element in ActiveObjectiveElements) Destroy(element);
        ActiveObjectiveElements.Clear();

        var activeObjectivesIds = ObjectivesManager.ActiveObjectives;

        foreach (var objectiveId in activeObjectivesIds)
        {
            var objectiveElement = Instantiate(ActiveObjectiveElementPrefab, ActiveObjectivesList.transform);
            var elementComponent = objectiveElement.GetComponent<ObjectiveElement>();
            var objectiveObject = objectivesManager.GetObjectiveById(objectiveId);

            if (objectiveObject != null)
            {
                elementComponent.ObjectiveText.text = objectiveObject.objectiveText;
                elementComponent.ObjectiveText.color = ActiveObjectiveTextColor;
                ActiveObjectiveElements.Add(objectiveElement);
            }
            else
            {
                Debug.LogWarning($"Objective with ID {objectiveId} not found in objectives list.");
            }
        }
    }

    public void RefreshCompletedObjectivesList()
    {
        foreach (var element in CompletedObjectiveElements) Destroy(element);
        CompletedObjectiveElements.Clear();

        var completedObjectivesIds = ObjectivesManager.CompletedObjectives;

        foreach (var objectiveId in completedObjectivesIds)
        {
            var objectiveElement = Instantiate(CompletedObjectiveElementPrefab, CompletedObjectivesList.transform);
            var elementComponent = objectiveElement.GetComponent<ObjectiveElement>();
            var objectiveObject = objectivesManager.GetObjectiveById(objectiveId);

            if (objectiveObject != null)
            {
                elementComponent.ObjectiveText.text = objectiveObject.objectiveText;
                elementComponent.ObjectiveText.color = CompletedObjectiveTextColor;
                // Strikethrough the text to indicate completion
                elementComponent.ObjectiveText.fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                Debug.LogWarning($"Objective with ID {objectiveId} not found in objectives list.");
            }


            CompletedObjectiveElements.Add(objectiveElement);

            // Limit the number of completed objectives shown
            if (CompletedObjectiveElements.Count > numCompletedObjectivesToShow)
            {
                Destroy(CompletedObjectiveElements[0]);
                CompletedObjectiveElements.RemoveAt(0);
            }
        }
    }
}