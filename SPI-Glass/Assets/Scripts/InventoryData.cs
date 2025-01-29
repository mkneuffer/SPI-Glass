using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryData
{
    private List<int> inventory;

    public InventoryData(List<int> inventory)
    {
        this.inventory = inventory;
    }

    public override string ToString()
    {
        return ("items in inventory" + inventory);
    }

    public List<int> getInventory()
    {
        return inventory;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
