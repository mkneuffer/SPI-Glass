using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ItemData holyGrailReplica;
    public void useItem(string item)
    {
        switch (item)
        {
            case "Wood":
                useWood();
                break;
            default:
                Debug.Log("Attempting to use an incorrect item. {" + item + "} is not a valid item or does not have any attached use data");
                break;
        }
    }


    private void useWood()
    {
        ItemData woodItem = inventoryManager.FindItemByName("Wood");
        inventoryManager.RemoveItem(woodItem);
        inventoryManager.addItem(holyGrailReplica);
    }
}
