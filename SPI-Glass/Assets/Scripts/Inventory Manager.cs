using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] InventoryManager mainInventory;
    [SerializeField] ItemData[] Inventory;
    [SerializeField] ItemData currentItem;
    
    // Positions for each of the UI boxes, i dont wanna code these in when we r gonna change them anyway later
    // posX-1
    // posY-1

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

    public void setCurrentItem(int i)
    {
        currentItem = Inventory[i];
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
