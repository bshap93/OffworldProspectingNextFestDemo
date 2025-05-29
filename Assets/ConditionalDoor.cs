using DG.Tweening;
using UnityEngine;

public class ConditionalDoor : MonoBehaviour
{
    [SerializeField] private GameObject screenMonitor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    [SerializeField] private bool startActive;

    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [SerializeField] private Vector3 leftDoorMove;
    [SerializeField] private Vector3 rightDoorMove;

    private bool isActive;

    private void Start()
    {
        if (startActive)
            SetActive();
        else
            SetInactive();
    }

    public void SetActive()
    {
        isActive = true;
        screenMonitor.GetComponent<Renderer>().material = activeMaterial;
        if (leftDoor != null)
            SlideLeftDoor(leftDoor, true);
        if (rightDoor != null)
            SlideRightDoor(rightDoor, true);

        // Temporary
        leftDoor.SetActive(false);
        rightDoor.SetActive(false);
    }

    public void SetInactive()
    {
        isActive = false;
        screenMonitor.GetComponent<Renderer>().material = inactiveMaterial;
    }

    private void SlideLeftDoor(GameObject door, bool open)
    {
        if (open)
            leftDoor.transform.DOMoveX(leftDoorMove.x, 1);
    }

    private void SlideRightDoor(GameObject door, bool open)
    {
        if (open)
            rightDoor.transform.DOMoveX(rightDoorMove.x, 1);
    }
}