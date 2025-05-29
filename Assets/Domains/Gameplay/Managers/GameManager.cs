using Heathen.SteamworksIntegration;
using Heathen.SteamworksIntegration.API;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SteamSettings settings;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        App.Client.Initialize(3621020);
        App.evtSteamInitialized.AddListener(HandleInitialized);
        App.evtSteamInitializationError.AddListener(HanldeInitalizationError);
        App.isDebugging = true;
        settings.Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void HandleInitialized()
    {
        Debug.Log("Success, Steam is ready to use");
    }

    private void HanldeInitalizationError(string message)
    {
        Debug.LogError("Failure, Steam reported error: " + message);
    }
}