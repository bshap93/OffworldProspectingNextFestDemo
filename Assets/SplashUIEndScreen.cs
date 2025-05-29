using UnityEngine;

public class SplashUIEndScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}