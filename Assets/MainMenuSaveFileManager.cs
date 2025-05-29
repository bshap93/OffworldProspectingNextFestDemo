using UnityEngine;

public class MainMenuSaveFileManager : MonoBehaviour
{
    [SerializeField] private GameObject continueGameButton;


    private void Start()
    {
        var isNewGame = ES3.Load("IsNewGame", "Progression.es3", false);


        if (isNewGame)
        {
            continueGameButton.gameObject.SetActive(false);
            Debug.Log("Continue Game Button Disabled");
        }
    }
}