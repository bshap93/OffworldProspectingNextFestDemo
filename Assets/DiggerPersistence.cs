using Digger.Modules.Runtime.Sources;
using Domains.UI_Global.Events;
using UnityEngine;

public class DiggerPersistence : MonoBehaviour
{
    private DiggerMasterRuntime _diggerMasterRuntime;

    private void Awake()
    {
        // _diggerMasterRuntime = GetComponent<DiggerMasterRuntime>();
        // _diggerMasterRuntime.ClearScene();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // _diggerMasterRuntime.PersistAll();
            // AlertEvent.Trigger(AlertReason.DiggerPersisted, "Digger data has been persisted at runtime.",
            //     "Digger Persisted");
        }
    }

    private void OnApplicationQuit()
    {
        // _diggerMasterRuntime.PersistAll();
        // AlertEvent.Trigger(AlertReason.DiggerPersisted, "Digger data has been persisted on application quit.",
        //     "Digger Persisted");
    }
}