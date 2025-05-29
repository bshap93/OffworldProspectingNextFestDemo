using Domains.Scripts_that_Need_Sorting;
using UnityEngine;

public class ToolPanelController : MonoBehaviour
{
    [SerializeField] private ToolPanelItem pickaxeItem;
    [SerializeField] private ToolPanelItem scannerItem;
    [SerializeField] private ToolPanelItem shovelItem;


    private void Start()
    {
        // Initialize the tool panel items
        pickaxeItem.Deselect();
        scannerItem.Deselect();
        shovelItem.Deselect();
    }


    public void ActivateToolPanelItem(ToolType toolType)
    {
        switch (toolType)
        {
            case ToolType.Pickaxe:
                pickaxeItem.Select();
                scannerItem.Deselect();
                shovelItem.Deselect();
                break;
            case ToolType.Scanner:
                scannerItem.Select();
                pickaxeItem.Deselect();
                shovelItem.Deselect();
                break;
            case ToolType.Shovel:
                shovelItem.Select();
                scannerItem.Deselect();
                pickaxeItem.Deselect();
                break;
        }
    }
}