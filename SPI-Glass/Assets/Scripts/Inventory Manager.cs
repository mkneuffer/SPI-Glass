using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] InventoryManager mainInventory;
    [SerializeField] ItemData[] Inventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ItemData[] getInventory(InventoryManager mainInv)
    {
        return mainInv.Inventory;
    }

    public ItemData getItem(int i)
    {
        return Inventory[i];
    }

    public void addItem(ItemData item)
    {
        Inventory.Append(item);
    }

    public void removeItemByType(ItemData item)
    {
        for (int i = 0; i < Inventory.Length; i++) {
            if (Inventory[i] = item)
            {
                //;
            }
        }
    }

    public void removeItemByNum(int i)
    {
        Inventory[i] = null;
    }


}
