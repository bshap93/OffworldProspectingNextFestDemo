using DG.Tweening;
using Domains.Scripts_that_Need_Sorting;
using UnityEngine;

public class ConditionalDoor : MonoBehaviour
{
    [SerializeField] private GameObject screenMonitor;
    
    [SerializeField] string doorId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    [SerializeField] public bool startActive;


    [SerializeField] DoorsController doorsController;

    [SerializeField] private bool isLocked;

    private bool isActive;


    
    public bool GetLockedState()
    {
        return isLocked;
    }

    
    public void SetLocked(bool locked)
    {
        isLocked = locked;
        if (isLocked)
        {
            screenMonitor.GetComponent<Renderer>().material = inactiveMaterial;

        }
        else
        {
            screenMonitor.GetComponent<Renderer>().material = activeMaterial;

        }
    }
}