using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] ItemData[] Inventory;
=======
    //[SerializeField] InventoryManager mainInventory;
    [SerializeField] private List<ItemData> Inventory = new List<ItemData>();
    [SerializeField] private ItemData currentItem;
    [SerializeField] private Animator transition;
    [SerializeField] ReticleManager2 reticleManager;
    [SerializeField] private GameObject itemList;
    [SerializeField] private int startItem;
    //[SerializeField] private GameObject inventoryPrefab;
    //[SerializeField] private ItemUsageManager itemUsageManager;
    private float defaultObjectLocation = -564;

    //public Animator transition;

    // Positions for each of the UI boxes, i dont wanna code these in when we r gonna change them anyway later
    // posX-1
    // posY-1
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        
=======
        setCurrentItem(startItem);
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ItemData[] getInventory()
    {
        return Inventory;
    }

    public ItemData getItem(int i)
    {
<<<<<<< Updated upstream
        return Inventory[i];
=======
        return i >= 0 && i < Inventory.Count ? Inventory[i] : null;
    }

    public int getCurrentItemNum()
    {
        return currentItem.itemNum;
    }

    public void setCurrentItem(int i)
    {
        if (i >= 0 && i < Inventory.Count)
        {
            for (int j = 0; j < Inventory.Count; j++)
            {
                if (Inventory[j].itemNum == i)
                {
                    currentItem = Inventory[j];
                }
            }
        }
        if (reticleManager != null)
        {
            reticleManager.checkCurrentItem();
        }
        
    }

    public ItemData FindItemByName(string itemName)
    {
        return Inventory.FirstOrDefault(item => item.name == itemName);
    }

    public void RemoveItemByType(ItemData item)
    {
        if (Inventory.Contains(item))
        {
            Inventory.Remove(item);
            item.inInventory = false;
            //UpdateInventoryUI();
        }
>>>>>>> Stashed changes
    }

    public void addItem(ItemData item)
    {
<<<<<<< Updated upstream
        Inventory.Append(item);
=======
        if (!Inventory.Contains(item))
        {
            Inventory.Add(item);
            item.inInventory = true;
            //UpdateInventoryUI();
        }
>>>>>>> Stashed changes
    }

    public void removeItemByType(ItemData item)
    {
<<<<<<< Updated upstream
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


=======
        Inventory.Remove(item);
        item.inInventory = false;
        //UpdateInventoryUI();
    }

    public void RemoveItem(int i)
    {
        if (i >= 0 && i < Inventory.Count)
        {
            Inventory[i].inInventory = false;
            Inventory.RemoveAt(i);
        }
    }

    //private void UpdateInventoryUI()
    //{
    //    foreach (Transform child in itemList.transform)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    int index = 1;
    //    foreach (ItemData item in Inventory)
    //    {
    //        GameObject itemUI = Instantiate(inventoryPrefab, itemList.transform);
    //        itemUI.transform.position += new Vector3(defaultObjectLocation + (index * 400), 0, 0);
    //        Image iconImage = itemUI.GetComponentInChildren<Image>();
    //        TextMeshProUGUI itemNameText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
    //        Button button = itemUI.GetComponentInChildren<Button>();

    //        if (iconImage != null)
    //        {
    //            iconImage.sprite = item.GetItemIcon();
    //        }
    //        if (itemNameText != null)
    //        {
    //            itemNameText.text = item.name;
    //            button.onClick.AddListener(delegate { itemUsageManager.useItem(item.name); });
    //        }
    //        index++;
    //    }
    //}
>>>>>>> Stashed changes
}
