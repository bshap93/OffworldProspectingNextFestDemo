using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // Ensure that this GameObject is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
}