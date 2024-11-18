using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] InventoryManager mainInventory;
    [SerializeField] private List<ItemData> Inventory = new List<ItemData>();
    [SerializeField] private ItemData currentItem;

    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject inventoryPrefab;
    [SerializeField] private ItemUsageManager itemUsageManager;
    private float defaultObjectLocation = -564;


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
        CheckForHolyGrail();
    }

    private void CheckForHolyGrail()
{
    if (FindItemByName("Holy Grail Replica") != null)
    {
        SceneManager.LoadScene(3);
    }
}

    public List<ItemData> GetInventory()
    {
        return Inventory;
    }
    /*
        public ItemData[] getInventory(InventoryManager mainInv)
        {
            return mainInv.Inventory;
        }
    */
    public ItemData getItem(int i)
    {
        return i >= 0 && i < Inventory.Count ? Inventory[i] : null;
    }

    public void setCurrentItem(int i)
    {
        if (i >= 0 && i < Inventory.Count)
        {
            currentItem = Inventory[i];
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
            UpdateInventoryUI();
        }
    }

    public void addItem(ItemData item)
    {
        if (!Inventory.Contains(item))
        {
            Inventory.Add(item);
            item.inInventory = true;
            UpdateInventoryUI();
        }
    }

    public void RemoveItem(ItemData item)
    {
        Inventory.Remove(item);
        item.inInventory = false;
        UpdateInventoryUI();
    }

    public void RemoveItem(int i)
    {
        if (i >= 0 && i < Inventory.Count)
        {
            Inventory[i].inInventory = false;
            Inventory.RemoveAt(i);
        }
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        int index = 1;
        foreach (ItemData item in Inventory)
        {
            GameObject itemUI = Instantiate(inventoryPrefab, itemList.transform);
            itemUI.transform.position += new Vector3(defaultObjectLocation + (index * 400), 0, 0);
            Image iconImage = itemUI.GetComponentInChildren<Image>();
            TextMeshProUGUI itemNameText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
            Button button = itemUI.GetComponentInChildren<Button>();

            if (iconImage != null)
            {
                iconImage.sprite = item.GetItemIcon();
            }
            if (itemNameText != null)
            {
                itemNameText.text = item.name;
                button.onClick.AddListener(delegate { itemUsageManager.useItem(item.name); });
            }
            index++;
        }
    }
}
