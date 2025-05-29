using UnityEngine;

public class SaveConsoleUI : MonoBehaviour
{
    // Starts out hidden
    public CanvasGroup SaveConsoleCanvasGroup;

    public void ShowSaveConsole()
    {
        SaveConsoleCanvasGroup.alpha = 1;
        SaveConsoleCanvasGroup.interactable = true;
        SaveConsoleCanvasGroup.blocksRaycasts = true;
    }

    public void HideSaveConsole()
    {
        SaveConsoleCanvasGroup.alpha = 0;
        SaveConsoleCanvasGroup.interactable = false;
        SaveConsoleCanvasGroup.blocksRaycasts = false;
    }
}