using System.Collections;
using ThirdParty.Feel.MMTools.Core.MMSceneLoading.Scripts.Managers;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] private string sceneName;


    public void LoadScene()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(sceneName))
            // Start the coroutine to load the scene after a delay
            StartCoroutine(LoadAfterDelay(0.5f));
        else
            Debug.LogError("Scene name is empty. Cannot load scene.");
    }

    private IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MMSceneLoadingManager.LoadScene(sceneName);
    }
}