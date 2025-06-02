using UnityEngine;

public class ConditionalDoor : MonoBehaviour
{
    [SerializeField] private GameObject screenMonitor;

    [SerializeField] private string doorId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    [SerializeField] public bool startActive;


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
            screenMonitor.GetComponent<Renderer>().material = inactiveMaterial;
        else
            screenMonitor.GetComponent<Renderer>().material = activeMaterial;
    }
}